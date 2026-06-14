# 03 — Gestion des exceptions

La règle, omniprésente dans ce projet : **tout bloc `Catch` produit un rapport
détaillé puis le traite**. Deux modules portent ce mécanisme :
`Exceptions/GestionExceptions.vb` et `Infrastructure/Journal.vb`.

---

## 1. `PreparerException` — le rapport diagnostic

Construit une chaîne riche, robuste (elle **ne lève jamais** d'exception
elle-même) :

- **code d'erreur MariaDB/MySQL** si l'exception est une `MySqlException` ;
- **horodatage**, nom de la base, assembly et méthode appelante ;
- **message**, `HResult`, `Source`, `TargetSite` ;
- **pile d'environnement** (`Environment.StackTrace`) et **pile d'exécution**
  (`ex.StackTrace`) ;
- **exception interne** et **exception racine** (on déroule les `InnerException`) ;
- **première frame source** (`StackTrace(ex, True)` → fichier + ligne + méthode),
  lorsque les symboles (PDB) sont disponibles.

```vbnet
If TypeOf ex Is MySqlException Then
    sb.AppendLine("Code erreur MySQL/MariaDB : " & DirectCast(ex, MySqlException).Number.ToString())
End If
Dim racine As Exception = ex
While racine.InnerException IsNot Nothing : racine = racine.InnerException : End While
```

Une **surcharge pratique** déduit automatiquement l'assembly et la méthode
appelante (`PreparerException(ex)`), utile pour les appels rapides.

---

## 2. `TraiterException` — et la garde anti-récursion

Journalise le rapport (fichier, sortie de débogage) puis l'enregistre en base
(`journal_erreur`) si `JournaliserEnBase` est actif.

Le **danger** : la journalisation en base ouvre une connexion qui peut échouer
(conteneur arrêté) ; sans précaution, on obtiendrait :

```
exception → TraiterException → LogErreur → connexion échoue
          → exception → TraiterException → … (récursion infinie)
```

D'où une **garde de réentrance** par thread :

```vbnet
<ThreadStatic> Private _enTraitement As Boolean

Public Sub TraiterException(rapport As String, Optional arret As Boolean = True)
    If _enTraitement Then
        Debug.WriteLine("[GestionExceptions] (reentrance) " & rapport)
        Return
    End If
    _enTraitement = True
    Try
        Journal.Ecrire(rapport, Journal.Niveau.Erreur)
        If JournaliserEnBase Then
            Try : AccesDonnees.LogErreur(rapport, OutilsSysteme.NomUtilisateur())
            Catch : End Try   ' jamais de re-traitement ici
        End If
    Finally
        _enTraitement = False
    End Try
End Sub
```

> Double protection : `LogErreur` est **elle-même défensive** (elle ne rappelle
> jamais `TraiterException`). On a donc deux verrous indépendants contre la
> récursion.

---

## 3. Couplage avec l'historique d'actions

Quand l'échec provient d'une commande SQL, `AccesDonnees.SignalerEchec` **annexe
au rapport les 5 dernières actions** de la connexion (`Action`, `ActionNM1`…
`ActionNM5`). Le rapport indique ainsi *ce que faisait la connexion juste avant*
l'erreur — voir [`02-base-de-donnees.md`](02-base-de-donnees.md).

---

## 4. `Journal` — le journal applicatif

Journal thread-safe (les `BackgroundWorker` écrivent depuis d'autres threads), il
conserve les traces :

1. dans la **sortie de débogage** de Visual Studio ;
2. dans un **fichier texte** horodaté (dossier temporaire) ;
3. dans une **mémoire tampon circulaire** (500 lignes) lue par la galerie.

L'événement `LigneAjoutee` permet à l'interface d'**afficher le journal en
direct** : c'est par lui qu'on observe l'ouverture des connexions, les
transactions, les fermetures automatiques et les exceptions.

```vbnet
Public Event LigneAjoutee(ByVal ligneFormatee As String, ByVal niveauLigne As Niveau)
```

La fenêtre principale s'y abonne et **marshale vers le thread d'interface**
(`BeginInvoke`) avant d'écrire dans la zone de texte.

---

## 5. À retenir

- Un seul point d'entrée pour journaliser une erreur → cohérence et traçabilité.
- Le rapport contient *tout* ce qu'il faut pour diagnostiquer sans débogueur.
- La récursion lors d'un échec de journalisation est un piège classique :
  **toujours** protéger le gestionnaire d'exceptions.

# Fiche 01 — Une séquence de démarrage ordonnée

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Le besoin

Au lancement, une application réalise plusieurs **vérifications/initialisations** :
configuration, connexion à la base, chargement des préférences, préparation du
cache… Plutôt que de les enchaîner « en dur », on les modélise comme une **séquence
d'étapes** : chacune a un nom et une vérification.

```vbnet
New EtapeDemarrage("Connexion à la base", Function()
                                              Dim m As String = ""
                                              Return DepotEvenement.TesterConnexion(m)
                                          End Function)
```

## Exécuter et collecter

`SequenceDemarrage.Executer` exécute chaque étape, capture une éventuelle exception,
et renvoie un **résultat par étape** (nom, réussite, message). On choisit de
**s'arrêter au premier échec** ou de poursuivre :

```vbnet
For Each etape In etapes
    Try
        Dim ok = etape.Verification.Invoke()
        resultat = New ResultatEtape(etape.Nom, ok, If(ok, "Terminé.", "Échec…"))
    Catch ex As Exception
        resultat = New ResultatEtape(etape.Nom, False, ex.Message)   ' une étape qui jette = un échec
    End Try
    resultats.Add(resultat)
    If Not resultat.Reussi AndAlso arreterAuPremierEchec Then Exit For
Next
```

## Pur = testable

`SequenceDemarrage` ne dépend ni de l'interface, ni de la base : on lui passe des
étapes (de simples lambdas) et on vérifie le comportement — toutes réussies, arrêt
sur échec, poursuite forcée, exception capturée — **sans rien afficher**.

## Brancher sur l'interface

L'UI construit la liste d'étapes, appelle `Executer`, affiche les résultats dans une
grille et **journalise** chaque résultat. Le `Program.Main` peut, lui, dérouler la
séquence pendant l'affichage du **splash**.

## À retenir

- Modéliser le démarrage en **étapes nommées** (nom + vérification).
- `Executer` collecte un **résultat par étape** et gère **arrêt/poursuite**.
- Logique **pure** → testable ; l'UI ne fait qu'afficher et journaliser.

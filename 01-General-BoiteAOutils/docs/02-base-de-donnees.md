# 02 — Accès aux données

C'est le cœur technique du projet. Trois sujets : le **wrapper de connexion**, les
**méthodes d'exécution avec seconde chance**, et les **requêtes paramétrées**.

---

## 1. Configuration — `ConfigBdd`

`Infrastructure/ConfigBdd.vb` centralise la chaîne de connexion. Les paramètres
sont lus dans `App.config` (clés `Bdd.*`), avec repli sur le conteneur Docker.

La chaîne est assemblée via **`MySqlConnectionStringBuilder`** plutôt que par
concaténation : mots-clés validés, valeurs échappées.

```vbnet
Dim builder As New MySqlConnectionStringBuilder() With {
    .Server = Serveur, .Port = Port, .Database = NomBase,
    .UserID = Utilisateur, .Password = MotDePasse,
    .Pooling = pooling, .UseAffectedRows = True,
    .DefaultCommandTimeout = 600, .ConnectionTimeout = 10,
    .SslMode = MySqlSslMode.Disabled
}
```

---

## 2. Le wrapper `ConnexionMySql`

Encapsule une connexion et son cycle de vie. Techniques mises en œuvre :

### a) Patron `IDisposable`
Libération déterministe : fermeture de la connexion, nettoyage du pool pour la
connexion principale (`MySqlConnection.ClearPool`), `GC.SuppressFinalize`.

### b) Timer d'auto-fermeture
Une connexion **secondaire** hors transaction est fermée après 60 s d'inactivité
(`System.Timers.Timer`). La connexion **principale** n'est jamais fermée
automatiquement.

### c) Historique d'actions `[n-1..n-5]`
La propriété `Action` décale automatiquement l'historique à chaque affectation :

```vbnet
Public Property Action As String
    Set(value As String)
        SyncLock _verrouAction
            _actionNM5 = _actionNM4 : _actionNM4 = _actionNM3
            _actionNM3 = _actionNM2 : _actionNM2 = _actionNM1
            _actionNM1 = _action    : _action = value
        End SyncLock
    End Set
End Property
```

En cas d'exception, on dispose ainsi des **5 dernières actions SQL** de la
connexion — une aide au diagnostic redoutable.

### d) Identifiant thread-safe
`NouvelIdConnexion()` incrémente un compteur partagé sous `SyncLock`.

> **Note** : la configuration de session (`innodb_lock_wait_timeout`) est
> appliquée **paresseusement** à la première ouverture réussie, et non dans le
> constructeur. Construire un objet ne provoque donc pas de connexion immédiate
> (robuste si la base est arrêtée).

---

## 3. Les méthodes d'exécution « avec seconde chance »

`Bdd/AccesDonnees.vb` expose `ExecuteNonQuery`, `ExecuteScalar`,
`GetDTfromCommand`. Principe : sur une **erreur transitoire**, rejouer la commande.

```vbnet
Try
    retour = cmd.ExecuteNonQuery()
Catch ex As Exception
    If DoitReessayer(ex, oMy, numeroTentative) Then
        RegenererEtReouvrir(cmd, oMy)        ' nouvelle connexion + réaffectation
        Return ExecuteNonQuery(cmd, oMy, methode, pile, True, numeroTentative + 1)
    End If
    If oMy?.Tr IsNot Nothing AndAlso numeroTentative > MAX_TENTATIVES Then
        oMy.Rollback() : Return -1
    End If
    SignalerEchec(ex, oMy)                   ' rapport + historique des actions
End Try
```

`DoitReessayer` cible deux cas typiques :
- **interbloquage InnoDB** (« Deadlock found when trying to get lock ») ;
- **`DataReader` resté ouvert** sur la connexion.

Au-delà de `MAX_TENTATIVES` (3), on abandonne (rollback si transaction).
`SignalerEchec` annexe l'historique `[n-1..n-5]` au rapport d'exception.

---

## 4. Requêtes paramétrées (anti-injection SQL)

**Toute** valeur variable passe par un paramètre `@nom` — jamais par
concaténation. Exemple (`IndicateursFiche`) :

```vbnet
"SELECT f.poids_kg AS `Poids (kg)`, " &
"  ROUND(f.rendement * f.poids_kg / 1000, 2) AS `Heures fab.`, " &
"  ROUND(f.poids_accessoires / NULLIF(f.poids_kg,0) * 100, 1) AS `% Accessoires` " &
"FROM fiche_article f WHERE f.id = @idFiche;"
...
cmd.Parameters.AddWithValue("@idFiche", idFiche)
```

Seul cas où l'on construit du SQL « à la main » : la liste `IN (...)` de
`VerifierArticles` — et **uniquement** à partir d'une `List(Of Integer)` (aucune
chaîne utilisateur n'y entre, donc pas d'injection possible).

### Ce que démontre chaque requête de démonstration

| Méthode `AccesDonnees` | Démontre |
|---|---|
| `IndicateursFiche` | colonnes calculées (ratios) côté serveur |
| `LignesParFiche` | jointures multi-tables |
| `MontantGlobalParFiche` | agrégats `SUM` / `COUNT` |
| `StatistiquesClients` | sous-requêtes corrélées, filtre daté |
| `VerifierArticles` | test d'existence + liste de problèmes |
| `LogErreur` | `INSERT` + `LAST_INSERT_ID()` |

---

## 5. Transactions

`BeginTransaction` / `Commit` / `Rollback` sur `ConnexionMySql`. La page
**Base de données** de la galerie en fait la démonstration : insertion dans une
transaction, comptage, puis **`ROLLBACK`** — on prouve, en comptant avant/pendant/
après, que rien n'a été persisté.

---

## 6. `INSERT` + `LAST_INSERT_ID()`

`LogErreur` insère une ligne dans `journal_erreur` et récupère l'identifiant
généré dans la même commande :

```sql
INSERT INTO journal_erreur (message, survenu_le, par_qui)
VALUES (@message, @date, @parQui); SELECT LAST_INSERT_ID();
```

`ExecuteScalar` renvoie alors le nouvel `id`. Cette méthode est **volontairement
défensive** : en cas d'échec, elle ne rappelle jamais le gestionnaire d'exceptions
(qui rappellerait `LogErreur`) — cf. [`03-exceptions.md`](03-exceptions.md).

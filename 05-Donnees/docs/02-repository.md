# 02 — Repository (dépôt) et pagination

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Le patron **Repository** encapsule l'accès à une collection d'entités derrière une
interface stable, indépendante du SQL.

```vbnet
Public Interface IDepot(Of T)
    Function Lister(page As Integer, taille As Integer) As List(Of T)
    Function Compter() As Long
    Function ParId(id As Integer) As T
    Function Inserer(entite As T) As Integer
    Function MettreAJour(entite As T) As Boolean
    Function Supprimer(id As Integer) As Boolean
End Interface
```

## Requêtes paramétrées

Toute valeur passe par un paramètre `@nom` — jamais de concaténation (anti-injection) :

```vbnet
Using cmd As New MySqlCommand("INSERT INTO produit (reference, designation, prix_ht, stock) " &
                              "VALUES (@ref, @des, @prix, @stock); SELECT LAST_INSERT_ID();", cn, tr)
    cmd.Parameters.AddWithValue("@ref", p.Reference)
    ' ...
    Return Convert.ToInt32(cmd.ExecuteScalar())
End Using
```

## Pagination

```sql
SELECT ... FROM produit ORDER BY id LIMIT @taille OFFSET @decalage;
```

`decalage = (page - 1) * taille`. `Compter()` fournit le total pour calculer le
nombre de pages. La page est rendue directement dans une grille.

## Colonnes aliasées pour le mapping

Les requêtes nomment les colonnes comme les **propriétés** de l'entité, ce qui
permet au mappeur de les apparier automatiquement :

```sql
SELECT id AS Id, reference AS Reference, designation AS Designation,
       prix_ht AS PrixHt, stock AS Stock FROM produit ...
```

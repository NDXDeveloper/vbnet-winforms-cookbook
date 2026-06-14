# 04 — Persistance en base

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Module : `Persistance/DepotDocuments.vb` (+ `Persistance/ConfigBdd.vb`).
Démonstration : page **Persistance en base**. Base : `coffre` (MariaDB 12.3).

L'idée : **sérialiser un objet puis le persister** dans une base, avec assez de
métadonnées pour le relire, le désérialiser et vérifier qu'il n'a pas été altéré.

---

## 1. Le schéma

```
categorie 1───* document
```

La table `document` conserve :

| Colonne | Rôle |
|---|---|
| `contenu` (LONGBLOB) | la charge utile sérialisée (texte UTF-8 ou binaire) |
| `type_clr` | le type .NET de l'objet (pour savoir quoi reconstruire) |
| `format` | le format employé (`xml`, `contrat_xml`, `binaire`, `json`) |
| `taille_octets` | la taille du payload |
| `empreinte_sha256` | empreinte d'intégrité du payload |
| `fk_categorie`, `cree_le` | classement et horodatage |

Le **BLOB** accepte indifféremment du texte ou du binaire : un seul schéma gère
les quatre formats.

---

## 2. Enregistrer (sérialiser → stocker)

```vbnet
Dim id As Integer = DepotDocuments.Enregistrer(
        "Catalogue de démonstration", monObjet, FormatSerialisation.Binaire, fkCategorie:=1)
```

`Enregistrer` :

1. sérialise l'objet en octets (`Serialiseur.VersOctets`) ;
2. calcule son **empreinte SHA-256** ;
3. insère le tout via une **requête paramétrée** (`@param`) — seule protection
   fiable contre l'injection SQL ;
4. renvoie l'identifiant créé (`SELECT LAST_INSERT_ID()`).

```vbnet
cmd.Parameters.AddWithValue("@contenu", octets)        ' Byte() -> BLOB
cmd.Parameters.AddWithValue("@empreinte", empreinte)   ' SHA-256 hexadécimal
```

---

## 3. Recharger (relire → désérialiser)

```vbnet
Dim copie As Catalogue = DepotDocuments.Recharger(Of Catalogue)(id)
```

`Recharger` lit le `contenu` (octets) et le `format`, puis délègue à
`Serialiseur.DepuisOctets(Of T)`. C'est l'aller-retour complet : l'objet ressort
identique à celui qui a été enregistré.

---

## 4. Vérifier l'intégrité

```vbnet
Dim intact As Boolean = DepotDocuments.VerifierIntegrite(id)
```

La méthode recalcule l'empreinte SHA-256 du payload stocké et la compare à
l'empreinte enregistrée : toute altération du contenu est détectée.

---

## 5. Lister

`Lister()` renvoie un `DataTable` des documents (id, libellé, format, taille,
catégorie, date) — directement exploitable par une grille. `ListerCategories()`
alimente la liste déroulante des catégories.

---

## 6. Configuration d'accès — `ConfigBdd`

La chaîne de connexion est assemblée via `MySqlConnectionStringBuilder`
(mots-clés validés, valeurs échappées) à partir des clés `Bdd.*` d'`App.config`,
avec repli sur le conteneur Docker (`127.0.0.1:3308`, base `coffre`).

```vbnet
Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
    cn.Open()
    ' ...
End Using
```

Chaque opération ouvre/ferme sa connexion dans un bloc `Using` : libération
déterministe des ressources.

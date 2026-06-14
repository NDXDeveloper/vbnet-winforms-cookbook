# 04 — Persistance en base

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Le puits `PuitsBase` écrit les entrées dans MariaDB (base `journal`, port 3309).

## Table `entree_journal`

| Colonne | Rôle |
|---|---|
| `survenu_le` | horodatage (milliseconde) |
| `niveau` | gravité **numérique** (0..4) → filtrage `>= seuil` |
| `niveau_libelle` | gravité **lisible** (DEBUG, INFO, …) |
| `categorie`, `message`, `exception` | contenu |
| `machine` | origine |

Stocker le niveau à la fois en numérique et en libellé permet de **filtrer**
efficacement (`WHERE niveau >= @min`) tout en restant **lisible**.

## Écriture (requête paramétrée)

```vbnet
cmd.Parameters.AddWithValue("@niveau", CInt(entree.Niveau))
cmd.Parameters.AddWithValue("@message", entree.Message)
' ... INSERT INTO entree_journal (...) VALUES (@date, @niveau, ...);
```

Toujours des **paramètres** (`@nom`) — jamais de concaténation (anti-injection SQL).

## Lecture filtrée

```vbnet
Dim table As DataTable = PuitsBase.Lire(Niveau.Avertissement, 200)
```

`Lire` renvoie un `DataTable` (id, date, niveau, catégorie, message) directement
exploitable par une grille, trié du plus récent au plus ancien et limité à `max`.

## Configuration

`ConfigBdd.ChaineConnexion()` assemble la chaîne via `MySqlConnectionStringBuilder`
à partir des clés `Bdd.*` d'`App.config` (repli sur `127.0.0.1:3309`, base `journal`).

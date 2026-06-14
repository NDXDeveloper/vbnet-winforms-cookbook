# Fiche 02 — Parcourir un arbre en base : `WITH RECURSIVE`

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Le besoin

« Donne-moi **tous** les descendants du nœud X. » Avec une liste d'adjacence, faire
une requête par niveau serait lent. La base sait parcourir l'arbre toute seule grâce
à une **expression de table commune récursive** (`WITH RECURSIVE`), prise en charge
par MariaDB.

## La requête

```sql
WITH RECURSIVE arbre AS (
    -- ancre : le nœud de départ
    SELECT id, parent_id, libelle, 0 AS profondeur
    FROM noeud WHERE id = @id
    UNION ALL
    -- récursion : les enfants des nœuds déjà trouvés
    SELECT n.id, n.parent_id, n.libelle, a.profondeur + 1
    FROM noeud n JOIN arbre a ON n.parent_id = a.id
)
SELECT id, libelle, profondeur
FROM arbre
WHERE id <> @id          -- on exclut le nœud de départ
ORDER BY profondeur, id;
```

- La partie **ancre** sélectionne le point de départ.
- La partie **récursive** rejoint les enfants des lignes déjà accumulées, jusqu'à
  épuisement.
- On calcule au passage la **profondeur** (niveau dans l'arbre).

## Adjacence vs récursif : que choisir ?

| | Liste d'adjacence + reconstruction | Requête récursive |
|---|---|---|
| Écriture/MAJ | très simple | très simple (même schéma) |
| Charger tout l'arbre | une requête + reconstruction en mémoire | possible aussi |
| « Tous les descendants de X » | coûteux côté client | **une seule** requête efficace |

Ici on combine les deux : **reconstruction en mémoire** pour l'affichage complet
(`TreeView`), **requête récursive** pour interroger une sous-arborescence précise.

## À retenir

- `WITH RECURSIVE` = **ancre** + **récursion** (`UNION ALL`) ; idéal pour
  « tous les descendants ».
- On peut calculer la **profondeur** dans la récursion.
- Combiner reconstruction en mémoire (affichage) et récursif (sous-arbre ciblé).

# Fiche 01 — Stocker un arbre : la liste d'adjacence

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Le modèle

La façon la plus simple de stocker un arbre en base relationnelle : chaque ligne
porte une référence vers son **parent** (`parent_id`). Une racine a `parent_id`
**NULL**. C'est la **liste d'adjacence**.

```sql
CREATE TABLE noeud (
    id        INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
    parent_id INT UNSIGNED NULL,
    libelle   VARCHAR(150) NOT NULL,
    CONSTRAINT fk_parent FOREIGN KEY (parent_id) REFERENCES noeud(id) ON DELETE CASCADE
);
```

- La **clé étrangère auto-référencée** garantit l'intégrité (un parent doit exister).
- `ON DELETE CASCADE` : supprimer un nœud supprime automatiquement sa
  sous-arborescence.

## Reconstruire l'arbre en mémoire

La table est « plate ». Pour obtenir un vrai arbre (racines + enfants), on charge
tout et on relie en **deux passes** O(n) à l'aide d'un dictionnaire :

```vbnet
' 1) un nœud d'arbre par identifiant
For Each n In plats : parId(n.Id) = New NoeudArbre(n) : Next
' 2) rattacher chacun à son parent (ou en faire une racine)
For Each n In plats
    If n.ParentId.HasValue AndAlso parId.ContainsKey(n.ParentId.Value) Then
        parId(n.ParentId.Value).Enfants.Add(parId(n.Id))
    Else
        racines.Add(parId(n.Id))      ' racine, ou orphelin (parent absent)
    End If
Next
```

C'est de la pure logique de structures de données : **testable** sans base (arbre
construit, orphelin promu en racine, liste vide…).

## Afficher : le `TreeView`

On projette ensuite chaque `NoeudArbre` en `TreeNode` (récursivement), en gardant
l'`Id` dans `Tag` pour retrouver le nœud lors des actions (ajout/suppression).

## À retenir

- **Liste d'adjacence** = `parent_id` (+ FK auto-référencée, `ON DELETE CASCADE`).
- Reconstruire l'arbre = **deux passes O(n)** via un dictionnaire (logique pure).
- Le `TreeView` n'est qu'une **projection** de l'arbre reconstruit.

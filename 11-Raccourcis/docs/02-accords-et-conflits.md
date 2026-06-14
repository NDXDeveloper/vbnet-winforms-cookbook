# Fiche 02 — Accords (chords) et conflits de préfixe

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Un accord = une suite de frappes

Comme dans les éditeurs modernes, un raccourci peut comporter **plusieurs
frappes** : `Ctrl+K` *puis* `Ctrl+S`. On parle d'**accord** (*chord*). `Raccourci`
modélise cette suite ; sa forme canonique joint les combinaisons par `, ` :
`Ctrl+K, Ctrl+S`.

## Reconnaissance : une machine à états

Le gestionnaire suit une **séquence en cours** et, à chaque frappe, renvoie un
état :

| État | Signification |
|---|---|
| **Déclenché** | la séquence correspond exactement à un raccourci inscrit |
| **En attente** | la séquence est un **préfixe** d'un accord plus long |
| **Aucun** | impasse : on réinitialise (en retentant avec la dernière frappe seule) |

```vbnet
_enCours.Add(c)
If correspondanceExacte() Then Return Declenchee   ' on exécute l'action
If estPrefixeDUnAccord() Then Return EnAttente      ' on garde la séquence
' sinon : on vide et on repart de la dernière frappe
```

Exemple : `Ctrl+K` → *en attente* ; puis `Ctrl+S` → *déclenché* (« Tout
enregistrer »). Mais `Ctrl+K` puis `Ctrl+X` → *aucun*, la séquence est annulée.

## Le piège : les conflits de préfixe

Si l'on inscrit **à la fois** `Ctrl+K` (action courte) et `Ctrl+K, Ctrl+S` (accord),
le système est ambigu : dès `Ctrl+K`, faut-il déclencher l'action courte ou
attendre la suite ? On **interdit** donc, à l'inscription, qu'un raccourci soit le
préfixe d'un autre :

```vbnet
If e.Racc.EstPrefixeDe(r) OrElse r.EstPrefixeDe(e.Racc) Then
    Throw New InvalidOperationException("Conflit de préfixe …")
End If
```

En revanche, deux accords qui **partagent** un préfixe mais **divergent** ensuite
(`Ctrl+K, Ctrl+S` et `Ctrl+K, Ctrl+C`) sont parfaitement compatibles : après
`Ctrl+K`, la seconde frappe les départage.

## À retenir

- Un **accord** est une séquence ; on le reconnaît avec une **machine à états**.
- Trois issues : *déclenché*, *en attente*, *aucun*.
- On **refuse les conflits de préfixe** à l'inscription ; les préfixes **partagés
  mais divergents** restent autorisés.

# Fiche 01 — Le test de survol (hit-testing)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## La question

Pour sélectionner une forme à la souris, il faut répondre : « le point cliqué
est-il **sur** cette forme ? » C'est le *hit-testing*. La réponse dépend de la
géométrie.

## Rectangle

Le point est dedans s'il est entre les bords. On **normalise** d'abord (la largeur
ou la hauteur peuvent être négatives si on a tracé « vers le haut/gauche ») :

```vbnet
Dim x1 = Math.Min(X, X + Largeur), x2 = Math.Max(X, X + Largeur)
Dim y1 = Math.Min(Y, Y + Hauteur), y2 = Math.Max(Y, Y + Hauteur)
Return px >= x1 AndAlso px <= x2 AndAlso py >= y1 AndAlso py <= y2
```

## Ellipse

On utilise l'équation de l'ellipse (centre `c`, demi-axes `rx`, `ry`) :

```vbnet
Dim dx = (px - cx) / rx, dy = (py - cy) / ry
Return (dx * dx + dy * dy) <= 1.0
```

> Conséquence intéressante : un point dans le **coin** du cadre n'est PAS dans
> l'ellipse — ce qu'un test rectangulaire raterait.

## Ligne (segment)

Une ligne n'a pas de surface : on accepte le clic s'il passe **assez près** du
segment (tolérance). On calcule la distance point↔segment en projetant le point sur
le segment (paramètre `t` borné à `[0, 1]`).

```vbnet
Dim t = ((px-ax)*dx + (py-ay)*dy) / longueur2   ' projection
t = Math.Max(0, Math.Min(1, t))                  ' bornée au segment
distance = ... <= TOLERANCE_LIGNE
```

## Pourquoi c'est pur (et testé)

Aucune de ces fonctions n'a besoin d'écran : ce sont des maths. On les teste donc
directement (point intérieur/extérieur, dimensions négatives normalisées, coin
d'ellipse exclu, point loin d'une ligne).

## À retenir

- Le *hit-testing* dépend de la **géométrie** (rectangle, ellipse, segment).
- **Normaliser** les dimensions négatives ; pour une ligne, raisonner en **distance**.
- C'est de la **logique pure** → testable sans interface.

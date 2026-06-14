# Fiche 02 — Le contrôle de tracé (owner-draw)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Un contrôle qui se dessine

`ControleGraphique` hérite de `Control`, active le **double tampon** (pas de
scintillement) et peint tout dans `OnPaint`. Il s'appuie sur `EchelleGraphique`
(fiche 01) pour positionner chaque valeur.

```vbnet
Dim echelle = EchelleGraphique.Auto(serie.Valeurs, hauteur)
Dim yZero = y0 + echelle.VersY(0)            ' ligne de base (valeur 0)
```

## Trois représentations d'une même série

| Mode | Rendu |
|---|---|
| **Barres** | un rectangle par valeur, de la ligne de base jusqu'à `VersY(valeur)` |
| **Courbe** | une polyligne reliant les points `(x, VersY(valeur))` |
| **Points** | un petit disque à chaque point |

```vbnet
' Barres
Dim yVal = y0 + echelle.VersY(serie.Valeurs(i))
g.FillRectangle(pinceau, cx - largeurBarre / 2, Math.Min(yVal, yZero), largeurBarre, Math.Abs(yVal - yZero))

' Courbe
g.DrawLines(stylo, points.ToArray())
```

L'abscisse de chaque point est régulière : `x = x0 + (i + 0,5) * pas`, où
`pas = largeur / nombreDePoints`.

## Axes et étiquettes

On trace l'axe Y (gauche), l'axe X (à la valeur 0), les repères **min/max**, et les
**libellés** sous chaque point — uniquement si la place le permet (`pas >= 24`),
pour éviter le chevauchement.

## À retenir

- Contrôle = `Control` **double tampon** + tout le rendu dans `OnPaint`.
- La même série se trace en **barres / courbe / points** selon un mode.
- Le positionnement vient de l'**échelle** (testée) ; le contrôle ne fait que **peindre**.

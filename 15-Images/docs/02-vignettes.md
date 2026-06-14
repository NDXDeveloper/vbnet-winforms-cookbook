# Fiche 02 — Vignettes : proportions et qualité

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Conserver les proportions

Réduire une image dans une boîte (ex. 160×160) **sans la déformer** revient à
choisir un **facteur d'échelle unique** : le plus petit des deux rapports
largeur/hauteur. On évite aussi d'**agrandir** une petite image.

```vbnet
Public Shared Function Dimensionner(srcL, srcH, maxL, maxH) As Size
    Dim ratio = Math.Min(maxL / CDbl(srcL), maxH / CDbl(srcH))
    If ratio >= 1.0 Then Return New Size(srcL, srcH)   ' pas d'agrandissement
    Return New Size(Max(1, Round(srcL * ratio)), Max(1, Round(srcH * ratio)))
End Function
```

Ce calcul est **pur** (que des nombres) : on le teste exhaustivement (paysage,
portrait, image déjà plus petite, dimensions invalides) **sans manipuler de
pixels**.

## Redimensionner avec qualité

Pour une vignette nette, on règle l'interpolation :

```vbnet
g.InterpolationMode = InterpolationMode.HighQualityBicubic
g.PixelOffsetMode = PixelOffsetMode.HighQuality
g.DrawImage(source, 0, 0, taille.Width, taille.Height)
```

## Pourquoi stocker la vignette (et pas l'originale)

Dans une médiathèque, afficher des centaines d'originaux en pleine résolution est
lent et lourd. On stocke donc la **vignette** (PNG compact) en base, et on garde
juste les **dimensions** de l'original comme métadonnée. La liste reste rapide à
charger.

```vbnet
Using v As Bitmap = Vignette.Generer(image)      ' réduite, proportions conservées
    vignettePng = Vignette.VersPng(v)            ' encodée en PNG -> LONGBLOB
End Using
```

## À retenir

- Vignette = **facteur d'échelle unique** (le plus petit rapport), **sans
  agrandissement**.
- Le calcul des dimensions est **pur** et testable ; le rendu utilise une
  interpolation **bicubique**.
- On persiste la **version réduite** (+ dimensions de l'original) pour une
  médiathèque légère.

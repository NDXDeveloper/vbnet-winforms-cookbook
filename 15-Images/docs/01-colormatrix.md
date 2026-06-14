# Fiche 01 — Filtres d'image par `ColorMatrix`

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Une couleur = un vecteur

Chaque pixel a quatre composantes : Rouge, Vert, Bleu, Alpha — un vecteur
`(R, V, B, A)`. Un **filtre colorimétrique** transforme ce vecteur. GDI+ utilise une
matrice **5×5** (`ColorMatrix`) : la 5ᵉ ligne/colonne permet les translations (terme
constant), comme en géométrie homogène.

```
[ R' V' B' A' 1 ] = [ R V B A 1 ] × M(5×5)
```

## Niveaux de gris

Pour désaturer, on remplace chaque composante par la **luminance perçue**
(pondération 0,299 / 0,587 / 0,114). Toutes les colonnes R, V, B reçoivent les mêmes
poids ⇒ R' = V' = B' :

```vbnet
New ColorMatrix(New Single()() {
    New Single() {0.299F, 0.299F, 0.299F, 0, 0},
    New Single() {0.587F, 0.587F, 0.587F, 0, 0},
    New Single() {0.114F, 0.114F, 0.114F, 0, 0},
    New Single() {0, 0, 0, 1, 0},
    New Single() {0, 0, 0, 0, 1}})
```

## Négatif et luminosité

- **Négatif** : R' = 1 − R (coefficient −1 sur la diagonale + 1 dans la ligne de
  translation).
- **Luminosité** : R' = facteur × R (facteur sur la diagonale RVB).

## Appliquer en une passe

On confie la matrice à `ImageAttributes`, puis on dessine l'image source dans une
nouvelle image : GDI+ applique la transformation à tous les pixels d'un coup.

```vbnet
Using attributs As New ImageAttributes()
    attributs.SetColorMatrix(matrice)
    g.DrawImage(source, rect, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributs)
End Using
```

> Toujours **`Using`** sur `Graphics`, `ImageAttributes`, `Bitmap` : ce sont des
> ressources GDI (cf. le projet « Interop Win32 » sur les fuites de handles).

## À retenir

- Un filtre couleur = une **matrice 5×5** appliquée à `(R, V, B, A, 1)`.
- Gris = mêmes poids de luminance sur R/V/B ; négatif = −1 + translation ;
  luminosité = facteur sur la diagonale.
- `ImageAttributes.SetColorMatrix` + `DrawImage` = application en **une passe**.

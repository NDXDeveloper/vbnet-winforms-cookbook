# Fiche 02 — Zoom / déplacement, et pourquoi isoler la logique

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Zoomer et déplacer avec une transformation

Plutôt que de recalculer la position de chaque pixel, on **transforme le repère**
de dessin avant de tracer l'image. Deux opérations suffisent :

```vbnet
' Place l'origine au centre de la vue, décalée par le glisser de souris…
g.TranslateTransform(Me.Width / 2.0F + _decalage.X, Me.Height / 2.0F + _decalage.Y)
' …puis applique le facteur de zoom.
g.ScaleTransform(CSng(_zoom), CSng(_zoom))
' L'image est dessinée centrée sur l'origine -> elle zoome "autour" du centre.
g.DrawImage(_image, -_image.Width / 2.0F, -_image.Height / 2.0F, _image.Width, _image.Height)
```

Le **déplacement** (pan) se résume à mémoriser un décalage pendant le glisser :

```vbnet
Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
    If _glisse Then
        _decalage = New PointF(_origineDecalage.X + (e.X - _origineSouris.X),
                               _origineDecalage.Y + (e.Y - _origineSouris.Y))
        Invalidate()
    End If
End Sub
```

Le **zoom** réagit à la molette, par crans de `e.Delta \ 120` :

```vbnet
Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
    _zoom = CalculZoom.AppliquerCrans(_zoom, e.Delta \ 120)
    Invalidate()
End Sub
```

## Pourquoi sortir le calcul du contrôle ?

Un contrôle graphique est **difficile à tester** : il faut une fenêtre, un handle,
une boucle de messages. Mais la *règle* du zoom — multiplier par un pas, borner
entre 10 % et 1000 % — est de la simple arithmétique. On l'isole donc dans une
classe pure, `CalculZoom`, sans aucune dépendance à l'interface :

```vbnet
Public Shared Function AppliquerCrans(zoom As Double, crans As Integer) As Double
    Return Borner(zoom * Math.Pow(Pas, crans))
End Function
```

Conséquence : on peut écrire des tests **rapides et fiables** (« 100 crans positifs
plafonnent à `ZoomMax` », « un cran négatif divise par le pas »…) sans jamais
ouvrir de fenêtre. Le contrôle, lui, se contente d'appeler cette fonction.

## À retenir

- Zoom + déplacement = **une transformation** (`TranslateTransform` +
  `ScaleTransform`), pas un recalcul pixel par pixel.
- La molette se lit en **crans** (`e.Delta \ 120`).
- **Isoler la logique** du rendu : une classe pure se teste sans interface
  graphique, et reste réutilisable.

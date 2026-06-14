# Fiche 02 — Un canevas interactif en double tampon

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Le contrôle de dessin

On dérive un `Panel` en activant le **double tampon** (rendu en mémoire puis copie
unique → pas de scintillement, même pendant le glisser) :

```vbnet
Public NotInheritable Class Canevas
    Inherits Panel
    Public Sub New()
        Me.DoubleBuffered = True
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Scene.DessinerSur(e.Graphics)       ' toutes les formes
        If _enCours IsNot Nothing Then _enCours.DessinerSur(e.Graphics)  ' aperçu
    End Sub
End Class
```

## Créer une forme au glisser

- **MouseDown** : on mémorise le point de départ et on crée une forme « en cours ».
- **MouseMove** : on met à jour sa largeur/hauteur (delta) et on `Invalidate()`.
- **MouseUp** : si la forme a une taille suffisante, on l'ajoute à la `Scene`.

```vbnet
Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
    _enCours.Largeur = e.X - _depart.X
    _enCours.Hauteur = e.Y - _depart.Y
    Invalidate()
End Sub
```

## Déplacer une forme (mode sélection)

En mode déplacement, on saisit la forme la plus haute sous le curseur via le
hit-testing, puis on la suit :

```vbnet
' MouseDown
_selection = Scene.TrouverA(e.X, e.Y)
_decalage = New Size(e.X - _selection.X, e.Y - _selection.Y)
' MouseMove
_selection.X = e.X - _decalage.Width
_selection.Y = e.Y - _decalage.Height
Invalidate()
```

## Toujours redessiner via `Invalidate`

On ne peint jamais hors de `OnPaint` : on **invalide** le contrôle, Windows
replanifie un rendu. Le double tampon garantit la fluidité.

## À retenir

- Un canevas = `Panel` **double tampon** + dessin dans `OnPaint`.
- Création au **glisser** (down/move/up) ; déplacement via **hit-testing**.
- `Invalidate()` pour redessiner — jamais de dessin direct hors `OnPaint`.

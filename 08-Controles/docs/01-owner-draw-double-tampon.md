# Fiche 01 — Dessin propriétaire (owner-draw) et double tampon

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Peindre soi-même un contrôle

Pour un rendu sur mesure (un interrupteur, une jauge…), on hérite de `Control` et
on redéfinit `OnPaint`. Tout passe par l'objet `Graphics` fourni :

```vbnet
Protected Overrides Sub OnPaint(e As PaintEventArgs)
    Dim g = e.Graphics
    g.SmoothingMode = SmoothingMode.AntiAlias        ' bords lissés
    Using chemin = CheminArrondi(rect, rect.Height \ 2)
        Using pinceau As New SolidBrush(If(_actif, CouleurActive, CouleurInactive))
            g.FillPath(pinceau, chemin)              ' la piste
        End Using
    End Using
    g.FillEllipse(Brushes.White, x, marge, diametre, diametre)  ' la pastille
End Sub
```

> Règle d'or : **libérer** les objets GDI+ (`Pen`, `Brush`, `GraphicsPath`) avec
> `Using`. Ce sont des ressources natives qui ne se nettoient pas toutes seules.

## Redessiner au bon moment

On ne peint **jamais** hors de `OnPaint`. Pour rafraîchir après un changement
d'état, on appelle `Invalidate()` : Windows planifie alors un nouveau `OnPaint`.

```vbnet
Set(value As Boolean)
    If _actif <> value Then
        _actif = value
        Invalidate()                       ' demande un nouveau rendu
        RaiseEvent BasculeModifiee(Me, EventArgs.Empty)
    End If
End Set
```

## Le double tampon contre le scintillement

Peindre directement à l'écran, élément par élément, fait **clignoter** le contrôle.
La solution : peindre dans une image mémoire, puis la copier d'un seul coup. WinForms
le fait pour nous si l'on active les bons styles dans le constructeur :

```vbnet
SetStyle(ControlStyles.AllPaintingInWmPaint Or
         ControlStyles.UserPaint Or
         ControlStyles.OptimizedDoubleBuffer Or
         ControlStyles.ResizeRedraw, True)
```

| Style | Effet |
|---|---|
| `UserPaint` | le contrôle se dessine lui-même (pas Windows) |
| `AllPaintingInWmPaint` | tout le rendu passe par `OnPaint` (évite un effacement séparé) |
| `OptimizedDoubleBuffer` | rendu en mémoire puis copie unique → **pas de scintillement** |
| `ResizeRedraw` | redessine automatiquement au redimensionnement |

## À retenir

- Hériter de `Control`, redéfinir `OnPaint`, tout dessiner via `Graphics`.
- **`Using`** sur chaque objet GDI+.
- `Invalidate()` pour demander un rafraîchissement (jamais peindre ailleurs).
- Les 4 styles ci-dessus = un contrôle fluide, sans clignotement.

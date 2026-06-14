# Fiche 02 — Owner-draw : reprendre la main sur un contrôle standard

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Le principe

Certains contrôles standard (`TabControl`, `ListBox`, `ComboBox`, `MenuItem`…)
permettent de **dessiner soi-même** chaque élément : c'est l'*owner-draw*. On active
un mode propriétaire, puis on prend en charge le rendu.

## Onglets peints

```vbnet
Public Class OngletsPeints
    Inherits TabControl
    Public Sub New()
        Me.DrawMode = TabDrawMode.OwnerDrawFixed   ' je dessine les onglets
        Me.SizeMode = TabSizeMode.Fixed
    End Sub
    Protected Overrides Sub OnDrawItem(e As DrawItemEventArgs)
        Dim rect = Me.GetTabRect(e.Index)
        Dim actif = (e.Index = Me.SelectedIndex)
        Using fond As New SolidBrush(If(actif, CouleurAccent, CouleurInactif))
            e.Graphics.FillRectangle(fond, rect)
        End Using
        TextRenderer.DrawText(e.Graphics, Me.TabPages(e.Index).Text, police, rect, ...)
    End Sub
End Class
```

`GetTabRect(index)` donne la zone de chaque onglet ; on remplit, puis on écrit le
texte (en gras pour l'onglet actif).

## Une grille pré-stylée par héritage

Tout n'exige pas de l'owner-draw : pour une grille, hériter de `DataGridView` et
**fixer les styles** dans le constructeur suffit, et évite de reconfigurer
l'apparence partout :

```vbnet
Me.EnableHeadersVisualStyles = False
Me.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243)
Me.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250)
Me.ReadOnly = True
```

## À retenir

- **Owner-draw** : activer le mode propriétaire (`OwnerDrawFixed`) puis dessiner
  dans `OnDrawItem` (utiliser `GetTabRect`/`e.Bounds`).
- Pour de la simple **cohérence visuelle**, l'héritage + styles par défaut suffit
  (pas besoin de tout peindre).
- Toujours **libérer** les objets GDI+ (`Using`).

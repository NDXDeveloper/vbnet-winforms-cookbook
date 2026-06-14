# Fiche 01 — `PrintDocument` : impression événementielle

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Le modèle : un événement par page

En .NET, on n'« envoie » pas une image figée à l'imprimante : on **dessine** chaque
page à la demande. Le moteur lève l'événement `PrintPage` ; on y peint via l'objet
`Graphics` (comme pour un contrôle), puis on indique s'il reste des pages.

```vbnet
Dim doc As New PrintDocument()
AddHandler doc.BeginPrint, Sub(s, e) _index = 0          ' réinitialise le curseur
AddHandler doc.PrintPage, AddressOf SurPage              ' dessine UNE page
```

```vbnet
Private Sub SurPage(sender As Object, e As PrintPageEventArgs)
    Dim y = e.MarginBounds.Top
    While _index < _lignes.Count AndAlso (y + hauteurLigne) <= e.MarginBounds.Bottom
        e.Graphics.DrawString(_lignes(_index), _police, Brushes.Black, e.MarginBounds.Left, y)
        y += hauteurLigne
        _index += 1
    End While
    e.HasMorePages = _index < _lignes.Count               ' reste-t-il des lignes ?
End Sub
```

Tant que `HasMorePages = True`, le moteur rappelle `PrintPage` pour la page suivante.

## `MarginBounds` : la zone imprimable

`e.MarginBounds` donne le rectangle utile (hors marges), en centièmes de pouce. On
s'y réfère pour savoir où commencer (`Top`, `Left`) et quand arrêter (`Bottom`).

## Un seul document, deux usages

Le **même** `PrintDocument` sert à :

- l'**aperçu** : `New PrintPreviewDialog() With {.Document = doc}` puis `ShowDialog()` ;
- l'**impression** : `New PrintDialog() With {.Document = doc}`, et si l'utilisateur
  valide, `doc.Print()`.

> Pré-requis : au moins une imprimante installée (Windows fournit « Microsoft Print
> to PDF »). Sans imprimante par défaut, l'aperçu lève une exception — on l'entoure
> donc d'un `Try/Catch` dans l'interface.

## À retenir

- L'impression est **événementielle** : `PrintPage` dessine une page via `Graphics`.
- `HasMorePages` pilote la boucle de pages ; `BeginPrint` réinitialise l'état.
- Le **même** `PrintDocument` alimente l'aperçu et l'impression.

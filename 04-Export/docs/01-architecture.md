# 01 — Architecture

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

```
        NDX.Export.UI                NDX.Export.Tests
        (WinExe, WinForms)           (MSTest)
                 │                          │
                 └────────────┬─────────────┘
                              ▼
                     NDX.Export.Core
                     (exportateurs)  ◄── DataTable ── MariaDB (source)
```

## Les trois projets

| Projet | Type | Rôle |
|---|---|---|
| `NDX.Export.Core` | bibliothèque | exportateurs CSV/Excel/PDF + source de données |
| `NDX.Export.UI` | WinForms | galerie : charger une source, prévisualiser, exporter |
| `NDX.Export.Tests` | MSTest | exportateurs (logique) + source (intégration) |

## Principe : une interface, plusieurs formats

```vbnet
Public Interface IExportateur
    ReadOnly Property Format As FormatExport
    ReadOnly Property Extension As String
    ReadOnly Property TypeMime As String
    Function Exporter(table As DataTable) As Byte()
End Interface
```

`ExportateurCsv`, `ExportateurExcel` et `ExportateurPdf` implémentent ce contrat.
La fabrique `Exportateurs.Creer(format)` choisit la bonne implémentation. Ajouter
un format n'impacte ni l'IHM ni les autres exportateurs.

## Format de projet

SDK-style, cible **`net481`**, `Option Strict On`. Le `Core` référence
`System.IO.Compression` (archive ZIP du xlsx), `System.Data`, `MySql.Data`.

## Aucune dépendance tierce

Les trois formats sont produits avec les seules briques du .NET Framework :
`StringBuilder` (CSV), `System.IO.Compression.ZipArchive` (xlsx), écriture
d'octets (PDF). C'est volontaire : comprendre les formats plutôt que masquer
leur structure derrière une bibliothèque.

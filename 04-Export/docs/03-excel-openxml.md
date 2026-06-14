# 03 — Export Excel (.xlsx, Office Open XML)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Un fichier `.xlsx` n'est pas un format binaire opaque : c'est une **archive ZIP**
(paquet *Office Open XML*) contenant des **parties XML**. On peut donc le produire
avec `System.IO.Compression.ZipArchive`, **sans Excel ni bibliothèque tierce**.

## Parties minimales assemblées

| Partie | Rôle |
|---|---|
| `[Content_Types].xml` | déclare le type de chaque partie |
| `_rels/.rels` | relation racine → le classeur |
| `xl/workbook.xml` | le classeur et sa liste de feuilles |
| `xl/_rels/workbook.xml.rels` | relation classeur → feuille |
| `xl/worksheets/sheet1.xml` | les données de la feuille |

```vbnet
Using archive As New ZipArchive(flux, ZipArchiveMode.Create, leaveOpen:=True)
    EcrireEntree(archive, "[Content_Types].xml", ContentTypes())
    EcrireEntree(archive, "_rels/.rels", RelationsRacine())
    EcrireEntree(archive, "xl/workbook.xml", Workbook())
    EcrireEntree(archive, "xl/_rels/workbook.xml.rels", RelationsWorkbook())
    EcrireEntree(archive, "xl/worksheets/sheet1.xml", Feuille(table))
End Using
```

## Cellules

- **Texte** : cellule de type `inlineStr` (chaîne en ligne, sans table de chaînes
  partagées — plus simple), avec échappement XML.
- **Nombre** : cellule numérique `<c r="B2"><v>200</v></c>`, valeur en culture
  invariante (séparateur décimal `.`).

La **référence de cellule** (`A1`, `B2`, `AA10`) se calcule depuis l'index de
colonne (base 26) et le numéro de ligne.

```vbnet
' colonne 1 -> "A", 27 -> "AA"
While n > 0
    Dim reste = (n - 1) Mod 26
    nom = Chr(Asc("A"c) + reste) & nom
    n = (n - 1) \ 26
End While
```

## À retenir

Comprendre qu'un `.xlsx` (ou `.docx`, `.pptx`) est un **ZIP de XML** démystifie
ces formats : on peut les lire, les produire et les corriger à la main. Pour des
classeurs riches (styles, formules, multi-feuilles), une bibliothèque dédiée
reste préférable — mais le principe est là.

# Fiche 01 — Un `.xlsx`, c'est un ZIP de XML (OpenXML)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Le format

Un fichier `.xlsx` n'a rien de magique : c'est une **archive ZIP** contenant des
parties **XML** (norme OpenXML). On peut donc le créer et le lire avec
`System.IO.Compression`, **sans installer Excel**.

```
classeur.xlsx (ZIP)
├── [Content_Types].xml          ← déclare les types de parties
├── _rels/.rels                  ← relation racine -> workbook
└── xl/
    ├── workbook.xml             ← liste des feuilles
    ├── _rels/workbook.xml.rels  ← relation workbook -> feuille
    └── worksheets/sheet1.xml    ← les cellules
```

## Écrire les cellules

On écrit chaque cellule en **chaîne en ligne** (`inlineStr`), ce qui garantit un
aller-retour fidèle (pas de table de chaînes partagées à gérer) :

```xml
<row r="1">
  <c r="A1" t="inlineStr"><is><t>Produit</t></is></c>
  <c r="B1" t="inlineStr"><is><t>Quantité</t></is></c>
</row>
```

La référence de cellule (`A1`, `B1`…) vient de `ReferenceCellule.ColonneVersLettres`.
On échappe `&`, `<`, `>` dans le texte.

## Lire les cellules

À la lecture, on ouvre le ZIP, on charge `sheet1.xml`, et pour chaque `<c>` on
récupère sa valeur selon son type :

| `t` | Où est la valeur |
|---|---|
| `inlineStr` | `<is><t>…</t></is>` (nos fichiers) |
| `s` | index dans `xl/sharedStrings.xml` (fichiers Excel) |
| *(absent)* | `<v>…</v>` (nombre) |

On gère les trois cas : nos propres fichiers **et** ceux produits par Excel.

## Testable sans Office

Comme tout passe par des flux, on teste l'**aller-retour** en mémoire : écrire une
grille dans un `MemoryStream`, la relire, comparer — et vérifier que les caractères
spéciaux (`&`, `<`, `>`) sont préservés. Aucune installation requise.

## À retenir

- Un `.xlsx` = **ZIP + XML** : créable/lisible avec `System.IO.Compression`.
- Écrire en `inlineStr` simplifie l'aller-retour ; lire gère aussi `s` et `<v>`.
- Le tout est **testable** sans Excel (aller-retour en `MemoryStream`).

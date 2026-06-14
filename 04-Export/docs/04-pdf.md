# 04 — Export PDF (généré à la main)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Un PDF minimal est un fichier texte structuré : un en-tête, une suite d'**objets**
numérotés, une **table de références croisées** (`xref`) qui donne la position de
chaque objet, et un **trailer**. `ExportateurPdf` l'assemble octet par octet.

## Structure produite

```
%PDF-1.4
1 0 obj  << /Type /Catalog /Pages 2 0 R >>            endobj
2 0 obj  << /Type /Pages /Kids [4 0 R ...] /Count N >> endobj
3 0 obj  << /Type /Font /Subtype /Type1 /BaseFont /Courier >> endobj
4 0 obj  << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842]
            /Resources << /Font << /F1 3 0 R >> >> /Contents 5 0 R >> endobj
5 0 obj  << /Length L >> stream ... ET endstream endobj
xref
0 M
0000000000 65535 f
0000000015 00000 n
...
trailer << /Size M /Root 1 0 R >>
startxref
<offset de xref>
%%EOF
```

## Points clés

- **Offsets** : on écrit les objets dans un flux en mémoire en **mémorisant la
  position** de chacun ; la table `xref` rejoue ensuite ces positions (10 chiffres,
  format imposé de 20 octets par entrée).
- **Texte** : flux de contenu en opérateurs PDF — `BT … ET` (bloc texte),
  `/F1 9 Tf` (police/taille), `Td` (position), `TL` + `Tj` + `T*` (ligne puis saut).
- **Police** : Courier (à chasse fixe) → colonnes alignées sans calcul de largeur.
- **Pagination** : les lignes sont découpées en pages (une page = un objet Page +
  un flux de contenu).
- **Échappement** : `\`, `(` et `)` sont échappés ; les caractères non-ASCII sont
  remplacés par `?` (la police de base utilise un encodage standard).

## À retenir

Le PDF paraît opaque mais reste lisible et reproductible. Pour des documents
riches (images, polices embarquées, mise en page complexe), une bibliothèque
dédiée est indiquée ; ici, l'objectif est de **comprendre la mécanique**.

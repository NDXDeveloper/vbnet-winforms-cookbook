# Fiche 02 — Texte, coordonnées et retour à la ligne

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Écrire du texte

Dans un flux de contenu, le texte se place entre `BT` (begin text) et `ET`, avec
la police (`Tf`), une matrice de position (`Tm`) et la chaîne (`Tj`) :

```
BT
/F1 12 Tf                 ← police F1, corps 12
1 0 0 1 56 742 Tm         ← position (x, y) dans le repère PDF
(Bonjour) Tj              ← dessine la chaîne
ET
```

Les chaînes littérales sont entre parenthèses : il faut **échapper** `(`, `)` et
`\`.

## Le piège des coordonnées

Le repère PDF a son **origine en bas à gauche**, l'axe Y vers le **haut** —
l'inverse de l'habitude écran. Pour rester intuitif, l'API expose un repère
**haut-gauche** et convertit en interne :

```vbnet
' y mesuré depuis le HAUT -> ordonnée PDF mesurée depuis le BAS
_contenu.Append("1 0 0 1 ").Append(Fmt(x)).Append(" ").Append(Fmt(Hauteur - y)).Append(" Tm")
```

## Les accents

Les polices sont déclarées en **`/WinAnsiEncoding`**. En sérialisant le texte en
**Windows-1252**, les caractères `é à ç ù œ …` tombent sur les bons codes et
s'affichent correctement, sans incorporer de police.

## Le retour à la ligne, exactement

Couper un texte à une largeur donnée suppose de **mesurer** la largeur de chaque
caractère. Pour une police à **chasse fixe** comme Courier, c'est trivial et
**exact** : chaque caractère occupe `0,6 × taille` points. On en déduit le nombre
de caractères par ligne :

```vbnet
Public Shared Function CaracteresParLigne(largeurPts As Double, taillePolice As Double) As Integer
    Return Math.Max(1, CInt(Math.Floor(largeurPts / (0.6 * taillePolice))))
End Function
```

L'algorithme de découpe est alors un **word-wrap glouton** classique : on accumule
les mots tant qu'ils tiennent, on passe à la ligne sinon, et on coupe les mots plus
longs que la largeur. Comme tout cela est de l'arithmétique pure, on le **teste
sans rien dessiner** (largeur respectée, mots longs coupés, sauts de ligne
préservés).

## À retenir

- Texte = `BT … /F{n} {taille} Tf … Tm … (texte) Tj … ET`, avec échappement.
- Le repère PDF part **du bas** : convertir depuis un repère haut-gauche est plus simple à utiliser.
- `WinAnsiEncoding` + sérialisation **Windows-1252** = accents corrects.
- Le retour à la ligne est **exact** en chasse fixe → logique pure et testable.

# Fiche 01 — La mise à l'échelle : valeur → pixel

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Le cœur d'un graphique

Tracer des données, c'est avant tout **convertir des valeurs en positions**. Pour
l'axe vertical, on transforme une valeur (entre un **bas** et un **haut**) en une
**ordonnée pixel** dans une zone de hauteur donnée — sachant que, à l'écran, Y croît
**vers le bas** (donc la plus grande valeur est en haut).

```vbnet
Public Function VersY(valeur As Double) As Double
    Dim etendue = Haut - Bas
    If etendue <= 0 Then Return HauteurPixels            ' échelle plate : tout en bas
    Dim fraction = (valeur - Bas) / etendue
    fraction = Math.Max(0, Math.Min(1, fraction))        ' on borne à la zone
    Return HauteurPixels * (1 - fraction)                ' inversion haut/bas
End Function
```

## Choisir les bornes automatiquement

Pour des barres, on veut souvent partir de **zéro** ; pour des valeurs négatives, il
faut descendre en dessous. D'où une échelle **automatique** :

```vbnet
Dim bas = Math.Min(0, valeurs.Min())   ' 0 si tout est positif ; sinon la plus petite
Dim haut = valeurs.Max()
If haut <= bas Then haut = bas + 1      ' évite une étendue nulle
```

## Pourquoi l'isoler

Ce calcul est **pur** : il ne dépend ni de l'écran, ni de la base. On le teste donc
exhaustivement et sans interface :

- bas en bas, haut en haut, médiane au milieu ;
- valeurs hors bornes **plafonnées** ;
- **échelle plate** (bas = haut) sans division par zéro ;
- bornes auto (tout positif → départ à 0 ; négatives incluses ; liste vide).

Le contrôle de dessin se contente ensuite d'appeler `VersY` pour chaque point.

## À retenir

- Un graphique = une **conversion valeur → pixel** (avec inversion de l'axe Y).
- **Borner** la valeur à la zone et gérer l'**échelle plate** (pas de /0).
- Calcul **pur** → testé sans rien dessiner ; le rendu vient après.

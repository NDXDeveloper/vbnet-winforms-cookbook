# 05 — Géométrie

> Calculs géométriques de base sur des points 2D.

Module : `Geometrie/OutilsGeometrie.vb`. Démonstration interactive : page
**Géométrie** de la galerie (dessin live, angle réglable au curseur).

---

## 1. Point milieu — `PointMilieu`

Moyenne des coordonnées : `((Ax+Bx)/2, (Ay+By)/2)`.

---

## 2. Distance au carré — `DistanceAuCarre`

```vbnet
Dim dx = pt1.X - pt2.X
Dim dy = pt1.Y - pt2.Y
Return dx * dx + dy * dy
```

**Pourquoi le carré ?** Pour *comparer* des distances (trouver le point le plus
proche, par exemple), inutile de calculer la racine carrée : l'ordre est préservé,
et on évite un `Math.Sqrt` coûteux (et une conversion en flottant). Technique
classique d'optimisation géométrique.

---

## 3. Rotation d'un point — `PivoterPoint`

Applique la **matrice de rotation** standard autour d'un centre, angle en degrés
converti en radians :

```
x' = cos·(x−cx) − sin·(y−cy) + cx
y' = sin·(x−cx) + cos·(y−cy) + cy
```

```vbnet
Dim angleRad = angleEnDegres * (Math.PI / 180.0)
Dim cos = Math.Cos(angleRad), sin = Math.Sin(angleRad)
Dim dx = p.X - centre.X, dy = p.Y - centre.Y
Return New Point(CInt(cos*dx - sin*dy + centre.X), CInt(sin*dx + cos*dy + centre.Y))
```

Vérification (test unitaire) : `(10,0)` pivoté de 90° autour de l'origine → `(0,10)`.

---

## Démonstration

La page **Géométrie** dessine, dans un `Panel` via l'événement `Paint` :
le segment `[A,B]` et son milieu, ainsi qu'un point tournant autour d'un centre
selon l'angle choisi au `TrackBar`. La distance au carré `A↔B` est affichée en
continu.

# 07 — Contrôles WinForms (GDI+ / réflexion)

> Mise en forme et manipulation de contrôles WinForms.

Module : `Controles/OutilsControles.vb`. Démonstration : page **Contrôles WinForms**.

---

## 1. Style de grille — `AppliquerStyleGrille`

Met en forme un `DataGridView` (couleurs d'en-tête, lignes alternées, sélection).
`EnableHeadersVisualStyles = False` est nécessaire pour imposer ses propres
couleurs d'en-tête.

## 2. Double buffer **par réflexion** — `ActiverDoubleBuffer`

La propriété `DoubleBuffered` d'un `DataGridView` est **`Protected`** : impossible
de l'affecter directement. On y accède par **réflexion** :

```vbnet
Dim prop = grille.GetType().GetProperty("DoubleBuffered",
              BindingFlags.Instance Or BindingFlags.NonPublic)
prop?.SetValue(grille, True, Nothing)
```

Réduit fortement le scintillement lors du défilement. À manier avec discernement :
on contourne ici volontairement l'encapsulation.

## 3. Boutons à coins arrondis — `ArrondirBouton`

On construit un tracé `GraphicsPath` (arcs + segments) et on l'affecte comme
**`Region`** du bouton : tout ce qui sort du tracé devient transparent aux clics
et au rendu.

```vbnet
Dim chemin As New GraphicsPath()
chemin.AddArc(New Rectangle(0, 0, rayon, rayon), 180, 90)   ' coin haut-gauche
... ' segments + 3 autres arcs
bouton.Region = New Region(chemin)
```

## 4. ComboBox dessinée (owner-draw) — `DessinerElementCombo`

Avec `DrawMode = OwnerDrawFixed`, **vous** dessinez chaque élément
(`DrawItemEventArgs`) : fond sombre, texte clair, surbrillance à la sélection.

```vbnet
combo.DrawMode = DrawMode.OwnerDrawFixed
AddHandler combo.DrawItem, AddressOf OutilsControles.DessinerElementCombo
```

## 5. Positionner une combo sur un ID — `PositionnerComboSurId`

Parcourt les éléments d'une `ComboBox` liée à des données et sélectionne celui dont
`SelectedValue` (le `ValueMember`) correspond à l'ID fourni.

## 6. Parcours récursif des contrôles — `ListerTousLesControles`

Parcourt en profondeur l'arborescence `Controls` (un contrôle, ses enfants, leurs
enfants…). `TrouverControlesEnfants` en est la version « fonction » qui renvoie la
liste. Utile pour appliquer un traitement global (activer/désactiver, traduire…).

```vbnet
For Each enfant As Control In racine.Controls
    controles.Add(enfant)
    ListerTousLesControles(enfant, controles)   ' récursion
Next
```

## 7. Centrer une fenêtre — `CentrerFenetre`

Calcule la position pour centrer un `Form` sur la **zone de travail** de l'écran
principal (`Screen.PrimaryScreen.WorkingArea`, qui exclut la barre des tâches).

---

## Démonstration

La page **Contrôles WinForms** applique le style + double buffer sur une grille,
arrondit un bouton, alimente une ComboBox owner-draw liée à des données, la
positionne sur un ID, compte les contrôles de la page (récursion) et recentre la
fenêtre.

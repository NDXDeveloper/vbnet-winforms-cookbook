# Fiche 02 — Thèmes : appliquer des couleurs par récursion

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Un thème = un jeu de couleurs

```vbnet
Public NotInheritable Class Theme
    Public ReadOnly Property Nom As String
    Public ReadOnly Property Fond As Color
    Public ReadOnly Property Texte As Color
    Public ReadOnly Property Accent As Color
End Class
```

Deux thèmes prédéfinis (clair, sombre) suffisent à illustrer un « mode sombre ».

## Appliquer par récursion

Une fenêtre est un **arbre** de contrôles (`Controls` contient des contrôles, qui
en contiennent d'autres…). Appliquer un thème, c'est **descendre cet arbre** et
colorer chaque contrôle :

```vbnet
Public Shared Sub AppliquerSur(racine As Control, theme As Theme)
    racine.BackColor = theme.Fond
    racine.ForeColor = theme.Texte
    For Each enfant As Control In racine.Controls
        AppliquerSur(enfant, theme)        ' récursion
    Next
End Sub
```

C'est purement de la logique d'arbre : on la **teste** en créant un panneau avec un
enfant et en vérifiant que l'enfant a bien reçu les couleurs.

## Stocker un thème de façon lisible

Pour persister un thème, on convertit ses couleurs en **hexadécimal** (`#RRGGBB`)
via `CouleurHex` — format compact, lisible et portable — puis on le relit à
l'identique :

```vbnet
fond_hex = CouleurHex.VersHex(theme.Fond)        ' "#252526"
theme.Fond = CouleurHex.DepuisHex(fond_hex)      ' aller-retour exact
```

Cette conversion est **pure** : on la teste exhaustivement (format, aller-retour,
entrées invalides) sans interface ni base.

## À retenir

- Un **thème** est un petit jeu de couleurs (fond, texte, accent).
- L'appliquer = **parcourir récursivement** l'arbre des contrôles.
- Stocker les couleurs en **hexadécimal** (conversion pure et testable) rend la
  persistance simple et lisible.

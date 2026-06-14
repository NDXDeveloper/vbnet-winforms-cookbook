# Fiche 01 — Combinaisons de touches et normalisation

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## `Keys` : touche **et** modificateurs en une valeur

WinForms représente une frappe par l'énumération **`Keys`**, qui est un *drapeau* :
elle combine la touche principale et les modificateurs. C'est exactement ce que
fournit `KeyEventArgs.KeyData`.

```vbnet
Keys.Control Or Keys.S      ' = Ctrl+S
Valeur And Keys.KeyCode     ' -> la touche seule (Keys.S)
(Valeur And Keys.Control)   ' -> le modificateur Ctrl est-il présent ?
```

`Combinaison` enveloppe cette valeur et expose `Touche`, `AvecControle`,
`AvecAlt`, `AvecMaj`.

## Analyser une saisie

On découpe sur `+`. Chaque jeton est soit un **modificateur** (`Ctrl`, `Alt`,
`Maj`/`Shift`), soit la **touche** (il ne peut y en avoir qu'une). Des alias
français sont acceptés : `Entrée`, `Échap`, `Espace`, `Suppr`, `Haut`…

```vbnet
Select Case jeton.ToLowerInvariant()
    Case "ctrl", "control", "ctl" : modificateurs = modificateurs Or Keys.Control
    Case "alt"                    : modificateurs = modificateurs Or Keys.Alt
    Case "maj", "shift"           : modificateurs = modificateurs Or Keys.Shift
    Case Else                     : touche = ConvertirTouche(jeton)   ' lettre, chiffre, F5, alias…
End Select
```

Les saisies invalides sont **rejetées** : chaîne vide, `Ctrl+` (touche manquante),
`Ctrl+Maj` (modificateurs seuls), `Ctrl+A+B` (deux touches), touche inconnue.

## Forme canonique

`ToString` produit une forme **unique** quel que soit l'ordre de saisie :
modificateurs dans l'ordre `Ctrl` → `Alt` → `Maj`, puis la touche. Ainsi
`Maj+Ctrl+p` et `ctrl+MAJ+P` donnent tous deux `Ctrl+Maj+P`, et deux combinaisons
équivalentes sont **égales** (utile comme clef de dictionnaire).

## À retenir

- `Keys` encode touche + modificateurs ; `KeyData` en est la source directe.
- L'analyse valide la saisie et accepte des **alias français**.
- La **forme canonique** rend la comparaison et le stockage fiables.

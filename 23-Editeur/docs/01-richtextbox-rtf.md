# Fiche 01 — `RichTextBox`, mise en forme et RTF

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Le contrôle

`RichTextBox` est un éditeur de **texte enrichi** : chaque portion de texte peut
avoir sa propre police, taille, couleur, gras/italique… On agit sur la **sélection
courante** :

| Propriété | Effet |
|---|---|
| `SelectionFont` | police/style de la sélection (`Nothing` si styles mixtes) |
| `SelectionColor` | couleur du texte sélectionné |
| `SelectedText` | le texte sélectionné |

## Basculer un style (gras/italique)

On lit le style courant et on **inverse** le drapeau voulu :

```vbnet
Dim f = _rtb.SelectionFont
If f Is Nothing Then Return                 ' sélection à styles mixtes
Dim nouveau = If((f.Style And FontStyle.Bold) = FontStyle.Bold,
                 f.Style And Not FontStyle.Bold,   ' déjà gras -> on enlève
                 f.Style Or FontStyle.Bold)         ' sinon -> on ajoute
_rtb.SelectionFont = New Font(f, nouveau)
```

`FontStyle` est un **drapeau** (`Bold Or Italic Or Underline`) : on combine et on
teste avec `And`/`Or`/`And Not`.

## Le format RTF

Le contenu enrichi se sérialise dans la propriété **`Rtf`** (Rich Text Format) :
une chaîne autoporteuse qui décrit texte **et** mise en forme. On peut donc :

```vbnet
Dim contenu = _rtb.Rtf          ' enregistrer (en base)
_rtb.Rtf = contenuRecharge       ' recharger
```

> Robustesse : si la chaîne rechargée n'est pas du RTF valide, l'affectation à
> `.Rtf` lève une exception — on retombe alors sur `.Text` (texte brut).

## À retenir

- `RichTextBox` met en forme la **sélection** (`SelectionFont`/`SelectionColor`).
- Basculer un style = manipuler le **drapeau** `FontStyle`.
- **RTF** (`.Rtf`) sérialise texte + mise en forme : idéal à stocker/recharger.

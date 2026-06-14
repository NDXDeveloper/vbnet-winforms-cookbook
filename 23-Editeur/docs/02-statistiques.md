# Fiche 02 — Statistiques de texte : séparer la logique de l'interface

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Le besoin

Afficher en direct le nombre de mots, de caractères et de lignes pendant la frappe.
La tentation serait de coder ce comptage **dans** l'événement `TextChanged` du
contrôle. Mieux vaut l'**extraire** : c'est de la pure logique textuelle.

## Une classe pure

```vbnet
Public Sub New(texte As String)
    Dim t = If(texte, "")
    Caracteres = t.Length
    CaracteresSansEspaces = t.Count(Function(c) Not Char.IsWhiteSpace(c))
    Mots = t.Split(SEPARATEURS, StringSplitOptions.RemoveEmptyEntries).Length
    Lignes = If(t.Length = 0, 0, t.Replace(vbCrLf, vbLf).Split(ChrW(10)).Length)
End Sub
```

- `RemoveEmptyEntries` évite de compter des « mots vides » entre des espaces
  multiples.
- Les lignes : on normalise les fins de ligne (`vbCrLf` → `vbLf`) avant de couper.

## Brancher sur l'interface

L'interface se contente d'appeler la logique et d'afficher le résultat :

```vbnet
Private Sub SurTexteModifie(sender As Object, e As EventArgs)
    _lblStats.Text = StatistiquesTexte.Analyser(_rtb.Text).ToString()
End Sub
```

## L'intérêt : des tests simples

Parce que `StatistiquesTexte` ne dépend ni du contrôle ni de la base, on la teste
exhaustivement (phrase simple, plusieurs lignes, espaces multiples, texte vide) en
quelques lignes — sans ouvrir de fenêtre.

## À retenir

- **Séparer** le calcul (pur) de l'affichage (le contrôle) : code clair et testable.
- Attention aux **mots vides** (`RemoveEmptyEntries`) et aux **fins de ligne** mixtes.
- L'UI ne fait qu'appeler la logique et afficher.

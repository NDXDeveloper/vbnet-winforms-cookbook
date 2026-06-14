# Fiche 02 — Pagination : séparer le calcul du rendu

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Pourquoi isoler la pagination

Le rendu d'impression a besoin d'un objet `Graphics` (donc d'une imprimante ou d'un
aperçu) : difficile à tester. Mais la **logique** de pagination — « combien de
lignes par page, combien de pages, quelles lignes pour la page N » — n'est que de
l'arithmétique. On l'extrait dans `Paginateur`, **pur** et testable.

```vbnet
' Combien de lignes tiennent dans la hauteur utile ?
Public Shared Function LignesParPage(hauteurDisponible As Double, hauteurLigne As Double) As Integer
    Return Math.Max(1, CInt(Math.Floor(hauteurDisponible / hauteurLigne)))
End Function

' Combien de pages au total ?
Public Shared Function NombrePages(nbLignes As Integer, lignesParPage As Integer) As Integer
    Return CInt(Math.Ceiling(nbLignes / CDbl(lignesParPage)))
End Function
```

## Découper la bonne tranche

Pour afficher/imprimer la page *N*, on prend la tranche correspondante :

```vbnet
Public Shared Function LignesDeLaPage(lignes As IList(Of String), page As Integer, lignesParPage As Integer) As List(Of String)
    Dim debut = (page - 1) * lignesParPage
    If page < 1 OrElse debut >= lignes.Count Then Return New List(Of String)()
    Return lignes.Skip(debut).Take(lignesParPage).ToList()
End Function
```

## Tester sans imprimante

Comme tout est numérique, les tests couvrent les cas limites **hors-ligne** :
0 ligne → 0 page ; 10 lignes / 10 par page → 1 page ; 11 → 2 pages ; page hors
limites → tranche vide ; hauteur de ligne nulle → au moins 1 ligne par page.

> Dans le rendu réel, `ImprimeurTexte` calcule `hauteurLigne` via
> `police.GetHeight(e.Graphics)` et s'arrête à `MarginBounds.Bottom` — mais la
> *règle* qu'il applique est exactement celle, testée, du `Paginateur`.

## À retenir

- **Séparer** le calcul (pur, testable) du rendu (dépendant de `Graphics`).
- Trois primitives suffisent : lignes/page, nombre de pages, lignes d'une page.
- On teste tous les cas limites **sans imprimante**.

# Fiche 01 — Une version *comparable* : `IComparable` + opérateurs

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Le piège du tri alphabétique

Un numéro de version est *du texte*, mais le **comparer comme du texte** donne un
ordre faux :

| Comparaison texte | Comparaison sémantique |
|---|---|
| `"1.10.0" < "1.2.0"` (car `'1' < '2'`) | `1.10.0 > 1.2.0` (car 10 > 2) |

Il faut comparer **composante par composante, numériquement** : d'abord la majeure,
puis la mineure, puis la corrective.

## Rendre le type comparable

`VersionSemantique` implémente `IComparable(Of VersionSemantique)`. Une seule
méthode, `CompareTo`, suffit à débloquer `List.Sort`, `OrderBy`, `Max`, etc. :

```vbnet
Public Function CompareTo(autre As VersionSemantique) As Integer _
        Implements IComparable(Of VersionSemantique).CompareTo
    If Majeure <> autre.Majeure Then Return Majeure.CompareTo(autre.Majeure)
    If Mineure <> autre.Mineure Then Return Mineure.CompareTo(autre.Mineure)
    Return Corrective.CompareTo(autre.Corrective)
End Function
```

## Surcharger les opérateurs

Pour écrire `siA > siB` plutôt que `a.CompareTo(b) > 0`, on **surcharge les
opérateurs** — tous délégués à `CompareTo` pour rester cohérents :

```vbnet
Public Shared Operator >(a As VersionSemantique, b As VersionSemantique) As Boolean
    Return a.CompareTo(b) > 0
End Operator
```

> Règle : si l'on définit `=`, il faut aussi `<>`, et redéfinir `Equals` /
> `GetHashCode` pour que l'égalité reste cohérente partout (dictionnaires, `Distinct`…).

## Le motif `TryAnalyser`

Analyser une saisie utilisateur **ne doit pas** lever d'exception sur une faute de
frappe. On expose donc deux variantes :

- `Analyser(texte)` : pratique, **lève** `FormatException` si invalide ;
- `TryAnalyser(texte, ByRef resultat)` : renvoie **`Boolean`**, sans exception.

C'est exactement le contrat de `Integer.Parse` / `Integer.TryParse`. La saisie
tolère `1`, `1.2`, `1.2.3` et un `v` initial.

## À retenir

- Comparer des versions **numériquement**, pas alphabétiquement.
- `IComparable` = une méthode (`CompareTo`) qui débloque tri et requêtes LINQ.
- Surcharger les opérateurs **en les déléguant** à `CompareTo`.
- Offrir un `TryAnalyser` pour les saisies faillibles.

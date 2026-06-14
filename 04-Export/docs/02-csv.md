# 02 — Export CSV

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Le CSV (valeurs séparées) est le format d'échange le plus universel — mais
trompeusement simple : l'**échappement** est la partie qui compte.

## Options (`ExportateurCsv`)

| Propriété | Défaut | Rôle |
|---|---|---|
| `Separateur` | `;` | séparateur de champs (Excel FR attend `;`) |
| `AvecEntetes` | `True` | écrire la ligne des noms de colonnes |
| `AvecBom` | `True` | préfixer d'un BOM UTF-8 (Excel détecte l'encodage) |

## Échappement (RFC 4180)

Un champ est entouré de guillemets s'il contient le séparateur, un guillemet, ou
un saut de ligne ; les guillemets internes sont **doublés** :

```
Nom;Quantite;Prix
"Vis; speciale";200;1.5
"Ecrou ""M8""";50;0.2
```

```vbnet
Private Function Echapper(valeur As String) As String
    Dim doitEntourer = valeur.Contains(Separateur) OrElse valeur.Contains("""") _
                       OrElse valeur.Contains(vbCr) OrElse valeur.Contains(vbLf)
    If Not doitEntourer Then Return valeur
    Return """" & valeur.Replace("""", """""") & """"
End Function
```

## Encodage

Les valeurs sont rendues en **culture invariante** (séparateur décimal `.`,
dates ISO) pour un fichier stable, indépendant des réglages régionaux. Le BOM
UTF-8 aide Excel à ouvrir correctement les caractères accentués.

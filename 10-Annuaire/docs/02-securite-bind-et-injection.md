# Fiche 02 — Injection LDAP et échappement des filtres

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Le danger : concaténer une saisie dans un filtre

Un filtre LDAP est une expression comme `(uid=alice)`. Si l'on **concatène**
directement une saisie utilisateur, un attaquant peut détourner le filtre — c'est
l'**injection LDAP**, cousine de l'injection SQL.

Exemple : champ « identifiant » contenant `*)(uid=*`

```
Filtre construit naïvement : (uid=*)(uid=*))   ← ouvre/ferme des sous-filtres
Effet : correspond à TOUS les comptes (contournement du ciblage).
```

## La parade : échapper selon la RFC 4515

Avant d'insérer une valeur dans un filtre, on remplace les caractères spéciaux par
`\` suivi de leur code hexadécimal :

| Caractère | Échappé |
|:---:|:---:|
| `\` | `\5c` |
| `*` | `\2a` |
| `(` | `\28` |
| `)` | `\29` |
| NUL | `\00` |

```vbnet
Public Shared Function EchapperFiltre(valeur As String) As String
    For Each c As Char In valeur
        Select Case c
            Case "\"c : sb.Append("\5c")
            Case "*"c : sb.Append("\2a")
            Case "("c : sb.Append("\28")
            Case ")"c : sb.Append("\29")
            Case ChrW(0) : sb.Append("\00")
            Case Else : sb.Append(c)
        End Select
    Next
End Function
```

Ainsi `*)(uid=*` devient `\2a\29\28uid=\2a` : une simple **chaîne littérale**, sans
pouvoir syntaxique. C'est une logique **pure**, donc testée sans annuaire (mots
ordinaires inchangés, caractères spéciaux échappés, tentative d'injection
neutralisée).

## Autres bonnes pratiques (rappel)

- **Bind** pour authentifier : ne jamais comparer soi-même un mot de passe lu.
- **Moindre privilège** : lire sa propre entrée en « self » ; réserver le bind
  administrateur aux opérations qui l'exigent (lister tous les comptes).
- **TLS** en production (LDAPS / StartTLS) : ici, LDAP est en clair car il s'agit
  d'une démonstration locale.

## À retenir

- Ne **jamais** concaténer une saisie dans un filtre LDAP.
- Échapper `\ * ( )` (RFC 4515) neutralise l'injection.
- L'échappement est une fonction **pure** : facile à tester exhaustivement.

# Fiche 02 — Raccourcis Internet `.url` (format INI)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Un `.url` est un simple fichier texte

Contrairement au `.lnk`, un raccourci Internet est un **fichier INI** trivial :

```ini
[InternetShortcut]
URL=https://exemple.test/page
```

On peut donc le **générer** et l'**analyser** entièrement en VB.NET, sans COM ni
shell — ce qui le rend parfaitement **testable**.

## Générer

```vbnet
Public Shared Function GenererContenu(url As String) As String
    Dim sb As New StringBuilder()
    sb.Append("[InternetShortcut]").Append(vbCrLf)
    sb.Append("URL=").Append(url.Trim()).Append(vbCrLf)
    Return sb.ToString()
End Function
```

## Analyser

On lit ligne à ligne et on cherche la clé `URL` (insensible à la casse), en
ignorant les autres clés (`IconIndex`, etc.) :

```vbnet
For Each ligne In contenu.Replace(vbCrLf, vbLf).Split(vbLf(0))
    Dim p = ligne.IndexOf("="c)
    If p > 0 AndAlso ligne.Substring(0, p).Trim().Equals("URL", StringComparison.OrdinalIgnoreCase) Then
        Return ligne.Substring(p + 1).Trim()
    End If
Next
Throw New FormatException("Aucune URL trouvée.")
```

## Pourquoi c'est intéressant pédagogiquement

Le même besoin — « créer un raccourci » — se résout de **deux manières très
différentes** selon la cible :

| | `.lnk` (application) | `.url` (Web) |
|---|---|---|
| Format | binaire (shell COM) | texte (INI) |
| Création | `WScript.Shell` | écriture de fichier |
| Testable hors-OS | non (COM/STA) | **oui** (texte pur) |

C'est un bon rappel : **choisir la technique selon le format**, et **isoler ce qui
est pur** (le `.url`) pour le tester facilement.

## À retenir

- Un `.url` = `[InternetShortcut]` + `URL=…` ; lecture/écriture en **texte pur**.
- L'analyse ignore les autres clés et tolère la casse.
- Pur ⇒ **testable** sans disque ni shell, contrairement au `.lnk`.

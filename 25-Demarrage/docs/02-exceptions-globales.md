# Fiche 02 — Capturer les exceptions globalement

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Pourquoi

Une exception imprévue ne devrait pas **fermer brutalement** l'application devant
l'utilisateur. Une application robuste l'**intercepte** au niveau global, la
**journalise** (pour le diagnostic) et **informe** poliment.

## Les deux sources d'exceptions

| Source | Événement |
|---|---|
| Fil d'**interface** (gestionnaires d'événements WinForms) | `Application.ThreadException` |
| **Autres fils** / domaine d'application | `AppDomain.CurrentDomain.UnhandledException` |

On s'abonne aux deux :

```vbnet
Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
AddHandler Application.ThreadException, Sub(s, e) surException(e.Exception)
AddHandler AppDomain.CurrentDomain.UnhandledException,
    Sub(s, e)
        Dim ex = TryCast(e.ExceptionObject, Exception)
        If ex IsNot Nothing Then surException(ex)
    End Sub
```

## Le piège : l'ordre d'installation

`SetUnhandledExceptionMode` doit être appelé **avant** la création de la première
fenêtre. On installe donc la capture **tout au début** de `Program.Main`, avant le
splash et avant `Application.Run` :

```vbnet
Application.EnableVisualStyles()
GestionnaireExceptions.Installer(AddressOf SurExceptionGlobale)   ' AVANT toute fenêtre
... splash ...
Application.Run(New FrmPrincipale())
```

`SurExceptionGlobale` journalise (`DepotEvenement`) puis affiche un message — sans
laisser l'application planter.

> En débogage, Visual Studio s'arrête quand même sur l'exception ; c'est en
> exécution normale (hors débogueur) que la capture prend tout son sens.

## À retenir

- Deux sources : **`Application.ThreadException`** (UI) et
  **`AppDomain.UnhandledException`** (autres fils).
- **Installer tôt** (`SetUnhandledExceptionMode` exige « avant toute fenêtre »).
- Réagir = **journaliser + informer**, pas planter.

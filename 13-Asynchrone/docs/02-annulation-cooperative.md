# Fiche 02 — Annulation coopérative (`CancellationToken`)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## On n'« arrête » pas un thread de force

Tuer un thread en plein travail laisse l'application dans un état incohérent (fichiers
à moitié écrits, verrous non relâchés…). .NET retient donc l'**annulation
coopérative** : le code vérifie *lui-même*, à des points sûrs, si l'on a demandé
l'arrêt.

## Le trio source / jeton / vérification

```vbnet
' Côté appelant : la source produit un jeton et déclenche l'annulation.
_cts = New CancellationTokenSource()
Await MoteurTaches.ExecuterAsync(..., jeton:=_cts.Token)
...
Private Sub SurAnnuler(...) 
    _cts.Cancel()          ' demande l'arrêt
End Sub
```

```vbnet
' Côté travail : on vérifie le jeton à chaque itération.
For Each element In liste
    jeton.ThrowIfCancellationRequested()   ' lève OperationCanceledException
    traiter(element)
Next
```

`ThrowIfCancellationRequested` lève une `OperationCanceledException` que l'appelant
intercepte pour réagir proprement :

```vbnet
Try
    Await MoteurTaches.ExecuterAsync(...)
Catch ex As OperationCanceledException
    _lbl.Text = "Traitement annulé."
Finally
    _cts.Dispose()
End Try
```

## Pourquoi passer aussi le jeton à `Task.Run`

En donnant le jeton à `Task.Run(work, jeton)`, une annulation **avant** le démarrage
fait passer la tâche directement à l'état *Canceled* (sans exécuter le travail) — ce
qui est cohérent et économe.

## À retenir

- L'annulation est **coopérative** : on ne tue pas un thread, on lui **demande** de
  s'arrêter.
- `CancellationTokenSource` (côté appelant) → `CancellationToken` (côté travail).
- `ThrowIfCancellationRequested` aux points sûrs ; on capte
  `OperationCanceledException`.
- Toujours **libérer** la source (`Dispose`) une fois terminé.

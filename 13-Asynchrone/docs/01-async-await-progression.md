# Fiche 01 — `Async/Await`, `Task.Run` et progression

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Le problème : l'interface qui « gèle »

Si l'on exécute un traitement long **directement** dans un gestionnaire d'événement
(sur le fil de l'UI), la fenêtre ne répond plus : elle ne se redessine pas, ne
réagit pas aux clics. Il faut donc déporter le travail **hors du fil UI**.

## `Task.Run` + `Await`

`Task.Run` exécute le travail sur un thread du pool ; `Await` libère le fil UI le
temps du traitement, puis y revient une fois terminé :

```vbnet
Public Shared Async Function ExecuterAsync(Of T)(elements As IEnumerable(Of T),
        traiter As Action(Of T), progression As IProgress(Of Avancement),
        jeton As CancellationToken) As Task(Of Integer)
    Await Task.Run(
        Sub()
            For Each element In liste
                jeton.ThrowIfCancellationRequested()
                traiter(element)
                progression?.Report(New Avancement(faits, total, "…"))
            Next
        End Sub, jeton)
    Return faits
End Function
```

Côté interface, le gestionnaire devient **`Async Sub`** et `Await`-e le moteur :

```vbnet
Private Async Sub SurDemarrer(sender As Object, e As EventArgs)
    _btnDemarrer.Enabled = False
    Await MoteurTaches.ExecuterAsync(...)   ' la fenêtre reste réactive
    _btnDemarrer.Enabled = True
End Sub
```

## Le piège : mettre à jour l'UI depuis un autre fil

Un contrôle WinForms ne doit être touché que **depuis le fil UI**. Or `Report` est
appelé depuis le thread d'arrière-plan. La solution : **`Progress(Of T)`**, qui
capture le `SynchronizationContext` de l'UI **au moment de sa création** et
réexécute la callback sur ce fil :

```vbnet
Dim progression As New Progress(Of Avancement)(
    Sub(a)
        _barre.Value = a.Pourcentage   ' exécuté sur le fil UI, sans Invoke manuel
        _lbl.Text = a.Message
    End Sub)
```

> Conséquence pour les tests : un `Progress(Of T)` poste les callbacks de façon
> asynchrone. Pour des tests **déterministes**, on injecte un `IProgress(Of T)`
> maison qui enregistre les rapports **de façon synchrone**.

## À retenir

- Travail long ⇒ `Task.Run` + `Await` : l'UI ne gèle pas.
- Le gestionnaire devient `Async Sub` ; le reste du code l'`Await`-e via `Task`.
- **`Progress(Of T)`** ramène la progression sur le fil UI — pas de `Control.Invoke`
  manuel.

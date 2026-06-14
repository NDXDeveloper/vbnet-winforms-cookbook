' ============================================================================
'  MoteurTaches.vb  -  Execution asynchrone avec progression et annulation.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks

''' <summary>
''' Exécute un traitement sur une collection <b>en arrière-plan</b> (sans bloquer
''' l'interface), en rapportant la progression via <see cref="IProgress(Of T)"/>
''' et en honorant un <see cref="CancellationToken"/>. Illustre <c>Async/Await</c>,
''' <c>Task.Run</c>, la progression et l'annulation coopérative.
''' </summary>
Public NotInheritable Class MoteurTaches

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Applique <paramref name="traiter"/> à chaque élément, sur un thread d'arrière-plan.
    ''' Renvoie le nombre d'éléments traités. Lève <see cref="OperationCanceledException"/>
    ''' si l'annulation est demandée.
    ''' </summary>
    Public Shared Async Function ExecuterAsync(Of T)(
            ByVal elements As IEnumerable(Of T),
            ByVal traiter As Action(Of T),
            Optional ByVal progression As IProgress(Of Avancement) = Nothing,
            Optional ByVal jeton As CancellationToken = Nothing,
            Optional ByVal delaiMsParElement As Integer = 0) As Task(Of Integer)

        If elements Is Nothing Then Throw New ArgumentNullException(NameOf(elements))
        If traiter Is Nothing Then Throw New ArgumentNullException(NameOf(traiter))

        Dim liste As List(Of T) = elements.ToList()
        Dim total As Integer = liste.Count
        Dim faits As Integer = 0

        Await Task.Run(
            Sub()
                For Each element As T In liste
                    jeton.ThrowIfCancellationRequested()
                    traiter(element)
                    If delaiMsParElement > 0 Then Thread.Sleep(delaiMsParElement)
                    faits += 1
                    If progression IsNot Nothing Then
                        progression.Report(New Avancement(faits, total, "Traité : " & faits.ToString() & " / " & total.ToString()))
                    End If
                Next
            End Sub, jeton)

        Return faits
    End Function

End Class

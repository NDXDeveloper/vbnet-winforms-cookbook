' ============================================================================
'  MoteurTachesTests.vb  -  Tests du moteur asynchrone (progression, annulation).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Asynchrone

<TestClass>
Public Class MoteurTachesTests

    ''' <summary>IProgress de test : enregistre les rapports de façon synchrone (déterministe).</summary>
    Private NotInheritable Class CollecteurAvancement
        Implements IProgress(Of Avancement)
        Public ReadOnly Rapports As New List(Of Avancement)()
        Public Sub Report(ByVal value As Avancement) Implements IProgress(Of Avancement).Report
            SyncLock Rapports
                Rapports.Add(value)
            End SyncLock
        End Sub
    End Class

    <TestMethod>
    Public Async Function ExecuterAsync_TraiteTousLesElements() As Task
        Dim somme As Integer = 0
        Dim faits As Integer = Await MoteurTaches.ExecuterAsync({1, 2, 3, 4, 5}, Sub(x) somme += x)
        Assert.AreEqual(5, faits)
        Assert.AreEqual(15, somme)
    End Function

    <TestMethod>
    Public Async Function ExecuterAsync_RapporteLaProgression() As Task
        Dim collecteur As New CollecteurAvancement()
        Await MoteurTaches.ExecuterAsync(Enumerable.Range(1, 4),
                                         Sub(x)
                                         End Sub, collecteur)
        Assert.AreEqual(4, collecteur.Rapports.Count)
        Assert.AreEqual(100, collecteur.Rapports.Last().Pourcentage)
        Assert.AreEqual(25, collecteur.Rapports.First().Pourcentage)
    End Function

    <TestMethod>
    Public Async Function ExecuterAsync_Annulation_Leve() As Task
        ' TaskCanceledException dérive d'OperationCanceledException : on capte la base
        ' pour rester robuste quel que soit le type concret levé.
        Dim cts As New CancellationTokenSource()
        cts.Cancel()
        Dim leve As Boolean = False
        Try
            Await MoteurTaches.ExecuterAsync(Enumerable.Range(1, 3),
                                             Sub(x)
                                             End Sub, Nothing, cts.Token)
        Catch ex As OperationCanceledException
            leve = True
        End Try
        Assert.IsTrue(leve, "Une OperationCanceledException (ou dérivée) doit être levée.")
    End Function

    <TestMethod>
    Public Async Function ExecuterAsync_ListeVide_RendZero() As Task
        Dim faits As Integer = Await MoteurTaches.ExecuterAsync(New Integer() {},
                                                                Sub(x)
                                                                End Sub)
        Assert.AreEqual(0, faits)
    End Function

End Class

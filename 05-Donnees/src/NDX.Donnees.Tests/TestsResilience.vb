' ============================================================================
'  TestsResilience.vb  -  Tests unitaires de la reexecution (sans base).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Donnees

<TestClass>
Public Class TestsResilience

    <TestMethod>
    Public Sub Reessaie_les_fautes_transitoires_puis_reussit()
        Dim appels As Integer = 0
        Dim resultat As Integer = Resilience.Executer(Of Integer)(
            Function()
                appels += 1
                If appels < 3 Then Throw New Exception("faute transitoire simulee")
                Return appels
            End Function,
            Function(ex) True)   ' tout est considere transitoire
        Assert.AreEqual(3, appels, "Doit reessayer jusqu'a la 3e tentative.")
        Assert.AreEqual(3, resultat)
    End Sub

    <TestMethod>
    Public Sub Ne_reessaie_pas_une_faute_non_transitoire()
        Dim appels As Integer = 0
        Assert.ThrowsException(Of InvalidOperationException)(
            Sub()
                Resilience.Executer(Of Integer)(
                    Function()
                        appels += 1
                        Throw New InvalidOperationException("definitif")
                    End Function,
                    Function(ex) False)   ' jamais transitoire
            End Sub)
        Assert.AreEqual(1, appels, "Aucune nouvelle tentative attendue.")
    End Sub

End Class

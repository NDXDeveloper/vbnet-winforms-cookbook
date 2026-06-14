' ============================================================================
'  CalculAvancementTests.vb  -  Tests du calcul d'avancement (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Asynchrone

<TestClass>
Public Class CalculAvancementTests

    <DataTestMethod>
    <DataRow(0, 100, 0)>
    <DataRow(50, 100, 50)>
    <DataRow(100, 100, 100)>
    <DataRow(1, 3, 33)>
    <DataRow(2, 3, 67)>
    Public Sub Pourcentage_CasNominaux(ByVal fait As Integer, ByVal total As Integer, ByVal attendu As Integer)
        Assert.AreEqual(attendu, CalculAvancement.Pourcentage(fait, total))
    End Sub

    <TestMethod>
    Public Sub Pourcentage_TotalNul_RendZero()
        Assert.AreEqual(0, CalculAvancement.Pourcentage(5, 0))
    End Sub

    <TestMethod>
    Public Sub Pourcentage_EstBorne()
        Assert.AreEqual(100, CalculAvancement.Pourcentage(200, 100))
        Assert.AreEqual(0, CalculAvancement.Pourcentage(-5, 100))
    End Sub

End Class

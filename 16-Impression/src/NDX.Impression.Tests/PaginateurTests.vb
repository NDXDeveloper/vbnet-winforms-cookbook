' ============================================================================
'  PaginateurTests.vb  -  Tests de la pagination (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Impression

<TestClass>
Public Class PaginateurTests

    <TestMethod>
    Public Sub LignesParPage_CalculEtBornes()
        Assert.AreEqual(5, Paginateur.LignesParPage(100, 20))
        Assert.AreEqual(1, Paginateur.LignesParPage(10, 20))   ' au moins une
        Assert.AreEqual(1, Paginateur.LignesParPage(100, 0))   ' hauteur de ligne nulle
    End Sub

    <DataTestMethod>
    <DataRow(0, 10, 0)>
    <DataRow(10, 10, 1)>
    <DataRow(11, 10, 2)>
    <DataRow(25, 10, 3)>
    Public Sub NombrePages_CasNominaux(ByVal nbLignes As Integer, ByVal parPage As Integer, ByVal attendu As Integer)
        Assert.AreEqual(attendu, Paginateur.NombrePages(nbLignes, parPage))
    End Sub

    <TestMethod>
    Public Sub NombrePages_ParPageInvalide_Leve()
        Assert.ThrowsException(Of ArgumentOutOfRangeException)(Function() Paginateur.NombrePages(10, 0))
    End Sub

    <TestMethod>
    Public Sub LignesDeLaPage_DecoupeCorrectement()
        Dim lignes As List(Of String) = Enumerable.Range(1, 25).Select(Function(i) "L" & i.ToString()).ToList()
        Assert.AreEqual(10, Paginateur.LignesDeLaPage(lignes, 1, 10).Count)
        Assert.AreEqual("L1", Paginateur.LignesDeLaPage(lignes, 1, 10).First())
        Dim page3 = Paginateur.LignesDeLaPage(lignes, 3, 10)
        Assert.AreEqual(5, page3.Count)
        Assert.AreEqual("L25", page3.Last())
    End Sub

    <TestMethod>
    Public Sub LignesDeLaPage_HorsLimites_RendVide()
        Dim lignes As List(Of String) = Enumerable.Range(1, 25).Select(Function(i) "L" & i.ToString()).ToList()
        Assert.AreEqual(0, Paginateur.LignesDeLaPage(lignes, 4, 10).Count)
        Assert.AreEqual(0, Paginateur.LignesDeLaPage(lignes, 0, 10).Count)
    End Sub

End Class

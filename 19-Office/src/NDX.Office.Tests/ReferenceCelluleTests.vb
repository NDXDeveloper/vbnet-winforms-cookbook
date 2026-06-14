' ============================================================================
'  ReferenceCelluleTests.vb  -  Tests de la conversion A1 <-> (col, ligne) (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Office

<TestClass>
Public Class ReferenceCelluleTests

    <DataTestMethod>
    <DataRow("A", 1)>
    <DataRow("Z", 26)>
    <DataRow("AA", 27)>
    <DataRow("AB", 28)>
    <DataRow("BA", 53)>
    Public Sub LettresVersColonne_EstCorrect(ByVal lettres As String, ByVal attendu As Integer)
        Assert.AreEqual(attendu, ReferenceCellule.LettresVersColonne(lettres))
    End Sub

    <DataTestMethod>
    <DataRow(1, "A")>
    <DataRow(26, "Z")>
    <DataRow(27, "AA")>
    <DataRow(28, "AB")>
    Public Sub ColonneVersLettres_EstCorrect(ByVal index As Integer, ByVal attendu As String)
        Assert.AreEqual(attendu, ReferenceCellule.ColonneVersLettres(index))
    End Sub

    <TestMethod>
    Public Sub Analyser_DecomposeLaReference()
        Dim r = ReferenceCellule.Analyser("C10")
        Assert.AreEqual(3, r.Colonne)
        Assert.AreEqual(10, r.Ligne)
        Assert.AreEqual("C10", r.A1)
    End Sub

    <TestMethod>
    Public Sub AllerRetour_ColonneEtLettres()
        For i As Integer = 1 To 100
            Assert.AreEqual(i, ReferenceCellule.LettresVersColonne(ReferenceCellule.ColonneVersLettres(i)))
        Next
    End Sub

    <DataTestMethod>
    <DataRow("10")>
    <DataRow("C")>
    <DataRow("")>
    Public Sub Analyser_Invalide_Leve(ByVal refA1 As String)
        Assert.ThrowsException(Of FormatException)(Function() ReferenceCellule.Analyser(refA1))
    End Sub

End Class

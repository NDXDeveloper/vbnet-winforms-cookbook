' ============================================================================
'  VersionSemantiqueTests.vb  -  Tests du type VersionSemantique.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.MiseAJour

<TestClass>
Public Class VersionSemantiqueTests

    <TestMethod>
    Public Sub Analyser_FormeComplete_RenseigneLesComposantes()
        Dim v = VersionSemantique.Analyser("2.5.9")
        Assert.AreEqual(2, v.Majeure)
        Assert.AreEqual(5, v.Mineure)
        Assert.AreEqual(9, v.Corrective)
    End Sub

    <DataTestMethod>
    <DataRow("1", 1, 0, 0)>
    <DataRow("1.4", 1, 4, 0)>
    <DataRow("v3.2.1", 3, 2, 1)>
    <DataRow("  10.0.0  ", 10, 0, 0)>
    Public Sub Analyser_FormesTolerees(ByVal texte As String, ByVal maj As Integer, ByVal min As Integer, ByVal corr As Integer)
        Dim v = VersionSemantique.Analyser(texte)
        Assert.AreEqual(New VersionSemantique(maj, min, corr), v)
    End Sub

    <DataTestMethod>
    <DataRow("")>
    <DataRow("abc")>
    <DataRow("1.2.3.4")>
    <DataRow("1.-2.0")>
    <DataRow("1..3")>
    Public Sub TryAnalyser_FormesInvalides_RenvoieFalse(ByVal texte As String)
        Dim v As VersionSemantique
        Assert.IsFalse(VersionSemantique.TryAnalyser(texte, v))
    End Sub

    <TestMethod>
    Public Sub Comparaison_EstNumerique_PasAlphabetique()
        ' Piège classique : en texte, "1.10.0" < "1.2.0". En sémantique, c'est l'inverse.
        Assert.IsTrue(VersionSemantique.Analyser("1.10.0") > VersionSemantique.Analyser("1.2.0"))
    End Sub

    <TestMethod>
    Public Sub Operateurs_SontCoherents()
        Dim a = VersionSemantique.Analyser("1.2.0")
        Dim b = VersionSemantique.Analyser("1.2.3")
        Assert.IsTrue(a < b)
        Assert.IsTrue(b > a)
        Assert.IsTrue(a <= VersionSemantique.Analyser("1.2.0"))
        Assert.IsTrue(a >= VersionSemantique.Analyser("1.2.0"))
        Assert.IsTrue(a = VersionSemantique.Analyser("1.2.0"))
        Assert.IsTrue(a <> b)
    End Sub

    <TestMethod>
    Public Sub Tri_OrdonneCorrectement()
        Dim liste As New List(Of VersionSemantique) From {
            VersionSemantique.Analyser("2.0.0"),
            VersionSemantique.Analyser("1.10.0"),
            VersionSemantique.Analyser("1.2.0"),
            VersionSemantique.Analyser("1.2.10")}
        liste.Sort()
        Assert.AreEqual("1.2.0", liste(0).ToString())
        Assert.AreEqual("1.2.10", liste(1).ToString())
        Assert.AreEqual("1.10.0", liste(2).ToString())
        Assert.AreEqual("2.0.0", liste(3).ToString())
    End Sub

    <TestMethod>
    Public Sub Egalite_ImpliqueMemeHashCode()
        Dim a = VersionSemantique.Analyser("3.4.5")
        Dim b = New VersionSemantique(3, 4, 5)
        Assert.IsTrue(a.Equals(b))
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode())
    End Sub

    <TestMethod>
    Public Sub Constructeur_Negatif_Leve()
        Assert.ThrowsException(Of ArgumentException)(Function() New VersionSemantique(1, -1, 0))
    End Sub

End Class

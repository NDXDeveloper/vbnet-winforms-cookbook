' ============================================================================
'  CouleurHexTests.vb  -  Tests de la conversion Color <-> hexadecimal (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.FormBase

<TestClass>
Public Class CouleurHexTests

    <TestMethod>
    Public Sub VersHex_FormatAttendu()
        Assert.AreEqual("#0080FF", CouleurHex.VersHex(Color.FromArgb(0, 128, 255)))
        Assert.AreEqual("#000000", CouleurHex.VersHex(Color.FromArgb(0, 0, 0)))
        Assert.AreEqual("#FFFFFF", CouleurHex.VersHex(Color.FromArgb(255, 255, 255)))
    End Sub

    <TestMethod>
    Public Sub DepuisHex_DecomposeLesComposantes()
        Dim c = CouleurHex.DepuisHex("#0080FF")
        Assert.AreEqual(0, CInt(c.R))
        Assert.AreEqual(128, CInt(c.G))
        Assert.AreEqual(255, CInt(c.B))
    End Sub

    <TestMethod>
    Public Sub DepuisHex_ToleSansDiese()
        Assert.AreEqual(CouleurHex.DepuisHex("#1A2B3C").ToArgb(), CouleurHex.DepuisHex("1A2B3C").ToArgb())
    End Sub

    <TestMethod>
    Public Sub AllerRetour_ConserveLaCouleur()
        For Each c As Color In New Color() {Color.Red, Color.FromArgb(12, 34, 56), Color.Gainsboro, Color.FromArgb(124, 77, 255)}
            Dim restituee = CouleurHex.DepuisHex(CouleurHex.VersHex(c))
            Assert.AreEqual(c.R, restituee.R)
            Assert.AreEqual(c.G, restituee.G)
            Assert.AreEqual(c.B, restituee.B)
        Next
    End Sub

    <DataTestMethod>
    <DataRow("#12")>
    <DataRow("#GGGGGG")>
    <DataRow("")>
    Public Sub DepuisHex_Invalide_Leve(ByVal hex As String)
        Assert.ThrowsException(Of FormatException)(Function() CouleurHex.DepuisHex(hex))
    End Sub

End Class

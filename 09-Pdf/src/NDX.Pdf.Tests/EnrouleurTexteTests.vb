' ============================================================================
'  EnrouleurTexteTests.vb  -  Tests du retour a la ligne (sans rien dessiner).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Pdf

<TestClass>
Public Class EnrouleurTexteTests

    <TestMethod>
    Public Sub CaracteresParLigne_EstExact_PourCourier()
        ' 60 points / (0,6 * 10) = 60 / 6 = 10 caractères.
        Assert.AreEqual(10, EnrouleurTexte.CaracteresParLigne(60, 10))
    End Sub

    <TestMethod>
    Public Sub CaracteresParLigne_AuMoinsUn()
        Assert.AreEqual(1, EnrouleurTexte.CaracteresParLigne(1, 100))
    End Sub

    <TestMethod>
    Public Sub Enrouler_CoupeAuxMots()
        Dim lignes = EnrouleurTexte.Enrouler("hello world", 60, 10) ' maxi = 10
        Assert.AreEqual(2, lignes.Count)
        Assert.AreEqual("hello", lignes(0))
        Assert.AreEqual("world", lignes(1))
    End Sub

    <TestMethod>
    Public Sub Enrouler_NeDepassePasLaLargeur()
        Dim maxi As Integer = EnrouleurTexte.CaracteresParLigne(120, 10)
        Dim texte As String = "Cette phrase de démonstration doit être coupée en plusieurs lignes successives."
        For Each ligne As String In EnrouleurTexte.Enrouler(texte, 120, 10)
            Assert.IsTrue(ligne.Length <= maxi, "Ligne trop longue : « " & ligne & " »")
        Next
    End Sub

    <TestMethod>
    Public Sub Enrouler_CoupeLesMotsTropLongs()
        Dim lignes = EnrouleurTexte.Enrouler("abcdefghijklmnop", 60, 10) ' maxi = 10
        Assert.AreEqual(2, lignes.Count)
        Assert.AreEqual("abcdefghij", lignes(0))
        Assert.AreEqual("klmnop", lignes(1))
    End Sub

    <TestMethod>
    Public Sub Enrouler_RespecteLesSautsDeLigne()
        Dim lignes = EnrouleurTexte.Enrouler("a" & vbLf & "b", 200, 10)
        Assert.AreEqual(2, lignes.Count)
        Assert.AreEqual("a", lignes(0))
        Assert.AreEqual("b", lignes(1))
    End Sub

    <TestMethod>
    Public Sub Enrouler_TexteVide_RendListeVide()
        Assert.AreEqual(0, EnrouleurTexte.Enrouler("", 100, 10).Count)
    End Sub

End Class

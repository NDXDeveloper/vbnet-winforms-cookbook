' ============================================================================
'  EchelleGraphiqueTests.vb  -  Tests de la mise a l'echelle (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Graphiques

<TestClass>
Public Class EchelleGraphiqueTests

    <TestMethod>
    Public Sub VersY_BasEnBas_HautEnHaut()
        Dim e As New EchelleGraphique(0, 100, 200)
        Assert.AreEqual(200.0, e.VersY(0), 0.0001, "La plus petite valeur est en bas.")
        Assert.AreEqual(0.0, e.VersY(100), 0.0001, "La plus grande valeur est en haut.")
        Assert.AreEqual(100.0, e.VersY(50), 0.0001, "La valeur médiane est au milieu.")
    End Sub

    <TestMethod>
    Public Sub VersY_EstBornee()
        Dim e As New EchelleGraphique(0, 100, 200)
        Assert.AreEqual(0.0, e.VersY(150), 0.0001, "Au-delà du haut -> plafonné en haut.")
        Assert.AreEqual(200.0, e.VersY(-10), 0.0001, "En-dessous du bas -> plafonné en bas.")
    End Sub

    <TestMethod>
    Public Sub VersY_EchellePlate_NePasDiviserParZero()
        Dim e As New EchelleGraphique(5, 5, 200)
        Dim y = e.VersY(5)
        Assert.IsFalse(Double.IsNaN(y), "Pas de division par zéro sur une échelle plate.")
        Assert.AreEqual(200.0, y, 0.0001)
    End Sub

    <TestMethod>
    Public Sub Auto_DemarreAZeroSiToutPositif()
        Dim e = EchelleGraphique.Auto(New Double() {10, 20, 30}, 200)
        Assert.AreEqual(0.0, e.Bas, 0.0001)
        Assert.AreEqual(30.0, e.Haut, 0.0001)
    End Sub

    <TestMethod>
    Public Sub Auto_InclutLesValeursNegatives()
        Dim e = EchelleGraphique.Auto(New Double() {-5, 10}, 200)
        Assert.AreEqual(-5.0, e.Bas, 0.0001)
        Assert.AreEqual(10.0, e.Haut, 0.0001)
    End Sub

    <TestMethod>
    Public Sub Auto_ListeVide_BornesParDefaut()
        Dim e = EchelleGraphique.Auto(New Double() {}, 200)
        Assert.AreEqual(0.0, e.Bas, 0.0001)
        Assert.AreEqual(1.0, e.Haut, 0.0001)
    End Sub

End Class

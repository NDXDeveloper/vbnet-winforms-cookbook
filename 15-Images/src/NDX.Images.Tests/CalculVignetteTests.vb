' ============================================================================
'  CalculVignetteTests.vb  -  Tests du calcul de dimensions (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Images

<TestClass>
Public Class CalculVignetteTests

    <TestMethod>
    Public Sub Dimensionner_Paysage()
        Dim t = CalculVignette.Dimensionner(400, 200, 100, 100)
        Assert.AreEqual(100, t.Width)
        Assert.AreEqual(50, t.Height)
    End Sub

    <TestMethod>
    Public Sub Dimensionner_Portrait()
        Dim t = CalculVignette.Dimensionner(200, 400, 100, 100)
        Assert.AreEqual(50, t.Width)
        Assert.AreEqual(100, t.Height)
    End Sub

    <TestMethod>
    Public Sub Dimensionner_PlusPetiteQueLaBoite_PasDAgrandissement()
        Dim t = CalculVignette.Dimensionner(50, 40, 100, 100)
        Assert.AreEqual(50, t.Width)
        Assert.AreEqual(40, t.Height)
    End Sub

    <TestMethod>
    Public Sub Dimensionner_DimensionsInvalides_RendVide()
        Assert.AreEqual(0, CalculVignette.Dimensionner(0, 0, 100, 100).Width)
    End Sub

End Class

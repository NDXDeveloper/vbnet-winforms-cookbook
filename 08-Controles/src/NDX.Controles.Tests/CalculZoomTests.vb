' ============================================================================
'  CalculZoomTests.vb  -  Tests de l'arithmetique du zoom (sans interface).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Controles

<TestClass>
Public Class CalculZoomTests

    <TestMethod>
    Public Sub Borner_RespecteLesLimites()
        Assert.AreEqual(CalculZoom.ZoomMin, CalculZoom.Borner(0.001), 0.0000001)
        Assert.AreEqual(CalculZoom.ZoomMax, CalculZoom.Borner(99.0), 0.0000001)
        Assert.AreEqual(1.0, CalculZoom.Borner(1.0), 0.0000001)
    End Sub

    <TestMethod>
    Public Sub AppliquerCrans_Positif_Agrandit()
        Dim apres As Double = CalculZoom.AppliquerCrans(1.0, 1)
        Assert.AreEqual(CalculZoom.Pas, apres, 0.0000001)
    End Sub

    <TestMethod>
    Public Sub AppliquerCrans_Negatif_Reduit()
        Dim apres As Double = CalculZoom.AppliquerCrans(1.0, -1)
        Assert.AreEqual(1.0 / CalculZoom.Pas, apres, 0.0000001)
    End Sub

    <TestMethod>
    Public Sub AppliquerCrans_Zero_NeChangeRien()
        Assert.AreEqual(2.5, CalculZoom.AppliquerCrans(2.5, 0), 0.0000001)
    End Sub

    <TestMethod>
    Public Sub AppliquerCrans_EstBorne_AuMaximum()
        ' Beaucoup de crans positifs : on plafonne à ZoomMax.
        Assert.AreEqual(CalculZoom.ZoomMax, CalculZoom.AppliquerCrans(1.0, 100), 0.0000001)
    End Sub

    <TestMethod>
    Public Sub AppliquerCrans_EstBorne_AuMinimum()
        Assert.AreEqual(CalculZoom.ZoomMin, CalculZoom.AppliquerCrans(1.0, -100), 0.0000001)
    End Sub

End Class

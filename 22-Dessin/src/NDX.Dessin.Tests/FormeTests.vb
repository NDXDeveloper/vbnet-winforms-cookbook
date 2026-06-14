' ============================================================================
'  FormeTests.vb  -  Tests du test de contenance (hit-testing, pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Dessin

<TestClass>
Public Class FormeTests

    <TestMethod>
    Public Sub Rectangle_Contient()
        Dim f As New Forme() With {.Type = TypeForme.Rectangle, .X = 10, .Y = 10, .Largeur = 100, .Hauteur = 50}
        Assert.IsTrue(f.Contient(50, 30), "Point intérieur.")
        Assert.IsFalse(f.Contient(200, 200), "Point extérieur.")
    End Sub

    <TestMethod>
    Public Sub Rectangle_DimensionsNegatives_SontNormalisees()
        ' Tracé « vers le haut/gauche » : largeur/hauteur négatives.
        Dim f As New Forme() With {.Type = TypeForme.Rectangle, .X = 110, .Y = 60, .Largeur = -100, .Hauteur = -50}
        Assert.IsTrue(f.Contient(50, 30), "Le rectangle est normalisé avant le test.")
    End Sub

    <TestMethod>
    Public Sub Ellipse_Contient()
        Dim f As New Forme() With {.Type = TypeForme.Ellipse, .X = 0, .Y = 0, .Largeur = 100, .Hauteur = 100}
        Assert.IsTrue(f.Contient(50, 50), "Le centre est dedans.")
        Assert.IsFalse(f.Contient(5, 5), "Le coin du cadre est HORS de l'ellipse.")
    End Sub

    <TestMethod>
    Public Sub Ligne_Contient()
        Dim f As New Forme() With {.Type = TypeForme.Ligne, .X = 0, .Y = 0, .Largeur = 100, .Hauteur = 100}
        Assert.IsTrue(f.Contient(50, 50), "Point sur la ligne (à la tolérance près).")
        Assert.IsFalse(f.Contient(50, 0), "Point éloigné de la ligne.")
    End Sub

End Class

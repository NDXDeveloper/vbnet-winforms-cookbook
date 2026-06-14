' ============================================================================
'  FiltresTests.vb  -  Tests des filtres ColorMatrix (sur de vrais pixels).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Images

<TestClass>
Public Class FiltresTests

    Private Shared Function Uni(ByVal c As Color) As Bitmap
        Dim bmp As New Bitmap(4, 4)
        Using g = Graphics.FromImage(bmp)
            g.Clear(c)
        End Using
        Return bmp
    End Function

    <TestMethod>
    Public Sub NiveauxDeGris_RendUnPixelGris()
        Using source = Uni(Color.FromArgb(200, 50, 50))
            Using gris = Filtres.NiveauxDeGris(source)
                Dim p = gris.GetPixel(1, 1)
                Assert.AreEqual(p.R, p.G, "Un pixel gris a R = V = B.")
                Assert.AreEqual(p.G, p.B)
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub Negatif_InverseLesComposantes()
        Using source = Uni(Color.FromArgb(200, 50, 50))
            Using neg = Filtres.Negatif(source)
                Dim p = neg.GetPixel(1, 1)
                Assert.IsTrue(Math.Abs(p.R - 55) <= 2, "R inversé attendu ≈ 55, obtenu " & p.R)
                Assert.IsTrue(Math.Abs(p.G - 205) <= 2, "V inversé attendu ≈ 205, obtenu " & p.G)
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub Luminosite_ConserveLesDimensions()
        Using source = Uni(Color.Gray)
            Using clair = Filtres.Luminosite(source, 1.5F)
                Assert.AreEqual(source.Width, clair.Width)
                Assert.AreEqual(source.Height, clair.Height)
            End Using
        End Using
    End Sub

End Class

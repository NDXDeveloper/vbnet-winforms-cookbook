' ============================================================================
'  Vignette.vb  -  Generation de vignettes (redimensionnement de qualite).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO

''' <summary>Génère une vignette (image réduite) en conservant les proportions.</summary>
Public NotInheritable Class Vignette

    Private Sub New()
    End Sub

    ''' <summary>Crée une vignette tenant dans <paramref name="maxL"/>×<paramref name="maxH"/>.</summary>
    Public Shared Function Generer(ByVal source As Image, Optional ByVal maxL As Integer = 160, Optional ByVal maxH As Integer = 160) As Bitmap
        If source Is Nothing Then Throw New ArgumentNullException(NameOf(source))
        Dim taille As Size = CalculVignette.Dimensionner(source.Width, source.Height, maxL, maxH)
        Dim resultat As New Bitmap(taille.Width, taille.Height)
        Using g As Graphics = Graphics.FromImage(resultat)
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.DrawImage(source, 0, 0, taille.Width, taille.Height)
        End Using
        Return resultat
    End Function

    ''' <summary>Encode une image en PNG (tableau d'octets).</summary>
    Public Shared Function VersPng(ByVal image As Image) As Byte()
        Using flux As New MemoryStream()
            image.Save(flux, Imaging.ImageFormat.Png)
            Return flux.ToArray()
        End Using
    End Function

End Class

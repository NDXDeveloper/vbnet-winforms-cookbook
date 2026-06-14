' ============================================================================
'  Filtres.vb  -  Filtres d'image via ColorMatrix (GDI+).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Drawing.Imaging

''' <summary>
''' Applique des filtres colorimétriques à une image via une <see cref="ColorMatrix"/>
''' (transformation linéaire 5×5 appliquée à chaque pixel). Niveaux de gris,
''' luminosité, négatif.
''' </summary>
Public NotInheritable Class Filtres

    Private Sub New()
    End Sub

    ''' <summary>Convertit en niveaux de gris (pondération de luminance perçue).</summary>
    Public Shared Function NiveauxDeGris(ByVal source As Image) As Bitmap
        Dim m As New ColorMatrix(New Single()() {
            New Single() {0.299F, 0.299F, 0.299F, 0, 0},
            New Single() {0.587F, 0.587F, 0.587F, 0, 0},
            New Single() {0.114F, 0.114F, 0.114F, 0, 0},
            New Single() {0, 0, 0, 1, 0},
            New Single() {0, 0, 0, 0, 1}})
        Return Appliquer(source, m)
    End Function

    ''' <summary>Négatif (inversion des composantes RVB).</summary>
    Public Shared Function Negatif(ByVal source As Image) As Bitmap
        Dim m As New ColorMatrix(New Single()() {
            New Single() {-1, 0, 0, 0, 0},
            New Single() {0, -1, 0, 0, 0},
            New Single() {0, 0, -1, 0, 0},
            New Single() {0, 0, 0, 1, 0},
            New Single() {1, 1, 1, 0, 1}})
        Return Appliquer(source, m)
    End Function

    ''' <summary>Ajuste la luminosité (<paramref name="facteur"/> &gt; 1 éclaircit, &lt; 1 assombrit).</summary>
    Public Shared Function Luminosite(ByVal source As Image, ByVal facteur As Single) As Bitmap
        Dim m As New ColorMatrix(New Single()() {
            New Single() {facteur, 0, 0, 0, 0},
            New Single() {0, facteur, 0, 0, 0},
            New Single() {0, 0, facteur, 0, 0},
            New Single() {0, 0, 0, 1, 0},
            New Single() {0, 0, 0, 0, 1}})
        Return Appliquer(source, m)
    End Function

    ''' <summary>Applique une matrice de couleur et renvoie une nouvelle image.</summary>
    Private Shared Function Appliquer(ByVal source As Image, ByVal matrice As ColorMatrix) As Bitmap
        If source Is Nothing Then Throw New ArgumentNullException(NameOf(source))
        Dim resultat As New Bitmap(source.Width, source.Height)
        Using g As Graphics = Graphics.FromImage(resultat)
            Using attributs As New ImageAttributes()
                attributs.SetColorMatrix(matrice)
                Dim rect As New Rectangle(0, 0, source.Width, source.Height)
                g.DrawImage(source, rect, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributs)
            End Using
        End Using
        Return resultat
    End Function

End Class

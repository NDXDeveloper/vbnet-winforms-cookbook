' ============================================================================
'  PageFiltres.vb  -  Application de filtres (avant / apres).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Images

''' <summary>Applique un filtre à une image et compare l'original au résultat.</summary>
Public NotInheritable Class PageFiltres
    Inherits PageBase

    Private ReadOnly _pbOriginal As New PictureBox() With {.Dock = DockStyle.Fill, .SizeMode = PictureBoxSizeMode.Zoom, .BackColor = Color.FromArgb(245, 245, 245)}
    Private ReadOnly _pbResultat As New PictureBox() With {.Dock = DockStyle.Fill, .SizeMode = PictureBoxSizeMode.Zoom, .BackColor = Color.FromArgb(245, 245, 245)}
    Private _original As Image

    Public Sub New()
        MyBase.New("Filtres (avant/après)", "Niveaux de gris, négatif, luminosité — par ColorMatrix. Image de démo intégrée, ou ouvrez un fichier.")
        Construire()
        ChargerOriginal(ImageDemonstration())
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        haut.Controls.Add(Bouton("Image de démo", AddressOf SurDemo))
        haut.Controls.Add(Bouton("Ouvrir…", AddressOf SurOuvrir))
        haut.Controls.Add(Bouton("Niveaux de gris", AddressOf SurGris))
        haut.Controls.Add(Bouton("Négatif", AddressOf SurNegatif))
        haut.Controls.Add(Bouton("Éclaircir", AddressOf SurEclaircir))
        haut.Controls.Add(Bouton("Assombrir", AddressOf SurAssombrir))

        Dim grille As New TableLayoutPanel() With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 2}
        grille.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        grille.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50))
        grille.RowStyles.Add(New RowStyle(SizeType.Absolute, 22))
        grille.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        grille.Controls.Add(New Label() With {.Text = "Original", .Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleCenter}, 0, 0)
        grille.Controls.Add(New Label() With {.Text = "Résultat", .Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleCenter}, 1, 0)
        grille.Controls.Add(_pbOriginal, 0, 1)
        grille.Controls.Add(_pbResultat, 1, 1)

        Contenu.Controls.Add(grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub ChargerOriginal(ByVal image As Image)
        If _original IsNot Nothing Then _original.Dispose()
        _original = image
        _pbOriginal.Image = _original
        AfficherResultat(Nothing)
    End Sub

    Private Sub AfficherResultat(ByVal image As Image)
        If _pbResultat.Image IsNot Nothing Then _pbResultat.Image.Dispose()
        _pbResultat.Image = image
    End Sub

    Private Sub SurDemo(ByVal sender As Object, ByVal e As EventArgs)
        ChargerOriginal(ImageDemonstration())
    End Sub

    Private Sub SurOuvrir(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New OpenFileDialog() With {.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif"}
            If dlg.ShowDialog() = DialogResult.OK Then ChargerOriginal(Image.FromFile(dlg.FileName))
        End Using
    End Sub

    Private Sub SurGris(ByVal sender As Object, ByVal e As EventArgs)
        If _original IsNot Nothing Then AfficherResultat(Filtres.NiveauxDeGris(_original))
    End Sub

    Private Sub SurNegatif(ByVal sender As Object, ByVal e As EventArgs)
        If _original IsNot Nothing Then AfficherResultat(Filtres.Negatif(_original))
    End Sub

    Private Sub SurEclaircir(ByVal sender As Object, ByVal e As EventArgs)
        If _original IsNot Nothing Then AfficherResultat(Filtres.Luminosite(_original, 1.4F))
    End Sub

    Private Sub SurAssombrir(ByVal sender As Object, ByVal e As EventArgs)
        If _original IsNot Nothing Then AfficherResultat(Filtres.Luminosite(_original, 0.6F))
    End Sub

End Class

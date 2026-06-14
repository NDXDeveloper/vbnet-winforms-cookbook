Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

''' <summary>Page "Images" : nuances de gris, detection de format par signature, plan vide.</summary>
Public NotInheritable Class PageImages
    Inherits PageBase

    Private ReadOnly _picOriginal As New PictureBox()
    Private ReadOnly _picGris As New PictureBox()
    Private ReadOnly _lblInfo As New Label()
    Private _imgCourante As Image

    Public Sub New()
        MyBase.New("Images",
                   "EnNuancesDeGris (matrice de couleurs), EstEnNuancesDeGris, DetecterExtension (signature binaire), CreerPlanVide.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True}
        barre.Controls.Add(Bouton("Charger une image...", AddressOf SurCharger))
        barre.Controls.Add(Bouton("Generer un plan vide", AddressOf SurPlanVide))
        barre.Controls.Add(Bouton("Convertir en gris", AddressOf SurGris))
        barre.Controls.Add(Bouton("Deja en gris ?", AddressOf SurEstGris))
        barre.Controls.Add(Bouton("Detecter extension d'un fichier...", AddressOf SurDetecter))

        _lblInfo.Dock = DockStyle.Bottom
        _lblInfo.Height = 26
        _lblInfo.TextAlign = ContentAlignment.MiddleLeft
        _lblInfo.Font = New Font("Segoe UI", 9.5F)

        Dim split As New SplitContainer() With {.Dock = DockStyle.Fill}
        ConfigurerApercu(_picOriginal, "Original")
        ConfigurerApercu(_picGris, "Resultat (gris)")
        split.Panel1.Controls.Add(_picOriginal)
        split.Panel2.Controls.Add(_picGris)

        Contenu.Controls.Add(split)
        Contenu.Controls.Add(_lblInfo)
        Contenu.Controls.Add(barre)
    End Sub

    Private Sub ConfigurerApercu(ByVal pic As PictureBox, ByVal titre As String)
        pic.Dock = DockStyle.Fill
        pic.SizeMode = PictureBoxSizeMode.Zoom
        pic.BackColor = Color.FromArgb(248, 248, 248)
        pic.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Private Sub DefinirImage(ByVal img As Image)
        _imgCourante?.Dispose()
        _imgCourante = img
        _picOriginal.Image = img
        _picGris.Image = Nothing
    End Sub

    Private Sub SurCharger(ByVal sender As Object, ByVal e As EventArgs)
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Tous les fichiers|*.*"
            If ofd.ShowDialog() = DialogResult.OK Then
                Try
                    DefinirImage(Image.FromFile(ofd.FileName))
                    _lblInfo.Text = "Charge : " & Path.GetFileName(ofd.FileName)
                Catch ex As Exception
                    _lblInfo.Text = "Impossible de charger l'image : " & ex.Message
                End Try
            End If
        End Using
    End Sub

    Private Sub SurPlanVide(ByVal sender As Object, ByVal e As EventArgs)
        Dim octets As Byte() = OutilsImages.CreerPlanVide(800, 560)
        If octets Is Nothing Then Return
        Using ms As New MemoryStream(octets)
            ' On clone pour pouvoir disposer le flux sans invalider l'image.
            DefinirImage(New Bitmap(Image.FromStream(ms)))
        End Using
        _lblInfo.Text = "Plan vide genere (" & octets.Length.ToString() & " octets en JPEG)."
    End Sub

    Private Sub SurGris(ByVal sender As Object, ByVal e As EventArgs)
        If _imgCourante Is Nothing Then
            _lblInfo.Text = "Chargez d'abord une image ou generez un plan vide."
            Return
        End If
        _picGris.Image = OutilsImages.EnNuancesDeGris(_imgCourante)
        _lblInfo.Text = "Conversion en nuances de gris effectuee (coefficients 0.30 / 0.59 / 0.11)."
    End Sub

    Private Sub SurEstGris(ByVal sender As Object, ByVal e As EventArgs)
        If _imgCourante Is Nothing Then
            _lblInfo.Text = "Chargez d'abord une image."
            Return
        End If
        _lblInfo.Text = "EstEnNuancesDeGris = " & OutilsImages.EstEnNuancesDeGris(_imgCourante).ToString()
    End Sub

    Private Sub SurDetecter(ByVal sender As Object, ByVal e As EventArgs)
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Tous les fichiers|*.*"
            If ofd.ShowDialog() = DialogResult.OK Then
                Try
                    Dim octets As Byte() = File.ReadAllBytes(ofd.FileName)
                    Dim ext As String = OutilsImages.DetecterExtension(octets)
                    _lblInfo.Text = "Extension detectee par signature : " &
                                    If(String.IsNullOrEmpty(ext), "(inconnue)", ext) &
                                    "  | extension du nom : " & Path.GetExtension(ofd.FileName)
                Catch ex As Exception
                    _lblInfo.Text = "Erreur de lecture : " & ex.Message
                End Try
            End If
        End Using
    End Sub

End Class

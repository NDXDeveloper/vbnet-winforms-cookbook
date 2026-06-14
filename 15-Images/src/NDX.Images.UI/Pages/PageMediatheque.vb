' ============================================================================
'  PageMediatheque.vb  -  Mediatheque : ajouter / lister / relire les vignettes.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports NDX.Images

''' <summary>Stocke des images (métadonnées + vignette) en base et les relit.</summary>
Public NotInheritable Class PageMediatheque
    Inherits PageBase

    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _apercu As New PictureBox() With {.Dock = DockStyle.Fill, .SizeMode = PictureBoxSizeMode.Zoom, .BackColor = Color.FromArgb(245, 245, 245)}
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Médiathèque (base)", "Ajoute une image (sa vignette PNG est stockée en base), liste les images et réaffiche la vignette sélectionnée.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(Bouton("Ajouter une image de démo", AddressOf SurAjouterDemo))
        haut.Controls.Add(Bouton("Ouvrir et ajouter…", AddressOf SurAjouterFichier))
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))
        haut.Controls.Add(_lblEtat)

        Dim grille As New TableLayoutPanel() With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 1}
        grille.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 60))
        grille.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40))

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.MultiSelect = False
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White
        AddHandler _grille.SelectionChanged, AddressOf SurSelection

        grille.Controls.Add(_grille, 0, 0)
        grille.Controls.Add(_apercu, 1, 0)

        Contenu.Controls.Add(grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurAjouterDemo(ByVal sender As Object, ByVal e As EventArgs)
        Using img As Bitmap = ImageDemonstration()
            Ajouter("demo-" & DateTime.Now.ToString("HHmmss") & ".png", Vignette.VersPng(img))
        End Using
    End Sub

    Private Sub SurAjouterFichier(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New OpenFileDialog() With {.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif"}
            If dlg.ShowDialog() = DialogResult.OK Then
                Ajouter(Path.GetFileName(dlg.FileName), File.ReadAllBytes(dlg.FileName))
            End If
        End Using
    End Sub

    Private Sub Ajouter(ByVal nom As String, ByVal donnees As Byte())
        Try
            Dim id As Integer = DepotImage.Enregistrer(nom, donnees)
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Image ajoutée (id = " & id.ToString() & ")."
            SurLister(Me, EventArgs.Empty)
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotImage.ListerTable()
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurSelection(ByVal sender As Object, ByVal e As EventArgs)
        If _grille.CurrentRow Is Nothing OrElse _grille.CurrentRow.Cells("Id") Is Nothing Then Return
        Dim valeur As Object = _grille.CurrentRow.Cells("Id").Value
        If valeur Is Nothing OrElse Convert.IsDBNull(valeur) Then Return
        Try
            Dim octets As Byte() = DepotImage.ChargerVignette(Convert.ToInt32(valeur))
            If _apercu.Image IsNot Nothing Then _apercu.Image.Dispose()
            Using flux As New MemoryStream(octets)
                _apercu.Image = Image.FromStream(flux)
            End Using
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

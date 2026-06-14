' ============================================================================
'  PageArchives.vb  -  "Compress-then-store" : archives compressees en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports NDX.Compression

''' <summary>Enregistre des textes compressés en base, les liste et les recharge.</summary>
Public NotInheritable Class PageArchives
    Inherits PageBase

    Private ReadOnly _txtNom As New TextBox() With {.Width = 220}
    Private ReadOnly _txtContenu As New TextBox()
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _sortie As TextBox

    Public Sub New()
        MyBase.New("Archives (base)", "Compresse un texte, le stocke en base (taille d'origine, taille compressée, SHA-256), puis le recharge.")
        _sortie = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New Panel() With {.Dock = DockStyle.Top, .Height = 132}

        _txtContenu.Multiline = True
        _txtContenu.ScrollBars = ScrollBars.Vertical
        _txtContenu.Dock = DockStyle.Fill
        _txtContenu.Font = New Font("Consolas", 9.5F)
        _txtContenu.Text = "Contenu de démonstration à compresser puis stocker en base."

        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Bottom, .Height = 40, .FlowDirection = FlowDirection.LeftToRight}
        barre.Controls.Add(New Label() With {.Text = "Nom :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        _txtNom.Text = "document.txt"
        barre.Controls.Add(_txtNom)
        barre.Controls.Add(Bouton("Enregistrer (compressé)", AddressOf SurEnregistrer))
        barre.Controls.Add(Bouton("Lister", AddressOf SurLister))
        barre.Controls.Add(Bouton("Recharger la sélection", AddressOf SurRecharger))

        haut.Controls.Add(_txtContenu)
        haut.Controls.Add(barre)

        Dim centre As New Panel() With {.Dock = DockStyle.Fill}
        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.MultiSelect = False
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White

        Dim bas As New Panel() With {.Dock = DockStyle.Bottom, .Height = 150}
        _sortie.Dock = DockStyle.Fill
        bas.Controls.Add(_sortie)

        centre.Controls.Add(_grille)

        Contenu.Controls.Add(centre)
        Contenu.Controls.Add(bas)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurEnregistrer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim donnees As Byte() = Encoding.UTF8.GetBytes(_txtContenu.Text)
            Dim algo As AlgorithmeCompression = AlgorithmeCompression.GZip
            Dim id As Integer = DepotArchive.Enregistrer(_txtNom.Text, donnees, algo)
            _sortie.Text = "Archive enregistrée (id = " & id.ToString() & ") - " &
                           donnees.Length.ToString("N0") & " octets compressés en GZip." & vbCrLf
            SurLister(sender, e)
        Catch ex As Exception
            _sortie.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotArchive.Lister()
        Catch ex As Exception
            _sortie.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurRecharger(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If _grille.CurrentRow Is Nothing Then
                _sortie.Text = "Sélectionnez d'abord une ligne dans la liste."
                Return
            End If
            Dim id As Integer = Convert.ToInt32(_grille.CurrentRow.Cells("Id").Value)
            Dim donnees As Byte() = DepotArchive.Recharger(id)
            Dim texte As String = Encoding.UTF8.GetString(donnees)
            Dim apercu As String = If(texte.Length > 600, texte.Substring(0, 600) & " […]", texte)
            _sortie.Text = "Archive id=" & id.ToString() & " rechargée et décompressée (" &
                           donnees.Length.ToString("N0") & " octets) :" & vbCrLf & vbCrLf & apercu
        Catch ex As Exception
            _sortie.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

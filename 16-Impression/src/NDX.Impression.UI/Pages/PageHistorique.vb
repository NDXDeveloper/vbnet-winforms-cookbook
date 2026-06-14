' ============================================================================
'  PageHistorique.vb  -  Historique des documents imprimes (base).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Impression

''' <summary>Liste les documents archivés et réaffiche le contenu sélectionné.</summary>
Public NotInheritable Class PageHistorique
    Inherits PageBase

    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _apercu As TextBox
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Historique (base)", "Documents soumis à l'impression. Sélectionnez une ligne pour relire son contenu.")
        _apercu = New TextBox() With {.Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Both, .WordWrap = False,
                                      .Dock = DockStyle.Fill, .BackColor = Color.FromArgb(248, 248, 248), .Font = New Font("Consolas", 9.5F)}
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))
        haut.Controls.Add(Bouton("Relire la sélection", AddressOf SurRelire))
        haut.Controls.Add(_lblEtat)

        _grille.Dock = DockStyle.Top
        _grille.Height = 240
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.MultiSelect = False
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White

        Contenu.Controls.Add(_apercu)
        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotImpression.ListerTable()
            _lblEtat.Text = ""
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurRelire(ByVal sender As Object, ByVal e As EventArgs)
        If _grille.CurrentRow Is Nothing Then Return
        Try
            Dim id As Integer = Convert.ToInt32(_grille.CurrentRow.Cells("Id").Value)
            _apercu.Text = DepotImpression.Recharger(id)
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

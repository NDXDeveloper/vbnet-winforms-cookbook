' ============================================================================
'  PagePreferences.vb  -  Stockage clef/valeur des preferences (base).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Controles

''' <summary>Liste, crée et met à jour des préférences (upsert) en base.</summary>
Public NotInheritable Class PagePreferences
    Inherits PageBase

    Private ReadOnly _txtCle As New TextBox() With {.Width = 200}
    Private ReadOnly _txtValeur As New TextBox() With {.Width = 240}
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Préférences (base)", "Stockage clef/valeur persistant (INSERT ... ON DUPLICATE KEY UPDATE : créer ou mettre à jour en une requête).")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(New Label() With {.Text = "Clef :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_txtCle)
        haut.Controls.Add(New Label() With {.Text = "Valeur :", .AutoSize = True, .Margin = New Padding(8, 9, 2, 0)})
        haut.Controls.Add(_txtValeur)
        haut.Controls.Add(Bouton("Écrire (upsert)", AddressOf SurEcrire))
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))
        haut.Controls.Add(_lblEtat)

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White

        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotPreferences.Lister()
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = ""
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurEcrire(ByVal sender As Object, ByVal e As EventArgs)
        If String.IsNullOrWhiteSpace(_txtCle.Text) Then
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "La clef est obligatoire."
            Return
        End If
        Try
            DepotPreferences.Ecrire(_txtCle.Text.Trim(), _txtValeur.Text)
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Préférence enregistrée."
            SurLister(sender, e)
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

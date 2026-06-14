' ============================================================================
'  PageLiaisons.vb  -  Liaisons action <-> raccourci, persistees en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Raccourcis

''' <summary>Définit et liste les liaisons « action ↔ raccourci » en base (upsert).</summary>
Public NotInheritable Class PageLiaisons
    Inherits PageBase

    Private ReadOnly _txtAction As New TextBox() With {.Width = 220}
    Private ReadOnly _txtRaccourci As New TextBox() With {.Width = 200}
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Liaisons (base)", "Le raccourci saisi est analysé puis normalisé (« ctrl+k ctrl+s » → « Ctrl+K, Ctrl+S ») avant l'enregistrement.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(New Label() With {.Text = "Action :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        _txtAction.Text = "Enregistrer"
        haut.Controls.Add(_txtAction)
        haut.Controls.Add(New Label() With {.Text = "Raccourci :", .AutoSize = True, .Margin = New Padding(10, 9, 2, 0)})
        _txtRaccourci.Text = "Ctrl+S"
        haut.Controls.Add(_txtRaccourci)
        haut.Controls.Add(Bouton("Définir (upsert)", AddressOf SurDefinir))
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))
        haut.Controls.Add(Bouton("Vérifier les conflits", AddressOf SurVerifier))
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
            _grille.DataSource = DepotRaccourci.ListerTable()
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = ""
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurDefinir(ByVal sender As Object, ByVal e As EventArgs)
        If String.IsNullOrWhiteSpace(_txtAction.Text) Then
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "L'action est obligatoire."
            Return
        End If
        Try
            DepotRaccourci.Definir(_txtAction.Text.Trim(), _txtRaccourci.Text)
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Liaison enregistrée."
            SurLister(sender, e)
        Catch ex As FormatException
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = ex.Message
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    ''' <summary>Tente de construire un gestionnaire : révèle doublons et conflits de préfixe.</summary>
    Private Sub SurVerifier(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim g As GestionnaireRaccourcis = DepotRaccourci.ConstruireGestionnaire()
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = g.Nombre.ToString() & " raccourci(s) cohérent(s), aucun conflit."
        Catch ex As InvalidOperationException
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Conflit : " & ex.Message
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

' ============================================================================
'  PageMesures.vb  -  Mesures (base) : liste + ajout.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Globalization
Imports System.Windows.Forms
Imports NDX.Graphiques

''' <summary>Liste les mesures en base et permet d'en ajouter (alimente le graphique).</summary>
Public NotInheritable Class PageMesures
    Inherits PageBase

    Private ReadOnly _txtLibelle As New TextBox() With {.Width = 160, .Text = "Juil"}
    Private ReadOnly _txtValeur As New TextBox() With {.Width = 80, .Text = "21"}
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Mesures (base)", "Les mesures (libellé + valeur) alimentent le graphique. Ajoutez-en puis rechargez le graphique.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(New Label() With {.Text = "Libellé :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_txtLibelle)
        haut.Controls.Add(New Label() With {.Text = "Valeur :", .AutoSize = True, .Margin = New Padding(8, 9, 2, 0)})
        haut.Controls.Add(_txtValeur)
        haut.Controls.Add(Bouton("Ajouter", AddressOf SurAjouter))
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))
        haut.Controls.Add(_lblEtat)

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White

        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurAjouter(ByVal sender As Object, ByVal e As EventArgs)
        Dim valeur As Double
        If Not Double.TryParse(_txtValeur.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, valeur) Then
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Valeur numérique attendue."
            Return
        End If
        Try
            DepotMesure.Ajouter(_txtLibelle.Text.Trim(), valeur)
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Mesure ajoutée."
            SurLister(sender, e)
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotMesure.ListerTable()
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

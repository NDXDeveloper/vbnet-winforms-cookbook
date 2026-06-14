' ============================================================================
'  PageCatalogue.vb  -  Lister/ajouter/supprimer des produits (Repository, pagination).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Globalization
Imports System.Windows.Forms
Imports NDX.Donnees

Public NotInheritable Class PageCatalogue
    Inherits PageBase

    Private ReadOnly _depot As New DepotProduit()
    Private ReadOnly _dgv As New DataGridView()
    Private ReadOnly _lblEtat As New Label()
    Private ReadOnly _txtRef As New TextBox()
    Private ReadOnly _txtDes As New TextBox()
    Private ReadOnly _txtPrix As New TextBox()
    Private ReadOnly _txtStock As New TextBox()
    Private Const TAILLE As Integer = 10
    Private _page As Integer = 1

    Public Sub New()
        MyBase.New("Catalogue (Repository)", "Lister (paginé), ajouter et supprimer des produits via le dépôt.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim nav As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True}
        nav.Controls.Add(Bouton("Rafraîchir", AddressOf SurRafraichir))
        nav.Controls.Add(Bouton("< Précédent", AddressOf SurPrecedent))
        nav.Controls.Add(Bouton("Suivant >", AddressOf SurSuivant))
        nav.Controls.Add(Bouton("Supprimer la sélection", AddressOf SurSupprimer))

        Dim ajout As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True}
        _txtRef.Width = 90 : _txtDes.Width = 200 : _txtPrix.Width = 70 : _txtStock.Width = 60
        _txtRef.Text = "REF-001" : _txtDes.Text = "Nouveau produit" : _txtPrix.Text = "9.90" : _txtStock.Text = "100"
        ajout.Controls.Add(New Label() With {.Text = "Réf :", .AutoSize = True, .Margin = New Padding(4, 9, 0, 0)})
        ajout.Controls.Add(_txtRef)
        ajout.Controls.Add(New Label() With {.Text = "Désignation :", .AutoSize = True, .Margin = New Padding(6, 9, 0, 0)})
        ajout.Controls.Add(_txtDes)
        ajout.Controls.Add(New Label() With {.Text = "Prix :", .AutoSize = True, .Margin = New Padding(6, 9, 0, 0)})
        ajout.Controls.Add(_txtPrix)
        ajout.Controls.Add(New Label() With {.Text = "Stock :", .AutoSize = True, .Margin = New Padding(6, 9, 0, 0)})
        ajout.Controls.Add(_txtStock)
        ajout.Controls.Add(Bouton("Ajouter", AddressOf SurAjouter))

        _lblEtat.Dock = DockStyle.Bottom
        _lblEtat.Height = 24
        _lblEtat.TextAlign = ContentAlignment.MiddleLeft

        _dgv.Dock = DockStyle.Fill
        _dgv.ReadOnly = True
        _dgv.AllowUserToAddRows = False
        _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _dgv.BackgroundColor = Color.White

        Contenu.Controls.Add(_dgv)
        Contenu.Controls.Add(_lblEtat)
        Contenu.Controls.Add(ajout)
        Contenu.Controls.Add(nav)
    End Sub

    Private Sub Rafraichir()
        Try
            _dgv.DataSource = _depot.Lister(_page, TAILLE)
            Dim total As Long = _depot.Compter()
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = String.Format("Page {0} (taille {1}) — {2} produit(s) au total.", _page, TAILLE, total)
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Base indisponible : " & ex.Message
        End Try
    End Sub

    Private Sub SurRafraichir(ByVal s As Object, ByVal e As EventArgs)
        Rafraichir()
    End Sub
    Private Sub SurPrecedent(ByVal s As Object, ByVal e As EventArgs)
        If _page > 1 Then _page -= 1
        Rafraichir()
    End Sub
    Private Sub SurSuivant(ByVal s As Object, ByVal e As EventArgs)
        _page += 1
        Rafraichir()
    End Sub

    Private Sub SurAjouter(ByVal s As Object, ByVal e As EventArgs)
        Try
            Dim prix As Decimal
            Decimal.TryParse(_txtPrix.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, prix)
            Dim stock As Integer
            Integer.TryParse(_txtStock.Text, stock)
            Dim p As New Produit() With {.Reference = _txtRef.Text.Trim(), .Designation = _txtDes.Text.Trim(), .PrixHt = prix, .Stock = stock}
            Dim id As Integer = _depot.Inserer(p)
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Produit inséré (id " & id.ToString() & ")."
            Rafraichir()
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Échec de l'insertion : " & ex.Message
        End Try
    End Sub

    Private Sub SurSupprimer(ByVal s As Object, ByVal e As EventArgs)
        If _dgv.CurrentRow Is Nothing OrElse _dgv.CurrentRow.Cells("Id").Value Is Nothing Then
            _lblEtat.Text = "Sélectionnez une ligne."
            Return
        End If
        Try
            Dim id As Integer = Convert.ToInt32(_dgv.CurrentRow.Cells("Id").Value)
            If _depot.Supprimer(id) Then
                _lblEtat.ForeColor = Color.Green
                _lblEtat.Text = "Produit " & id.ToString() & " supprimé."
            End If
            Rafraichir()
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Échec de la suppression : " & ex.Message
        End Try
    End Sub

End Class

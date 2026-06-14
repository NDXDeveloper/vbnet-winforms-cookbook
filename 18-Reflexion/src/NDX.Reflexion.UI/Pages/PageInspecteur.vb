' ============================================================================
'  PageInspecteur.vb  -  Inspection des membres d'un type (+ catalogue en base).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Reflexion

''' <summary>Choisit un type et liste ses membres ; peut les cataloguer en base.</summary>
Public NotInheritable Class PageInspecteur
    Inherits PageBase

    Private ReadOnly _cbo As New ComboBox() With {.Width = 280, .DropDownStyle = ComboBoxStyle.DropDownList}
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Inspecteur de type", "Liste les membres publics (propriétés, champs, événements) du type choisi, par réflexion.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        _cbo.Items.Add(New ElementType("Article (exemple)", GetType(Article)))
        _cbo.Items.Add(New ElementType("Personne (exemple)", GetType(Personne)))
        _cbo.Items.Add(New ElementType("System.String", GetType(String)))
        _cbo.Items.Add(New ElementType("System.DateTime", GetType(DateTime)))
        _cbo.SelectedIndex = 0
        haut.Controls.Add(New Label() With {.Text = "Type :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_cbo)
        haut.Controls.Add(Bouton("Inspecter", AddressOf SurInspecter))
        haut.Controls.Add(Bouton("Cataloguer en base", AddressOf SurCataloguer))
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

    Private Function TypeChoisi() As Type
        Return DirectCast(_cbo.SelectedItem, ElementType).Type
    End Function

    Private Sub SurInspecter(ByVal sender As Object, ByVal e As EventArgs)
        _grille.DataSource = InspecteurType.ListerTout(TypeChoisi())
        _lblEtat.ForeColor = Color.DimGray
        _lblEtat.Text = _grille.RowCount.ToString() & " membre(s)."
    End Sub

    Private Sub SurCataloguer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim n As Integer = DepotMetadonnees.EnregistrerDescription(TypeChoisi())
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = n.ToString() & " membre(s) catalogué(s) en base."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private NotInheritable Class ElementType
        Public ReadOnly Property Libelle As String
        Public ReadOnly Property Type As Type
        Public Sub New(ByVal libelle As String, ByVal t As Type)
            Me.Libelle = libelle
            Me.Type = t
        End Sub
        Public Overrides Function ToString() As String
            Return Libelle
        End Function
    End Class

End Class

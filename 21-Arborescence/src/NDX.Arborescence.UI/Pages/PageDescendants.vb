' ============================================================================
'  PageDescendants.vb  -  Descendants d'un noeud via requete recursive.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Arborescence

''' <summary>Choisit un nœud et liste tous ses descendants (WITH RECURSIVE).</summary>
Public NotInheritable Class PageDescendants
    Inherits PageBase

    Private ReadOnly _cbo As New ComboBox() With {.Width = 280, .DropDownStyle = ComboBoxStyle.DropDownList}
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Descendants (récursif)", "Sélectionnez un nœud : tous ses descendants sont récupérés en une requête récursive (WITH RECURSIVE).")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(New Label() With {.Text = "Nœud :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_cbo)
        haut.Controls.Add(Bouton("Charger les nœuds", AddressOf SurCharger))
        haut.Controls.Add(Bouton("Voir les descendants", AddressOf SurDescendants))
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

    Private Sub SurCharger(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _cbo.Items.Clear()
            For Each n As Noeud In DepotNoeud.Lister()
                _cbo.Items.Add(New ElementNoeud(n.Id, n.Libelle))
            Next
            If _cbo.Items.Count > 0 Then _cbo.SelectedIndex = 0
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = _cbo.Items.Count.ToString() & " nœud(s)."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurDescendants(ByVal sender As Object, ByVal e As EventArgs)
        Dim element As ElementNoeud = TryCast(_cbo.SelectedItem, ElementNoeud)
        If element Is Nothing Then
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Chargez puis sélectionnez un nœud."
            Return
        End If
        Try
            _grille.DataSource = DepotNoeud.Descendants(element.Id)
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = _grille.RowCount.ToString() & " descendant(s)."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private NotInheritable Class ElementNoeud
        Public ReadOnly Property Id As Integer
        Private ReadOnly _libelle As String
        Public Sub New(ByVal id As Integer, ByVal libelle As String)
            Me.Id = id
            _libelle = libelle
        End Sub
        Public Overrides Function ToString() As String
            Return "#" & Id.ToString() & " — " & _libelle
        End Function
    End Class

End Class

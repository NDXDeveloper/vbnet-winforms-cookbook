' ============================================================================
'  PageArbre.vb  -  Affichage de l'arbre (TreeView) + ajout / suppression.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Arborescence

''' <summary>Reconstruit l'arbre depuis la base et l'affiche dans un TreeView (avec CRUD).</summary>
Public NotInheritable Class PageArbre
    Inherits PageBase

    Private ReadOnly _arbre As New TreeView()
    Private ReadOnly _txtLibelle As New TextBox() With {.Width = 200}
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Arbre (TreeView)", "L'arbre est reconstruit depuis la base (liste plate -> arbre) puis affiché. Ajoutez un enfant au nœud sélectionné.")
        Construire()
        Rafraichir()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(New Label() With {.Text = "Libellé :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_txtLibelle)
        haut.Controls.Add(Bouton("Ajouter (enfant du sélectionné / racine)", AddressOf SurAjouter))
        haut.Controls.Add(Bouton("Supprimer le sélectionné", AddressOf SurSupprimer))
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurRafraichir))
        haut.Controls.Add(_lblEtat)

        _arbre.Dock = DockStyle.Fill
        _arbre.HideSelection = False
        _arbre.Font = New Font("Segoe UI", 10.0F)

        Contenu.Controls.Add(_arbre)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub Rafraichir()
        Try
            _arbre.BeginUpdate()
            _arbre.Nodes.Clear()
            Dim racines As List(Of NoeudArbre) = ConstructeurArbre.Construire(DepotNoeud.Lister())
            For Each racine As NoeudArbre In racines
                _arbre.Nodes.Add(Convertir(racine))
            Next
            _arbre.ExpandAll()
            _arbre.EndUpdate()
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = ""
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Function Convertir(ByVal na As NoeudArbre) As TreeNode
        Dim tn As New TreeNode(na.Libelle) With {.Tag = na.Noeud.Id}
        For Each enfant As NoeudArbre In na.Enfants
            tn.Nodes.Add(Convertir(enfant))
        Next
        Return tn
    End Function

    Private Sub SurAjouter(ByVal sender As Object, ByVal e As EventArgs)
        If String.IsNullOrWhiteSpace(_txtLibelle.Text) Then
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Saisissez un libellé."
            Return
        End If
        Try
            Dim parent As Integer? = Nothing
            If _arbre.SelectedNode IsNot Nothing Then parent = CInt(_arbre.SelectedNode.Tag)
            DepotNoeud.Ajouter(parent, _txtLibelle.Text.Trim(), "")
            _txtLibelle.Clear()
            Rafraichir()
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurSupprimer(ByVal sender As Object, ByVal e As EventArgs)
        If _arbre.SelectedNode Is Nothing Then Return
        Try
            DepotNoeud.Supprimer(CInt(_arbre.SelectedNode.Tag))
            Rafraichir()
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurRafraichir(ByVal sender As Object, ByVal e As EventArgs)
        Rafraichir()
    End Sub

End Class

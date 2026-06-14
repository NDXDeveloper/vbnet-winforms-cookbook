' ============================================================================
'  PageGrille.vb  -  Demo de la GrillePersonnalisee (donnees en base).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.ControlesII

''' <summary>Affiche les articles dans la grille personnalisée (pré-stylée).</summary>
Public NotInheritable Class PageGrille
    Inherits PageBase

    Private ReadOnly _grille As New GrillePersonnalisee() With {.Dock = DockStyle.Fill}
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Grille (base)", "Un DataGridView dérivé, pré-stylé (en-tête coloré, lignes alternées), alimenté par la base.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))
        haut.Controls.Add(Bouton("Ajouter un article", AddressOf SurAjouter))
        haut.Controls.Add(_lblEtat)

        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotArticle.ListerTable()
            _lblEtat.Text = ""
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurAjouter(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim suffixe As String = DateTime.Now.ToString("HHmmss")
            DepotArticle.Ajouter("ART-" & suffixe, "Article " & suffixe, 9.9D, 10)
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Article ajouté."
            SurLister(sender, e)
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

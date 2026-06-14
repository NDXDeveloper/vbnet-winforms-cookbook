' ============================================================================
'  PageBibliotheque.vb  -  Liste et relit les PDF archives en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports NDX.Pdf

''' <summary>Affiche la bibliothèque des documents archivés et les recharge.</summary>
Public NotInheritable Class PageBibliotheque
    Inherits PageBase

    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Bibliothèque (base)", "Documents PDF archivés. Sélectionnez une ligne puis rechargez : le binaire est relu et ouvert dans la visionneuse.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))
        haut.Controls.Add(Bouton("Recharger et ouvrir", AddressOf SurRecharger))
        haut.Controls.Add(_lblEtat)

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.MultiSelect = False
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White

        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotDocument.Lister()
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = ""
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurRecharger(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If _grille.CurrentRow Is Nothing Then
                _lblEtat.ForeColor = Color.Firebrick
                _lblEtat.Text = "Sélectionnez d'abord une ligne."
                Return
            End If
            Dim id As Integer = Convert.ToInt32(_grille.CurrentRow.Cells("Id").Value)
            Dim pdf As Byte() = DepotDocument.Recharger(id)
            Dim chemin As String = Path.Combine(Path.GetTempPath(), "ndx-pdf-" & id.ToString() & ".pdf")
            File.WriteAllBytes(chemin, pdf)
            Process.Start(New ProcessStartInfo(chemin) With {.UseShellExecute = True})
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Document " & id.ToString() & " rechargé (" & pdf.Length.ToString("N0") & " octets) et ouvert."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

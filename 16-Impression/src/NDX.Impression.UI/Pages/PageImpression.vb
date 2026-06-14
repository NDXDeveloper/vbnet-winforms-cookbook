' ============================================================================
'  PageImpression.vb  -  Saisie, apercu, impression et archivage d'un document.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports NDX.Impression

''' <summary>Saisit un document, l'aperçoit, l'imprime et l'archive en base.</summary>
Public NotInheritable Class PageImpression
    Inherits PageBase

    Private Const LIGNES_PAR_PAGE_ESTIME As Integer = 50

    Private ReadOnly _txtTitre As New TextBox() With {.Width = 360, .Text = "Document de démonstration"}
    Private ReadOnly _txtCorps As New TextBox()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Aperçu & impression", "Saisissez un titre et un corps : aperçu paginé, impression réelle, ou archivage en base.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 36, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(New Label() With {.Text = "Titre :", .AutoSize = True, .Margin = New Padding(4, 8, 2, 0)})
        haut.Controls.Add(_txtTitre)

        _txtCorps.Multiline = True
        _txtCorps.ScrollBars = ScrollBars.Vertical
        _txtCorps.Dock = DockStyle.Fill
        _txtCorps.Font = New Font("Consolas", 9.5F)
        _txtCorps.Text = CorpsExemple()

        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Bottom, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        barre.Controls.Add(Bouton("Aperçu", AddressOf SurApercu))
        barre.Controls.Add(Bouton("Imprimer…", AddressOf SurImprimer))
        barre.Controls.Add(Bouton("Archiver en base", AddressOf SurArchiver))
        barre.Controls.Add(_lblEtat)

        Contenu.Controls.Add(_txtCorps)
        Contenu.Controls.Add(barre)
        Contenu.Controls.Add(haut)
    End Sub

    Private Function Lignes() As List(Of String)
        Return _txtCorps.Lines.ToList()
    End Function

    Private Sub SurApercu(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim imprimeur As New ImprimeurTexte(_txtTitre.Text, Lignes())
            Using doc As PrintDocument = imprimeur.CreerDocument()
                Using dlg As New PrintPreviewDialog() With {.Document = doc, .Width = 900, .Height = 700}
                    dlg.ShowDialog()
                End Using
            End Using
        Catch ex As Exception
            Avertir("Aperçu impossible : " & ex.Message)
        End Try
    End Sub

    Private Sub SurImprimer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim imprimeur As New ImprimeurTexte(_txtTitre.Text, Lignes())
            Using doc As PrintDocument = imprimeur.CreerDocument()
                Using dlg As New PrintDialog() With {.Document = doc}
                    If dlg.ShowDialog() = DialogResult.OK Then
                        doc.Print()
                        Avertir("Document envoyé à l'imprimante.", True)
                    End If
                End Using
            End Using
        Catch ex As Exception
            Avertir("Impression impossible : " & ex.Message)
        End Try
    End Sub

    Private Sub SurArchiver(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim pages As Integer = Paginateur.NombrePages(Lignes().Count, LIGNES_PAR_PAGE_ESTIME)
            Dim id As Integer = DepotImpression.Enregistrer(_txtTitre.Text, _txtCorps.Text, Math.Max(1, pages))
            Avertir("Archivé (id = " & id.ToString() & ", ~" & Math.Max(1, pages).ToString() & " page(s)).", True)
        Catch ex As Exception
            Avertir("Erreur : " & ex.Message)
        End Try
    End Sub

    Private Sub Avertir(ByVal message As String, Optional ByVal succes As Boolean = False)
        _lblEtat.ForeColor = If(succes, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

    Private Shared Function CorpsExemple() As String
        Dim sb As New StringBuilder()
        For i As Integer = 1 To 120
            sb.AppendLine("Ligne " & i.ToString("000") & " — contenu de démonstration pour l'aperçu et la pagination.")
        Next
        Return sb.ToString()
    End Function

End Class

' ============================================================================
'  PageDonnees.vb  -  Echanges base <-> .xlsx (export / import).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Office

''' <summary>Exporte la table en .xlsx et importe un .xlsx dans la table.</summary>
Public NotInheritable Class PageDonnees
    Inherits PageBase

    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Données (base ↔ xlsx)", "Exportez la table en classeur (sans Office) ou importez un classeur dans la table.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))
        haut.Controls.Add(Bouton("Ajouter une ligne", AddressOf SurAjouter))
        haut.Controls.Add(Bouton("Exporter la table → .xlsx", AddressOf SurExporter))
        haut.Controls.Add(Bouton("Importer .xlsx → table", AddressOf SurImporter))
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

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotClasseur.ListerTable()
            _lblEtat.Text = ""
        Catch ex As Exception
            Avertir("Erreur : " & ex.Message)
        End Try
    End Sub

    Private Sub SurAjouter(ByVal sender As Object, ByVal e As EventArgs)
        Try
            DepotClasseur.Ajouter("Article " & DateTime.Now.ToString("HHmmss"), "Qté " & DateTime.Now.Second.ToString(), "Prix " & DateTime.Now.Millisecond.ToString())
            Avertir("Ligne ajoutée.", True)
            SurLister(sender, e)
        Catch ex As Exception
            Avertir("Erreur : " & ex.Message)
        End Try
    End Sub

    Private Sub SurExporter(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New SaveFileDialog() With {.Filter = "Classeur Excel (*.xlsx)|*.xlsx", .FileName = "export.xlsx"}
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Try
                ClasseurXlsx.Ecrire(dlg.FileName, DepotClasseur.ListerLignes())
                Avertir("Table exportée : " & dlg.FileName, True)
            Catch ex As Exception
                Avertir("Erreur : " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub SurImporter(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New OpenFileDialog() With {.Filter = "Classeur Excel (*.xlsx)|*.xlsx"}
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Try
                Dim lignes As List(Of String()) = ClasseurXlsx.Lire(dlg.FileName)
                Dim n As Integer = DepotClasseur.Importer(lignes)
                Avertir(n.ToString() & " ligne(s) importée(s).", True)
                SurLister(sender, e)
            Catch ex As Exception
                Avertir("Erreur : " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub Avertir(ByVal message As String, Optional ByVal succes As Boolean = False)
        _lblEtat.ForeColor = If(succes, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

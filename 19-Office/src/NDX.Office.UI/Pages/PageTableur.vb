' ============================================================================
'  PageTableur.vb  -  Grille editable : ecrire / lire un .xlsx.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Office

''' <summary>Saisit un petit tableau, l'écrit en .xlsx (sans Office) et le relit.</summary>
Public NotInheritable Class PageTableur
    Inherits PageBase

    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Tableur (.xlsx)", "Éditez les cellules, écrivez un .xlsx (sans Office), relisez-le. L'export via Excel COM n'est proposé que si Excel est installé.")
        Construire()
        Prologue()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        haut.Controls.Add(Bouton("Écrire .xlsx (sans Office)", AddressOf SurEcrire))
        haut.Controls.Add(Bouton("Lire un .xlsx", AddressOf SurLire))
        haut.Controls.Add(Bouton("Exporter via Excel (COM)", AddressOf SurExcel))
        haut.Controls.Add(_lblEtat)

        _grille.Dock = DockStyle.Fill
        _grille.AllowUserToAddRows = True
        _grille.BackgroundColor = Color.White
        _grille.Columns.Add("A", "Colonne A")
        _grille.Columns.Add("B", "Colonne B")
        _grille.Columns.Add("C", "Colonne C")
        For Each col As DataGridViewColumn In _grille.Columns
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Next

        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub Prologue()
        _grille.Rows.Add("Produit", "Quantité", "Prix")
        _grille.Rows.Add("Café", "12", "4,50")
        _grille.Rows.Add("Thé", "8", "3,20")
        _grille.Rows.Add("Chocolat", "5", "6,90")
    End Sub

    Private Function LignesDeLaGrille() As List(Of String())
        Dim lignes As New List(Of String())()
        For Each ligne As DataGridViewRow In _grille.Rows
            If ligne.IsNewRow Then Continue For
            lignes.Add(New String() {Cellule(ligne, 0), Cellule(ligne, 1), Cellule(ligne, 2)})
        Next
        Return lignes
    End Function

    Private Shared Function Cellule(ByVal ligne As DataGridViewRow, ByVal index As Integer) As String
        Dim v As Object = ligne.Cells(index).Value
        Return If(v Is Nothing, "", Convert.ToString(v))
    End Function

    Private Sub SurEcrire(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New SaveFileDialog() With {.Filter = "Classeur Excel (*.xlsx)|*.xlsx", .FileName = "tableau.xlsx"}
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Try
                ClasseurXlsx.Ecrire(dlg.FileName, LignesDeLaGrille())
                Avertir("Écrit (sans Office) : " & dlg.FileName, True)
            Catch ex As Exception
                Avertir("Erreur : " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub SurLire(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New OpenFileDialog() With {.Filter = "Classeur Excel (*.xlsx)|*.xlsx"}
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Try
                Dim lignes As List(Of String()) = ClasseurXlsx.Lire(dlg.FileName)
                _grille.Rows.Clear()
                For Each l As String() In lignes
                    _grille.Rows.Add(Element(l, 0), Element(l, 1), Element(l, 2))
                Next
                Avertir(lignes.Count.ToString() & " ligne(s) lue(s).", True)
            Catch ex As Exception
                Avertir("Erreur : " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub SurExcel(ByVal sender As Object, ByVal e As EventArgs)
        If Not AutomationExcel.EstDisponible() Then
            Avertir("Microsoft Excel n'est pas installé : utilisez « Écrire .xlsx (sans Office) ».")
            Return
        End If
        Using dlg As New SaveFileDialog() With {.Filter = "Classeur Excel (*.xlsx)|*.xlsx", .FileName = "tableau-excel.xlsx"}
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Try
                AutomationExcel.Exporter(dlg.FileName, LignesDeLaGrille())
                Avertir("Exporté via Excel : " & dlg.FileName, True)
            Catch ex As Exception
                Avertir("Erreur Excel : " & ex.Message)
            End Try
        End Using
    End Sub

    Private Shared Function Element(ByVal tableau As String(), ByVal index As Integer) As String
        Return If(tableau IsNot Nothing AndAlso index < tableau.Length, tableau(index), "")
    End Function

    Private Sub Avertir(ByVal message As String, Optional ByVal succes As Boolean = False)
        _lblEtat.ForeColor = If(succes, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

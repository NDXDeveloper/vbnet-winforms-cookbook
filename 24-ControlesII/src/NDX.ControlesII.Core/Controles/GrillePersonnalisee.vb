' ============================================================================
'  GrillePersonnalisee.vb  -  DataGridView pre-style (lignes alternees, en-tete).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Grille de données dérivée de <see cref="DataGridView"/> avec un style par
''' défaut soigné (en-tête coloré, lignes alternées, lecture seule). Évite de
''' reconfigurer la même apparence dans chaque écran.
''' </summary>
Public Class GrillePersonnalisee
    Inherits DataGridView

    Public Sub New()
        Me.ReadOnly = True
        Me.AllowUserToAddRows = False
        Me.AllowUserToDeleteRows = False
        Me.AllowUserToResizeRows = False
        Me.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.MultiSelect = False
        Me.RowHeadersVisible = False
        Me.BorderStyle = BorderStyle.None
        Me.BackgroundColor = Color.White
        Me.EnableHeadersVisualStyles = False
        Me.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Me.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243)
        Me.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Me.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.5F)
        Me.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.ColumnHeadersHeight = 30
        Me.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250)
        Me.RowTemplate.Height = 26
    End Sub

End Class

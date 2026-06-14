' ============================================================================
'  ImprimeurTexte.vb  -  Impression d'un texte multi-pages (PrintDocument).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Linq

''' <summary>
''' Construit un <see cref="PrintDocument"/> qui imprime un texte ligne à ligne,
''' avec pagination automatique : à chaque page, on dessine autant de lignes que la
''' marge le permet, puis on signale s'il en reste (<c>HasMorePages</c>).
''' </summary>
Public NotInheritable Class ImprimeurTexte

    Private ReadOnly _titre As String
    Private ReadOnly _lignes As List(Of String)
    Private ReadOnly _police As Font
    Private _index As Integer

    Public Sub New(ByVal titre As String, ByVal lignes As IEnumerable(Of String), Optional ByVal police As Font = Nothing)
        _titre = If(titre, "")
        _lignes = If(lignes, Enumerable.Empty(Of String)()).ToList()
        _police = If(police, New Font("Consolas", 10.0F))
    End Sub

    ''' <summary>Crée le document d'impression (à confier à un PrintDialog ou PrintPreviewDialog).</summary>
    Public Function CreerDocument() As PrintDocument
        Dim doc As New PrintDocument()
        doc.DocumentName = If(String.IsNullOrEmpty(_titre), "Document", _titre)
        AddHandler doc.BeginPrint, Sub(sender, e) _index = 0
        AddHandler doc.PrintPage, AddressOf SurPage
        Return doc
    End Function

    Private Sub SurPage(ByVal sender As Object, ByVal e As PrintPageEventArgs)
        Dim hauteurLigne As Single = _police.GetHeight(e.Graphics)
        Dim x As Single = e.MarginBounds.Left
        Dim y As Single = e.MarginBounds.Top

        ' Titre sur la première page.
        If _index = 0 AndAlso _titre.Length > 0 Then
            Using gras As New Font(_police.FontFamily, _police.SizeInPoints + 4.0F, FontStyle.Bold)
                e.Graphics.DrawString(_titre, gras, Brushes.Black, x, y)
            End Using
            y += hauteurLigne * 2.0F
        End If

        Dim bas As Single = e.MarginBounds.Bottom
        While _index < _lignes.Count AndAlso (y + hauteurLigne) <= bas
            e.Graphics.DrawString(_lignes(_index), _police, Brushes.Black, x, y)
            y += hauteurLigne
            _index += 1
        End While

        e.HasMorePages = _index < _lignes.Count
    End Sub

End Class

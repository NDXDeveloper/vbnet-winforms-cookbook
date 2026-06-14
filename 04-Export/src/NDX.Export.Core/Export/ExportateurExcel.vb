' ============================================================================
'  ExportateurExcel.vb  -  Export d'un DataTable au format Excel .xlsx (OpenXML).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Text

''' <summary>
''' Exporte un <see cref="DataTable"/> en classeur Excel <c>.xlsx</c>. Un fichier
''' xlsx est une archive ZIP (format Office Open XML) contenant des parties XML.
''' On assemble ici les parties minimales, <b>sans Excel ni bibliotheque tierce</b> :
''' types de contenu, relations, classeur et une feuille. Les chaines sont ecrites
''' en ligne (<c>inlineStr</c>), les valeurs numeriques en nombres.
''' </summary>
Public NotInheritable Class ExportateurExcel
    Implements IExportateur

    ''' <summary>Nom de la feuille.</summary>
    Public Property NomFeuille As String = "Feuille1"

    Public ReadOnly Property Format As FormatExport Implements IExportateur.Format
        Get
            Return FormatExport.Excel
        End Get
    End Property
    Public ReadOnly Property Extension As String Implements IExportateur.Extension
        Get
            Return ".xlsx"
        End Get
    End Property
    Public ReadOnly Property TypeMime As String Implements IExportateur.TypeMime
        Get
            Return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        End Get
    End Property

    Public Function Exporter(ByVal table As DataTable) As Byte() Implements IExportateur.Exporter
        Using flux As New MemoryStream()
            Using archive As New ZipArchive(flux, ZipArchiveMode.Create, leaveOpen:=True)
                EcrireEntree(archive, "[Content_Types].xml", ContentTypes())
                EcrireEntree(archive, "_rels/.rels", RelationsRacine())
                EcrireEntree(archive, "xl/workbook.xml", Workbook())
                EcrireEntree(archive, "xl/_rels/workbook.xml.rels", RelationsWorkbook())
                EcrireEntree(archive, "xl/worksheets/sheet1.xml", Feuille(table))
            End Using
            Return flux.ToArray()
        End Using
    End Function

#Region "Parties XML"

    Private Function ContentTypes() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<Types xmlns=""http://schemas.openxmlformats.org/package/2006/content-types"">" &
            "<Default Extension=""rels"" ContentType=""application/vnd.openxmlformats-package.relationships+xml""/>" &
            "<Default Extension=""xml"" ContentType=""application/xml""/>" &
            "<Override PartName=""/xl/workbook.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml""/>" &
            "<Override PartName=""/xl/worksheets/sheet1.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml""/>" &
            "</Types>"
    End Function

    Private Function RelationsRacine() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">" &
            "<Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"" Target=""xl/workbook.xml""/>" &
            "</Relationships>"
    End Function

    Private Function Workbook() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<workbook xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" " &
            "xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"">" &
            "<sheets><sheet name=""" & EchapperXml(NomFeuille) & """ sheetId=""1"" r:id=""rId1""/></sheets></workbook>"
    End Function

    Private Function RelationsWorkbook() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">" &
            "<Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"" Target=""worksheets/sheet1.xml""/>" &
            "</Relationships>"
    End Function

    Private Function Feuille(ByVal table As DataTable) As String
        Dim sb As New StringBuilder()
        sb.Append("<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>")
        sb.Append("<worksheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main""><sheetData>")

        ' Ligne 1 : en-tetes.
        sb.Append("<row r=""1"">")
        For c As Integer = 0 To table.Columns.Count - 1
            sb.Append(CelluleTexte(Reference(c + 1, 1), table.Columns(c).ColumnName))
        Next
        sb.Append("</row>")

        ' Lignes de donnees.
        Dim r As Integer = 2
        For Each ligne As DataRow In table.Rows
            sb.Append("<row r=""" & r.ToString() & """>")
            For c As Integer = 0 To table.Columns.Count - 1
                Dim refCellule As String = Reference(c + 1, r)
                Dim valeur As Object = ligne(c)
                If EstNumerique(table.Columns(c).DataType) AndAlso valeur IsNot Nothing AndAlso Not Convert.IsDBNull(valeur) Then
                    sb.Append("<c r=""" & refCellule & """><v>" & NombreInvariant(valeur) & "</v></c>")
                Else
                    sb.Append(CelluleTexte(refCellule, ValeurTexte(valeur)))
                End If
            Next
            sb.Append("</row>")
            r += 1
        Next

        sb.Append("</sheetData></worksheet>")
        Return sb.ToString()
    End Function

#End Region

#Region "Outils"

    Private Sub EcrireEntree(ByVal archive As ZipArchive, ByVal nom As String, ByVal contenu As String)
        Dim entree As ZipArchiveEntry = archive.CreateEntry(nom, CompressionLevel.Optimal)
        Using w As New StreamWriter(entree.Open(), New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
            w.Write(contenu)
        End Using
    End Sub

    Private Function CelluleTexte(ByVal reference As String, ByVal texte As String) As String
        Return "<c r=""" & reference & """ t=""inlineStr""><is><t xml:space=""preserve"">" & EchapperXml(texte) & "</t></is></c>"
    End Function

    ' Reference de cellule : colonne 1-based + ligne 1-based -> "A1", "B2", "AA10"...
    Private Function Reference(ByVal colonne As Integer, ByVal ligne As Integer) As String
        Dim nom As String = ""
        Dim n As Integer = colonne
        While n > 0
            Dim reste As Integer = (n - 1) Mod 26
            nom = Chr(Asc("A"c) + reste) & nom
            n = (n - 1) \ 26
        End While
        Return nom & ligne.ToString()
    End Function

    Private Shared Function EstNumerique(ByVal t As Type) As Boolean
        Return t Is GetType(Byte) OrElse t Is GetType(Short) OrElse t Is GetType(Integer) OrElse
               t Is GetType(Long) OrElse t Is GetType(Decimal) OrElse t Is GetType(Double) OrElse t Is GetType(Single)
    End Function

    Private Function NombreInvariant(ByVal valeur As Object) As String
        Dim f As IFormattable = TryCast(valeur, IFormattable)
        If f IsNot Nothing Then Return f.ToString(Nothing, CultureInfo.InvariantCulture)
        Return Convert.ToString(valeur, CultureInfo.InvariantCulture)
    End Function

    Private Function ValeurTexte(ByVal valeur As Object) As String
        If valeur Is Nothing OrElse Convert.IsDBNull(valeur) Then Return ""
        Dim f As IFormattable = TryCast(valeur, IFormattable)
        If f IsNot Nothing Then Return f.ToString(Nothing, CultureInfo.InvariantCulture)
        Return valeur.ToString()
    End Function

    Private Function EchapperXml(ByVal texte As String) As String
        If texte Is Nothing Then Return ""
        Return texte.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;")
    End Function

#End Region

End Class

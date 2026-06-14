' ============================================================================
'  ClasseurXlsx.vb  -  Lecture / ecriture de classeurs .xlsx SANS Office (OpenXML).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Text
Imports System.Xml.Linq

''' <summary>
''' Lit et écrit des fichiers <c>.xlsx</c> <b>sans Excel</b> : un .xlsx est une
''' archive ZIP contenant des parties XML (OpenXML). On écrit les cellules en
''' « chaîne en ligne » (inlineStr) pour un aller-retour fidèle, et on sait relire
''' aussi les chaînes partagées (sharedStrings) des fichiers produits par Excel.
''' </summary>
Public NotInheritable Class ClasseurXlsx

    Private Sub New()
    End Sub

    Private Shared ReadOnly NS As XNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main"
    Private Shared ReadOnly _utf8 As New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)

    ' ---- Écriture ----

    ''' <summary>Écrit les lignes dans un fichier .xlsx.</summary>
    Public Shared Sub Ecrire(ByVal chemin As String, ByVal lignes As IEnumerable(Of String()))
        Using fs As New FileStream(chemin, FileMode.Create, FileAccess.Write)
            Ecrire(fs, lignes)
        End Using
    End Sub

    ''' <summary>Écrit les lignes dans un flux .xlsx (le flux reste ouvert).</summary>
    Public Shared Sub Ecrire(ByVal flux As Stream, ByVal lignes As IEnumerable(Of String()))
        Using zip As New ZipArchive(flux, ZipArchiveMode.Create, leaveOpen:=True)
            AjouterPartie(zip, "[Content_Types].xml", CONTENT_TYPES)
            AjouterPartie(zip, "_rels/.rels", RELS_RACINE)
            AjouterPartie(zip, "xl/workbook.xml", WORKBOOK)
            AjouterPartie(zip, "xl/_rels/workbook.xml.rels", RELS_WORKBOOK)
            AjouterPartie(zip, "xl/worksheets/sheet1.xml", ConstruireFeuille(lignes))
        End Using
    End Sub

    Private Shared Function ConstruireFeuille(ByVal lignes As IEnumerable(Of String())) As String
        Dim sb As New StringBuilder()
        sb.Append("<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>")
        sb.Append("<worksheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main""><sheetData>")
        Dim numeroLigne As Integer = 1
        For Each ligne As String() In lignes
            sb.Append("<row r=""").Append(numeroLigne).Append(""">")
            For colonne As Integer = 1 To ligne.Length
                Dim valeur As String = If(ligne(colonne - 1), "")
                Dim refA1 As String = ReferenceCellule.ColonneVersLettres(colonne) & numeroLigne.ToString()
                sb.Append("<c r=""").Append(refA1).Append(""" t=""inlineStr""><is><t xml:space=""preserve"">").
                   Append(EchapperXml(valeur)).Append("</t></is></c>")
            Next
            sb.Append("</row>")
            numeroLigne += 1
        Next
        sb.Append("</sheetData></worksheet>")
        Return sb.ToString()
    End Function

    ' ---- Lecture ----

    ''' <summary>Lit les lignes d'un fichier .xlsx.</summary>
    Public Shared Function Lire(ByVal chemin As String) As List(Of String())
        Using fs As New FileStream(chemin, FileMode.Open, FileAccess.Read)
            Return Lire(fs)
        End Using
    End Function

    ''' <summary>Lit les lignes d'un flux .xlsx (le flux reste ouvert).</summary>
    Public Shared Function Lire(ByVal flux As Stream) As List(Of String())
        Using zip As New ZipArchive(flux, ZipArchiveMode.Read, leaveOpen:=True)
            Dim partagees As List(Of String) = LireChainesPartagees(zip)
            Dim feuille As ZipArchiveEntry = zip.GetEntry("xl/worksheets/sheet1.xml")
            If feuille Is Nothing Then Return New List(Of String())()

            Dim doc As XDocument
            Using s As Stream = feuille.Open()
                doc = XDocument.Load(s)
            End Using

            ' Cellules : (ligne, colonne) -> valeur.
            Dim cellules As New Dictionary(Of Integer, Dictionary(Of Integer, String))()
            Dim maxColonne As Integer = 0
            Dim maxLigne As Integer = 0
            For Each c As XElement In doc.Descendants(NS + "c")
                Dim refA1 As String = CStr(c.Attribute("r"))
                If String.IsNullOrEmpty(refA1) Then Continue For
                Dim cellule As ReferenceCellule = ReferenceCellule.Analyser(refA1)
                Dim valeur As String = LireValeurCellule(c, partagees)
                If Not cellules.ContainsKey(cellule.Ligne) Then cellules(cellule.Ligne) = New Dictionary(Of Integer, String)()
                cellules(cellule.Ligne)(cellule.Colonne) = valeur
                maxColonne = Math.Max(maxColonne, cellule.Colonne)
                maxLigne = Math.Max(maxLigne, cellule.Ligne)
            Next

            Dim resultat As New List(Of String())()
            For l As Integer = 1 To maxLigne
                Dim ligne(maxColonne - 1) As String
                For col As Integer = 1 To maxColonne
                    Dim v As String = ""
                    If cellules.ContainsKey(l) AndAlso cellules(l).ContainsKey(col) Then v = cellules(l)(col)
                    ligne(col - 1) = v
                Next
                resultat.Add(ligne)
            Next
            Return resultat
        End Using
    End Function

    Private Shared Function LireValeurCellule(ByVal c As XElement, ByVal partagees As List(Of String)) As String
        Dim type As String = CStr(c.Attribute("t"))
        If type = "inlineStr" Then
            Dim elemT As XElement = c.Descendants(NS + "t").FirstOrDefault()
            Return If(elemT Is Nothing, "", elemT.Value)
        End If
        Dim v As XElement = c.Element(NS + "v")
        If v Is Nothing Then Return ""
        If type = "s" Then
            Dim index As Integer
            If Integer.TryParse(v.Value, index) AndAlso index >= 0 AndAlso index < partagees.Count Then Return partagees(index)
            Return ""
        End If
        Return v.Value
    End Function

    Private Shared Function LireChainesPartagees(ByVal zip As ZipArchive) As List(Of String)
        Dim liste As New List(Of String)()
        Dim entree As ZipArchiveEntry = zip.GetEntry("xl/sharedStrings.xml")
        If entree Is Nothing Then Return liste
        Dim doc As XDocument
        Using s As Stream = entree.Open()
            doc = XDocument.Load(s)
        End Using
        For Each si As XElement In doc.Descendants(NS + "si")
            ' Concatène les fragments de texte (gère le texte enrichi).
            liste.Add(String.Concat(si.Descendants(NS + "t").Select(Function(t) t.Value)))
        Next
        Return liste
    End Function

    ' ---- Utilitaires ----

    Private Shared Sub AjouterPartie(ByVal zip As ZipArchive, ByVal nom As String, ByVal contenu As String)
        Dim entree As ZipArchiveEntry = zip.CreateEntry(nom)
        Using flux As Stream = entree.Open()
            Dim octets As Byte() = _utf8.GetBytes(contenu)
            flux.Write(octets, 0, octets.Length)
        End Using
    End Sub

    Private Shared Function EchapperXml(ByVal s As String) As String
        Return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
    End Function

    Private Const CONTENT_TYPES As String =
        "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
        "<Types xmlns=""http://schemas.openxmlformats.org/package/2006/content-types"">" &
        "<Default Extension=""rels"" ContentType=""application/vnd.openxmlformats-package.relationships+xml""/>" &
        "<Default Extension=""xml"" ContentType=""application/xml""/>" &
        "<Override PartName=""/xl/workbook.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml""/>" &
        "<Override PartName=""/xl/worksheets/sheet1.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml""/>" &
        "</Types>"

    Private Const RELS_RACINE As String =
        "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
        "<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">" &
        "<Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"" Target=""xl/workbook.xml""/>" &
        "</Relationships>"

    Private Const WORKBOOK As String =
        "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
        "<workbook xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"">" &
        "<sheets><sheet name=""Feuille1"" sheetId=""1"" r:id=""rId1""/></sheets></workbook>"

    Private Const RELS_WORKBOOK As String =
        "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
        "<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">" &
        "<Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"" Target=""worksheets/sheet1.xml""/>" &
        "</Relationships>"

End Class

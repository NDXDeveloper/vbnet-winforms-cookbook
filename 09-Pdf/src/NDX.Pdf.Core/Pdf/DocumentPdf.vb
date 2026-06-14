' ============================================================================
'  DocumentPdf.vb  -  Assemble un fichier PDF complet (multi-pages, sans dependance).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text

''' <summary>
''' Document PDF composé en mémoire, puis sérialisé en octets. Implémente la
''' structure minimale d'un PDF : en-tête, objets (catalogue, arbre de pages,
''' polices, pages, flux de contenu, dictionnaire d'informations), table de
''' références croisées (<c>xref</c>) et <c>trailer</c>. Aucune bibliothèque
''' externe : tout est généré ici.
''' </summary>
Public NotInheritable Class DocumentPdf

    ''' <summary>Largeur d'une page A4, en points.</summary>
    Public Const A4Largeur As Double = 595.28
    ''' <summary>Hauteur d'une page A4, en points.</summary>
    Public Const A4Hauteur As Double = 841.89

    Private ReadOnly _pages As New List(Of PagePdf)()

    ''' <summary>Titre du document (métadonnée).</summary>
    Public Property Titre As String = "Document"

    ''' <summary>Auteur du document (métadonnée).</summary>
    Public Property Auteur As String = "NDX.Pdf"

    ''' <summary>Nombre de pages.</summary>
    Public ReadOnly Property NombrePages As Integer
        Get
            Return _pages.Count
        End Get
    End Property

    ''' <summary>Ajoute une page (A4 par défaut) et la renvoie pour y dessiner.</summary>
    Public Function AjouterPage(Optional ByVal largeur As Double = A4Largeur,
                                Optional ByVal hauteur As Double = A4Hauteur) As PagePdf
        Dim page As New PagePdf(largeur, hauteur)
        _pages.Add(page)
        Return page
    End Function

    ''' <summary>Sérialise le document en un tableau d'octets PDF valide.</summary>
    Public Function Construire() As Byte()
        If _pages.Count = 0 Then AjouterPage()
        Dim enc As Encoding = Encoding.GetEncoding(1252)   ' Windows-1252 (Latin occidental)
        Dim nbFonts As Integer = PolicesPdf.Nombre
        Dim baseObjPages As Integer = 3 + nbFonts          ' 1=catalogue, 2=arbre, 3..=polices
        Dim objInfo As Integer = baseObjPages + 2 * _pages.Count
        Dim nbObjets As Integer = objInfo

        Dim sb As New StringBuilder()
        Dim offsets(nbObjets) As Integer                   ' offsets(n) = octet de l'objet n

        sb.Append("%PDF-1.4").Append(vbLf)
        sb.Append("%").Append(ChrW(&HE2)).Append(ChrW(&HE3)).Append(ChrW(&HCF)).Append(ChrW(&HD3)).Append(vbLf)

        ' --- Objet 1 : catalogue ---
        DebutObjet(sb, offsets, enc, 1)
        sb.Append("<< /Type /Catalog /Pages 2 0 R >>").Append(vbLf)
        FinObjet(sb)

        ' --- Objet 2 : arbre des pages ---
        DebutObjet(sb, offsets, enc, 2)
        sb.Append("<< /Type /Pages /Kids [ ")
        For i As Integer = 0 To _pages.Count - 1
            sb.Append(baseObjPages + 2 * i).Append(" 0 R ")
        Next
        sb.Append("] /Count ").Append(_pages.Count).Append(" >>").Append(vbLf)
        FinObjet(sb)

        ' --- Objets 3..(2+nbFonts) : polices standard ---
        For p As Integer = 0 To nbFonts - 1
            DebutObjet(sb, offsets, enc, 3 + p)
            sb.Append("<< /Type /Font /Subtype /Type1 /BaseFont /").
               Append(PolicesPdf.NomBaseFont(CType(p, PoliceStandard))).
               Append(" /Encoding /WinAnsiEncoding >>").Append(vbLf)
            FinObjet(sb)
        Next

        ' Dictionnaire de ressources « polices », commun a toutes les pages.
        Dim ressourcesPolices As String = ConstruireRessourcesPolices(nbFonts)

        ' --- Pages + flux de contenu ---
        For i As Integer = 0 To _pages.Count - 1
            Dim page As PagePdf = _pages(i)
            Dim numPage As Integer = baseObjPages + 2 * i
            Dim numContenu As Integer = numPage + 1

            DebutObjet(sb, offsets, enc, numPage)
            sb.Append("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 ").
               Append(Fmt(page.Largeur)).Append(" ").Append(Fmt(page.Hauteur)).Append("] ").
               Append("/Resources << /Font << ").Append(ressourcesPolices).Append(" >> >> ").
               Append("/Contents ").Append(numContenu).Append(" 0 R >>").Append(vbLf)
            FinObjet(sb)

            Dim flux As String = page.FluxContenu
            DebutObjet(sb, offsets, enc, numContenu)
            sb.Append("<< /Length ").Append(enc.GetByteCount(flux)).Append(" >>").Append(vbLf)
            sb.Append("stream").Append(vbLf)
            sb.Append(flux)
            sb.Append("endstream").Append(vbLf)
            FinObjet(sb)
        Next

        ' --- Dictionnaire d'informations (metadonnees) ---
        DebutObjet(sb, offsets, enc, objInfo)
        sb.Append("<< /Title (").Append(EchapperMeta(Titre)).Append(") /Author (").Append(EchapperMeta(Auteur)).
           Append(") /Producer (NDX.Pdf) /Creator (NDX.Pdf) >>").Append(vbLf)
        FinObjet(sb)

        ' --- Table xref ---
        Dim octetXref As Integer = enc.GetByteCount(sb.ToString())
        sb.Append("xref").Append(vbLf)
        sb.Append("0 ").Append(nbObjets + 1).Append(vbLf)
        sb.Append("0000000000 65535 f").Append(vbCrLf)
        For n As Integer = 1 To nbObjets
            sb.Append(offsets(n).ToString("0000000000")).Append(" 00000 n").Append(vbCrLf)
        Next

        ' --- Trailer ---
        sb.Append("trailer").Append(vbLf)
        sb.Append("<< /Size ").Append(nbObjets + 1).Append(" /Root 1 0 R /Info ").Append(objInfo).Append(" 0 R >>").Append(vbLf)
        sb.Append("startxref").Append(vbLf)
        sb.Append(octetXref).Append(vbLf)
        sb.Append("%%EOF")

        Return enc.GetBytes(sb.ToString())
    End Function

    Private Shared Function ConstruireRessourcesPolices(ByVal nbFonts As Integer) As String
        Dim sb As New StringBuilder()
        For p As Integer = 0 To nbFonts - 1
            sb.Append("/").Append(PolicesPdf.Reference(CType(p, PoliceStandard))).
               Append(" ").Append(3 + p).Append(" 0 R ")
        Next
        Return sb.ToString().TrimEnd()
    End Function

    Private Shared Sub DebutObjet(ByVal sb As StringBuilder, ByVal offsets As Integer(), ByVal enc As Encoding, ByVal numero As Integer)
        offsets(numero) = enc.GetByteCount(sb.ToString())
        sb.Append(numero).Append(" 0 obj").Append(vbLf)
    End Sub

    Private Shared Sub FinObjet(ByVal sb As StringBuilder)
        sb.Append("endobj").Append(vbLf)
    End Sub

    Private Shared Function Fmt(ByVal valeur As Double) As String
        Return valeur.ToString("0.###", CultureInfo.InvariantCulture)
    End Function

    Private Shared Function EchapperMeta(ByVal texte As String) As String
        If texte Is Nothing Then Return ""
        Return texte.Replace("\", "\\").Replace("(", "\(").Replace(")", "\)")
    End Function

End Class

' ============================================================================
'  ExportateurPdf.vb  -  Export d'un DataTable en PDF (genere a la main).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Text

''' <summary>
''' Exporte un <see cref="DataTable"/> en PDF minimal, <b>sans bibliotheque tierce</b> :
''' un en-tete <c>%PDF</c>, des objets (catalogue, pages, page(s), police Courier,
''' flux de contenu), une table de references croisees (<c>xref</c>) et un
''' <c>trailer</c>. Le tableau est rendu en texte a chasse fixe (Courier), pagine.
''' </summary>
''' <remarks>
''' Objectif pedagogique : montrer la structure d'un fichier PDF. Le rendu est
''' volontairement simple (texte tabule, pas de styles). Les caracteres non-ASCII
''' sont remplaces par '?' (la police de base Courier utilise un encodage standard).
''' </remarks>
Public NotInheritable Class ExportateurPdf
    Implements IExportateur

    Private Const LIGNES_PAR_PAGE As Integer = 60
    Private Const LARGEUR_MAX_COLONNE As Integer = 30

    ''' <summary>Titre affiche en premiere ligne du document.</summary>
    Public Property Titre As String = "Export"

    Public ReadOnly Property Format As FormatExport Implements IExportateur.Format
        Get
            Return FormatExport.Pdf
        End Get
    End Property
    Public ReadOnly Property Extension As String Implements IExportateur.Extension
        Get
            Return ".pdf"
        End Get
    End Property
    Public ReadOnly Property TypeMime As String Implements IExportateur.TypeMime
        Get
            Return "application/pdf"
        End Get
    End Property

    Public Function Exporter(ByVal table As DataTable) As Byte() Implements IExportateur.Exporter
        Dim lignes As List(Of String) = ConstruireLignes(table)
        Dim pages As List(Of List(Of String)) = Paginer(lignes)

        Dim nbPages As Integer = pages.Count
        Dim nbObjets As Integer = 3 + 2 * nbPages          ' catalogue, pages, police + (page+contenu) x N
        Dim offsets(nbObjets) As Long                       ' offsets(1..nbObjets)
        Dim encodage As Encoding = Encoding.ASCII

        Using ms As New MemoryStream()
            Dim ecrire As Action(Of String) =
                Sub(s)
                    Dim b As Byte() = encodage.GetBytes(s)
                    ms.Write(b, 0, b.Length)
                End Sub

            ecrire("%PDF-1.4" & vbLf)

            ' Objet 1 : catalogue.
            offsets(1) = ms.Position
            ecrire("1 0 obj" & vbLf & "<< /Type /Catalog /Pages 2 0 R >>" & vbLf & "endobj" & vbLf)

            ' Objet 2 : noeud Pages.
            Dim kids As New StringBuilder()
            For i As Integer = 0 To nbPages - 1
                kids.Append((4 + 2 * i).ToString()).Append(" 0 R ")
            Next
            offsets(2) = ms.Position
            ecrire("2 0 obj" & vbLf & "<< /Type /Pages /Kids [ " & kids.ToString() & "] /Count " & nbPages.ToString() & " >>" & vbLf & "endobj" & vbLf)

            ' Objet 3 : police Courier.
            offsets(3) = ms.Position
            ecrire("3 0 obj" & vbLf & "<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>" & vbLf & "endobj" & vbLf)

            ' Objets pages + flux de contenu.
            For i As Integer = 0 To nbPages - 1
                Dim objPage As Integer = 4 + 2 * i
                Dim objContenu As Integer = 5 + 2 * i

                offsets(objPage) = ms.Position
                ecrire(objPage.ToString() & " 0 obj" & vbLf &
                       "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] " &
                       "/Resources << /Font << /F1 3 0 R >> >> /Contents " & objContenu.ToString() & " 0 R >>" & vbLf &
                       "endobj" & vbLf)

                Dim contenu As String = ConstruireFluxContenu(pages(i))
                Dim octets As Byte() = encodage.GetBytes(contenu)
                offsets(objContenu) = ms.Position
                ecrire(objContenu.ToString() & " 0 obj" & vbLf & "<< /Length " & octets.Length.ToString() & " >>" & vbLf & "stream" & vbLf)
                ms.Write(octets, 0, octets.Length)
                ecrire(vbLf & "endstream" & vbLf & "endobj" & vbLf)
            Next

            ' Table de references croisees.
            Dim posXref As Long = ms.Position
            ecrire("xref" & vbLf & "0 " & (nbObjets + 1).ToString() & vbLf)
            ecrire("0000000000 65535 f " & vbLf)
            For n As Integer = 1 To nbObjets
                ecrire(offsets(n).ToString("D10", CultureInfo.InvariantCulture) & " 00000 n " & vbLf)
            Next

            ecrire("trailer" & vbLf & "<< /Size " & (nbObjets + 1).ToString() & " /Root 1 0 R >>" & vbLf)
            ecrire("startxref" & vbLf & posXref.ToString(CultureInfo.InvariantCulture) & vbLf & "%%EOF")

            Return ms.ToArray()
        End Using
    End Function

#Region "Mise en page texte"

    Private Function ConstruireLignes(ByVal table As DataTable) As List(Of String)
        Dim n As Integer = table.Columns.Count
        Dim largeurs(Math.Max(0, n - 1)) As Integer
        For c As Integer = 0 To n - 1
            largeurs(c) = Math.Min(LARGEUR_MAX_COLONNE, Math.Max(3, table.Columns(c).ColumnName.Length))
        Next
        For Each ligne As DataRow In table.Rows
            For c As Integer = 0 To n - 1
                largeurs(c) = Math.Min(LARGEUR_MAX_COLONNE, Math.Max(largeurs(c), ValeurTexte(ligne(c)).Length))
            Next
        Next

        Dim lignes As New List(Of String)()
        If Not String.IsNullOrEmpty(Titre) Then
            lignes.Add(Titre)
            lignes.Add("")
        End If

        Dim entetes As New List(Of String)()
        For c As Integer = 0 To n - 1
            entetes.Add(table.Columns(c).ColumnName)
        Next
        lignes.Add(FormaterLigne(entetes, largeurs))

        Dim total As Integer = 0
        For Each l As Integer In largeurs
            total += l + 2
        Next
        lignes.Add(New String("-"c, Math.Max(1, total)))

        For Each ligne As DataRow In table.Rows
            Dim cellules As New List(Of String)()
            For c As Integer = 0 To n - 1
                cellules.Add(ValeurTexte(ligne(c)))
            Next
            lignes.Add(FormaterLigne(cellules, largeurs))
        Next
        Return lignes
    End Function

    Private Function FormaterLigne(ByVal valeurs As List(Of String), ByVal largeurs As Integer()) As String
        Dim sb As New StringBuilder()
        For c As Integer = 0 To valeurs.Count - 1
            Dim largeur As Integer = If(c < largeurs.Length, largeurs(c), 10)
            Dim v As String = If(valeurs(c), "")
            If v.Length > largeur Then v = v.Substring(0, largeur)
            sb.Append(v.PadRight(largeur))
            If c < valeurs.Count - 1 Then sb.Append("  ")
        Next
        Return sb.ToString()
    End Function

    Private Function Paginer(ByVal lignes As List(Of String)) As List(Of List(Of String))
        Dim pages As New List(Of List(Of String))()
        Dim courante As New List(Of String)()
        For Each l As String In lignes
            courante.Add(l)
            If courante.Count >= LIGNES_PAR_PAGE Then
                pages.Add(courante)
                courante = New List(Of String)()
            End If
        Next
        If courante.Count > 0 OrElse pages.Count = 0 Then pages.Add(courante)
        Return pages
    End Function

    Private Function ConstruireFluxContenu(ByVal lignes As List(Of String)) As String
        Dim sb As New StringBuilder()
        sb.Append("BT" & vbLf & "/F1 9 Tf" & vbLf & "11 TL" & vbLf & "40 800 Td" & vbLf)
        For Each l As String In lignes
            sb.Append("(" & EchapperPdf(l) & ") Tj T*" & vbLf)
        Next
        sb.Append("ET")
        Return sb.ToString()
    End Function

#End Region

#Region "Outils"

    Private Function ValeurTexte(ByVal valeur As Object) As String
        If valeur Is Nothing OrElse Convert.IsDBNull(valeur) Then Return ""
        Dim f As IFormattable = TryCast(valeur, IFormattable)
        If f IsNot Nothing Then Return f.ToString(Nothing, CultureInfo.InvariantCulture)
        Return valeur.ToString()
    End Function

    ' Echappe les caracteres speciaux d'une chaine PDF et neutralise le non-ASCII.
    Private Function EchapperPdf(ByVal texte As String) As String
        If texte Is Nothing Then Return ""
        Dim sb As New StringBuilder(texte.Length)
        For Each ch As Char In texte
            Select Case ch
                Case "\"c : sb.Append("\\")
                Case "("c : sb.Append("\(")
                Case ")"c : sb.Append("\)")
                Case Else
                    If AscW(ch) >= 32 AndAlso AscW(ch) <= 126 Then
                        sb.Append(ch)
                    Else
                        sb.Append("?"c)
                    End If
            End Select
        Next
        Return sb.ToString()
    End Function

#End Region

End Class

' ============================================================================
'  ExportateurCsv.vb  -  Export d'un DataTable au format CSV.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports System.Globalization
Imports System.Text

''' <summary>
''' Exporte un <see cref="DataTable"/> en CSV. Echappement conforme a l'usage
''' (RFC 4180) : un champ contenant le separateur, un guillemet ou un saut de
''' ligne est entoure de guillemets, les guillemets internes etant doubles.
''' </summary>
Public NotInheritable Class ExportateurCsv
    Implements IExportateur

    ''' <summary>Separateur de champs (";" par defaut, attendu par Excel en locale FR).</summary>
    Public Property Separateur As String = ";"
    ''' <summary>Ecrire la ligne d'en-tetes (noms de colonnes).</summary>
    Public Property AvecEntetes As Boolean = True
    ''' <summary>Prefixer d'un BOM UTF-8 (aide Excel a detecter l'encodage).</summary>
    Public Property AvecBom As Boolean = True

    Public ReadOnly Property Format As FormatExport Implements IExportateur.Format
        Get
            Return FormatExport.Csv
        End Get
    End Property
    Public ReadOnly Property Extension As String Implements IExportateur.Extension
        Get
            Return ".csv"
        End Get
    End Property
    Public ReadOnly Property TypeMime As String Implements IExportateur.TypeMime
        Get
            Return "text/csv"
        End Get
    End Property

    Public Function Exporter(ByVal table As DataTable) As Byte() Implements IExportateur.Exporter
        Dim sb As New StringBuilder()

        If AvecEntetes Then
            Dim entetes As New List(Of String)()
            For Each colonne As DataColumn In table.Columns
                entetes.Add(Echapper(colonne.ColumnName))
            Next
            sb.AppendLine(String.Join(Separateur, entetes))
        End If

        For Each ligne As DataRow In table.Rows
            Dim cellules As New List(Of String)()
            For Each colonne As DataColumn In table.Columns
                cellules.Add(Echapper(ValeurTexte(ligne(colonne))))
            Next
            sb.AppendLine(String.Join(Separateur, cellules))
        Next

        Dim encodage As New UTF8Encoding(encoderShouldEmitUTF8Identifier:=AvecBom)
        Return encodage.GetBytes(sb.ToString())
    End Function

    ' Conversion neutre d'une valeur de cellule en texte.
    Private Function ValeurTexte(ByVal valeur As Object) As String
        If valeur Is Nothing OrElse Convert.IsDBNull(valeur) Then Return ""
        Dim f As IFormattable = TryCast(valeur, IFormattable)
        If f IsNot Nothing Then Return f.ToString(Nothing, CultureInfo.InvariantCulture)
        Return valeur.ToString()
    End Function

    ' Entoure de guillemets si necessaire ; double les guillemets internes.
    Private Function Echapper(ByVal valeur As String) As String
        If valeur Is Nothing Then valeur = ""
        Dim doitEntourer As Boolean =
            valeur.Contains(Separateur) OrElse valeur.Contains("""") OrElse
            valeur.Contains(vbCr) OrElse valeur.Contains(vbLf)
        If Not doitEntourer Then Return valeur
        Return """" & valeur.Replace("""", """""") & """"
    End Function

End Class

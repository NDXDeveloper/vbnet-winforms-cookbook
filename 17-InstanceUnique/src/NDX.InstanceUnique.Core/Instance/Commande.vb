' ============================================================================
'  Commande.vb  -  Encodage / decodage (pur) d'une commande (liste d'arguments).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

''' <summary>
''' Sérialise une liste d'arguments en une seule ligne (et inversement), pour la
''' transmettre d'une instance à l'autre. Les caractères spéciaux (séparateur,
''' antislash, retours à la ligne) sont échappés afin de préserver fidèlement
''' chaque argument. Logique pure : testable sans aucune communication.
''' </summary>
Public NotInheritable Class Commande

    Private Sub New()
    End Sub

    Private Const SEPARATEUR As Char = "|"c

    ''' <summary>Encode une liste d'arguments en une ligne.</summary>
    Public Shared Function Encoder(ByVal arguments As IEnumerable(Of String)) As String
        If arguments Is Nothing Then Return ""
        Return String.Join(SEPARATEUR, arguments.Select(AddressOf Echapper))
    End Function

    ''' <summary>Décode une ligne en liste d'arguments.</summary>
    Public Shared Function Decoder(ByVal ligne As String) As List(Of String)
        Dim resultat As New List(Of String)()
        If String.IsNullOrEmpty(ligne) Then Return resultat
        For Each morceau As String In ligne.Split(SEPARATEUR)
            resultat.Add(Desechapper(morceau))
        Next
        Return resultat
    End Function

    Private Shared Function Echapper(ByVal s As String) As String
        If s Is Nothing Then Return ""
        Dim sb As New StringBuilder(s.Length + 4)
        For Each c As Char In s
            Select Case c
                Case "\"c : sb.Append("\\")
                Case SEPARATEUR : sb.Append("\p")
                Case ChrW(13) : sb.Append("\r")
                Case ChrW(10) : sb.Append("\n")
                Case Else : sb.Append(c)
            End Select
        Next
        Return sb.ToString()
    End Function

    Private Shared Function Desechapper(ByVal s As String) As String
        Dim sb As New StringBuilder(s.Length)
        Dim i As Integer = 0
        While i < s.Length
            Dim c As Char = s(i)
            If c = "\"c AndAlso i + 1 < s.Length Then
                Dim suivant As Char = s(i + 1)
                Select Case suivant
                    Case "\"c : sb.Append("\"c)
                    Case "p"c : sb.Append(SEPARATEUR)
                    Case "r"c : sb.Append(ChrW(13))
                    Case "n"c : sb.Append(ChrW(10))
                    Case Else : sb.Append(suivant)
                End Select
                i += 2
            Else
                sb.Append(c)
                i += 1
            End If
        End While
        Return sb.ToString()
    End Function

End Class

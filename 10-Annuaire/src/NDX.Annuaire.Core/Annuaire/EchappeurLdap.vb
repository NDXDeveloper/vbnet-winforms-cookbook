' ============================================================================
'  EchappeurLdap.vb  -  Echappement des valeurs dans un filtre LDAP (RFC 4515).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Text

''' <summary>
''' Échappe une valeur insérée dans un filtre de recherche LDAP. Sans cela, un
''' identifiant contenant <c>*</c>, <c>(</c>, <c>)</c> ou <c>\</c> permettrait une
''' <b>injection LDAP</b> (équivalent de l'injection SQL). Conforme à la RFC 4515 :
''' chaque caractère spécial devient <c>\</c> suivi de son code hexadécimal.
''' </summary>
Public NotInheritable Class EchappeurLdap

    Private Sub New()
    End Sub

    ''' <summary>Échappe une valeur destinée à un filtre LDAP.</summary>
    Public Shared Function EchapperFiltre(ByVal valeur As String) As String
        If valeur Is Nothing Then Return ""
        Dim sb As New StringBuilder(valeur.Length + 8)
        For Each c As Char In valeur
            Select Case c
                Case "\"c : sb.Append("\5c")
                Case "*"c : sb.Append("\2a")
                Case "("c : sb.Append("\28")
                Case ")"c : sb.Append("\29")
                Case ChrW(0) : sb.Append("\00")
                Case Else : sb.Append(c)
            End Select
        Next
        Return sb.ToString()
    End Function

End Class

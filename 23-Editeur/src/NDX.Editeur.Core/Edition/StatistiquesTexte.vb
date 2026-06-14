' ============================================================================
'  StatistiquesTexte.vb  -  Comptage (pur) de mots, caracteres et lignes.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>
''' Statistiques simples d'un texte (mots, caractères, lignes). Calcul purement
''' textuel, indépendant du contrôle d'édition — donc testable directement.
''' </summary>
Public NotInheritable Class StatistiquesTexte

    Private Shared ReadOnly SEPARATEURS As Char() = {" "c, ChrW(9), ChrW(10), ChrW(13)}

    Public Sub New(ByVal texte As String)
        Dim t As String = If(texte, "")
        Caracteres = t.Length
        Dim sansEspaces As Integer = 0
        For Each c As Char In t
            If Not Char.IsWhiteSpace(c) Then sansEspaces += 1
        Next
        CaracteresSansEspaces = sansEspaces
        Mots = t.Split(SEPARATEURS, StringSplitOptions.RemoveEmptyEntries).Length
        If t.Length = 0 Then
            Lignes = 0
        Else
            Lignes = t.Replace(vbCrLf, vbLf).Split(ChrW(10)).Length
        End If
    End Sub

    Public ReadOnly Property Caracteres As Integer
    Public ReadOnly Property CaracteresSansEspaces As Integer
    Public ReadOnly Property Mots As Integer
    Public ReadOnly Property Lignes As Integer

    ''' <summary>Calcule les statistiques d'un texte.</summary>
    Public Shared Function Analyser(ByVal texte As String) As StatistiquesTexte
        Return New StatistiquesTexte(texte)
    End Function

    Public Overrides Function ToString() As String
        Return Mots.ToString() & " mot(s) · " & Caracteres.ToString() & " caractère(s) · " & Lignes.ToString() & " ligne(s)"
    End Function

End Class

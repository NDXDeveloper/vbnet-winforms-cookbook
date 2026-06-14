' ============================================================================
'  Theme.vb  -  Un theme : couleurs de fond, de texte et d'accent.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing

''' <summary>Jeu de couleurs d'un thème visuel (fond, texte, accent).</summary>
Public NotInheritable Class Theme

    Public Sub New(ByVal nom As String, ByVal fond As Color, ByVal texte As Color, ByVal accent As Color)
        Me.Nom = nom
        Me.Fond = fond
        Me.Texte = texte
        Me.Accent = accent
    End Sub

    Public ReadOnly Property Nom As String
    Public ReadOnly Property Fond As Color
    Public ReadOnly Property Texte As Color
    Public ReadOnly Property Accent As Color

    Public Overrides Function ToString() As String
        Return Nom
    End Function

End Class

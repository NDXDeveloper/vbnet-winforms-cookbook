' ============================================================================
'  GestionnaireThemes.vb  -  Themes predefinis + application recursive aux controles.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Fournit des thèmes prédéfinis et les applique à une arborescence de contrôles
''' (fond + texte propagés récursivement). C'est le cœur d'un « mode sombre ».
''' </summary>
Public NotInheritable Class GestionnaireThemes

    Private Sub New()
    End Sub

    ''' <summary>Thème clair (par défaut).</summary>
    Public Shared ReadOnly Property Clair As New Theme("Clair", Color.White, Color.FromArgb(33, 33, 33), Color.FromArgb(57, 73, 171))

    ''' <summary>Thème sombre.</summary>
    Public Shared ReadOnly Property Sombre As New Theme("Sombre", Color.FromArgb(37, 37, 38), Color.Gainsboro, Color.FromArgb(124, 77, 255))

    ''' <summary>Liste des thèmes prédéfinis.</summary>
    Public Shared Function Predefinis() As List(Of Theme)
        Return New List(Of Theme) From {Clair, Sombre}
    End Function

    ''' <summary>Applique récursivement le fond et la couleur de texte d'un thème à un contrôle et ses enfants.</summary>
    Public Shared Sub AppliquerSur(ByVal racine As Control, ByVal theme As Theme)
        If racine Is Nothing OrElse theme Is Nothing Then Return
        racine.BackColor = theme.Fond
        racine.ForeColor = theme.Texte
        For Each enfant As Control In racine.Controls
            AppliquerSur(enfant, theme)
        Next
    End Sub

End Class

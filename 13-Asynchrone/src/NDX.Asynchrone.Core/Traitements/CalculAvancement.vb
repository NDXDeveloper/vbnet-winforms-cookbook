' ============================================================================
'  CalculAvancement.vb  -  Calcul (pur) du pourcentage d'avancement.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Arithmétique de l'avancement, isolée pour être testable sans tâche réelle.</summary>
Public NotInheritable Class CalculAvancement

    Private Sub New()
    End Sub

    ''' <summary>Pourcentage entier (0..100) pour <paramref name="fait"/> éléments sur <paramref name="total"/>.</summary>
    Public Shared Function Pourcentage(ByVal fait As Integer, ByVal total As Integer) As Integer
        If total <= 0 Then Return 0
        Dim p As Integer = CInt(Math.Round(100.0 * fait / total, MidpointRounding.AwayFromZero))
        Return Math.Max(0, Math.Min(100, p))
    End Function

End Class

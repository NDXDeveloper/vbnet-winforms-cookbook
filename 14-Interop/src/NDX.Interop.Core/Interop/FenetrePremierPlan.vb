' ============================================================================
'  FenetrePremierPlan.vb  -  "Toujours au premier plan" via SetWindowPos.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Épingle / désépingle une fenêtre au premier plan (sans la déplacer ni la redimensionner).</summary>
Public NotInheritable Class FenetrePremierPlan

    Private Sub New()
    End Sub

    ''' <summary>Place la fenêtre au-dessus de toutes les autres (topmost).</summary>
    Public Shared Function Epingler(ByVal handle As IntPtr) As Boolean
        Return SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
    End Function

    ''' <summary>Retire l'état « toujours au premier plan ».</summary>
    Public Shared Function Desepingler(ByVal handle As IntPtr) As Boolean
        Return SetWindowPos(handle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
    End Function

End Class

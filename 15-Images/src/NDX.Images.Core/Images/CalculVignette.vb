' ============================================================================
'  CalculVignette.vb  -  Calcul (pur) des dimensions d'une vignette.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing

''' <summary>
''' Calcule les dimensions d'une vignette qui « tient » dans une boîte donnée, en
''' conservant les proportions et sans jamais agrandir l'image. Logique pure, donc
''' testable sans manipuler de pixels.
''' </summary>
Public NotInheritable Class CalculVignette

    Private Sub New()
    End Sub

    ''' <summary>Dimensions de la vignette pour une source <paramref name="srcL"/>×<paramref name="srcH"/> dans une boîte max.</summary>
    Public Shared Function Dimensionner(ByVal srcL As Integer, ByVal srcH As Integer,
                                        ByVal maxL As Integer, ByVal maxH As Integer) As Size
        If srcL <= 0 OrElse srcH <= 0 OrElse maxL <= 0 OrElse maxH <= 0 Then Return New Size(0, 0)
        Dim ratio As Double = Math.Min(maxL / CDbl(srcL), maxH / CDbl(srcH))
        If ratio >= 1.0 Then Return New Size(srcL, srcH)   ' pas d'agrandissement
        Return New Size(Math.Max(1, CInt(Math.Round(srcL * ratio))),
                        Math.Max(1, CInt(Math.Round(srcH * ratio))))
    End Function

End Class

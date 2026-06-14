' ============================================================================
'  CalculZoom.vb  -  Calcul (pur) du facteur de zoom, borne.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>
''' Arithmétique du zoom, isolée du contrôle d'affichage pour être testable sans
''' interface graphique. Le zoom est multiplicatif (chaque cran multiplie par un
''' pas constant) et borné entre un minimum et un maximum.
''' </summary>
Public NotInheritable Class CalculZoom

    Private Sub New()
    End Sub

    ''' <summary>Zoom minimal autorisé (10 %).</summary>
    Public Const ZoomMin As Double = 0.1

    ''' <summary>Zoom maximal autorisé (1000 %).</summary>
    Public Const ZoomMax As Double = 10.0

    ''' <summary>Pas multiplicatif par cran de molette (+20 %).</summary>
    Public Const Pas As Double = 1.2

    ''' <summary>Borne une valeur de zoom dans l'intervalle autorisé.</summary>
    Public Shared Function Borner(ByVal zoom As Double) As Double
        Return Math.Max(ZoomMin, Math.Min(ZoomMax, zoom))
    End Function

    ''' <summary>
    ''' Applique <paramref name="crans"/> crans de molette au zoom courant
    ''' (positif = agrandir, négatif = réduire), puis borne le résultat.
    ''' </summary>
    Public Shared Function AppliquerCrans(ByVal zoom As Double, ByVal crans As Integer) As Double
        Return Borner(zoom * Math.Pow(Pas, crans))
    End Function

End Class

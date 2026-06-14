' ============================================================================
'  CalculEtat.vb  -  Calcul (pur) de l'etat visuel d'un bouton.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>
''' Détermine l'état visuel d'un bouton à partir de trois entrées (activé, survolé,
''' enfoncé), selon une priorité fixe. Logique pure : testable sans dessiner.
''' </summary>
Public NotInheritable Class CalculEtat

    Private Sub New()
    End Sub

    ''' <summary>État résultant (Désactivé &gt; Enfoncé &gt; Survol &gt; Normal).</summary>
    Public Shared Function Determiner(ByVal active As Boolean, ByVal survol As Boolean, ByVal enfonce As Boolean) As EtatBouton
        If Not active Then Return EtatBouton.Desactive
        If enfonce Then Return EtatBouton.Enfonce
        If survol Then Return EtatBouton.Survol
        Return EtatBouton.Normal
    End Function

End Class

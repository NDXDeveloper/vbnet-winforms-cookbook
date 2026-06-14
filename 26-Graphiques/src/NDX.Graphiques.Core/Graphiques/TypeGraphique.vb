' ============================================================================
'  TypeGraphique.vb  -  Modes de representation d'une serie.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Façons de représenter une série de données.</summary>
Public Enum TypeGraphique
    ''' <summary>Barres verticales.</summary>
    Barres = 0
    ''' <summary>Courbe (ligne reliant les points).</summary>
    Courbe = 1
    ''' <summary>Points seuls.</summary>
    Points = 2
End Enum

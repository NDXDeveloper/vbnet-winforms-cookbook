' ============================================================================
'  EtatBouton.vb  -  Etats visuels d'un bouton.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>États visuels d'un bouton à états (machine à états simple).</summary>
Public Enum EtatBouton
    ''' <summary>Au repos.</summary>
    Normal = 0
    ''' <summary>Le curseur le survole.</summary>
    Survol = 1
    ''' <summary>Le bouton est enfoncé.</summary>
    Enfonce = 2
    ''' <summary>Le bouton est désactivé.</summary>
    Desactive = 3
End Enum

' ============================================================================
'  ResultatTouche.vb  -  Issue d'une frappe transmise au gestionnaire.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>État du gestionnaire après une frappe.</summary>
Public Enum EtatTouche
    ''' <summary>La frappe ne correspond à aucun raccourci (séquence réinitialisée).</summary>
    Aucun = 0
    ''' <summary>Début (ou suite) d'un accord : on attend la frappe suivante.</summary>
    EnAttente = 1
    ''' <summary>Un raccourci complet a été reconnu.</summary>
    Declenchee = 2
End Enum

''' <summary>Résultat d'une frappe : état + action déclenchée le cas échéant.</summary>
Public NotInheritable Class ResultatTouche

    Public Property Etat As EtatTouche
    Public Property Action As String
    ''' <summary>Séquence en cours (forme canonique), utile pour l'affichage.</summary>
    Public Property SequenceEnCours As String

    Public Shared Function Declenche(ByVal action As String, ByVal sequence As String) As ResultatTouche
        Return New ResultatTouche() With {.Etat = EtatTouche.Declenchee, .Action = action, .SequenceEnCours = sequence}
    End Function

    Public Shared Function EnAttente(ByVal sequence As String) As ResultatTouche
        Return New ResultatTouche() With {.Etat = EtatTouche.EnAttente, .SequenceEnCours = sequence}
    End Function

    Public Shared Function Aucun() As ResultatTouche
        Return New ResultatTouche() With {.Etat = EtatTouche.Aucun, .SequenceEnCours = ""}
    End Function

End Class

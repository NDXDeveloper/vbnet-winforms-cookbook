' ============================================================================
'  ResultatAuthentification.vb  -  Issue d'une tentative d'authentification.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Résultat d'une authentification LDAP : succès, message et compte associé.</summary>
Public NotInheritable Class ResultatAuthentification

    Public Property Reussite As Boolean
    Public Property Message As String
    Public Property Compte As Utilisateur

    Public Shared Function Succes(ByVal compte As Utilisateur) As ResultatAuthentification
        Return New ResultatAuthentification() With {
            .Reussite = True, .Compte = compte, .Message = "Authentification réussie."}
    End Function

    Public Shared Function Echec(ByVal message As String) As ResultatAuthentification
        Return New ResultatAuthentification() With {.Reussite = False, .Message = message}
    End Function

End Class

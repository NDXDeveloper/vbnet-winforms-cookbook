' ============================================================================
'  Utilisateur.vb  -  Compte d'annuaire (entree LDAP).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Un compte de l'annuaire (projection d'une entrée LDAP).</summary>
Public NotInheritable Class Utilisateur

    ''' <summary>Identifiant de connexion (attribut <c>uid</c>).</summary>
    Public Property Identifiant As String

    ''' <summary>Nom complet (attribut <c>cn</c>).</summary>
    Public Property NomComplet As String

    ''' <summary>Adresse de courriel (attribut <c>mail</c>), si présente.</summary>
    Public Property Courriel As String

    ''' <summary>Nom distinctif complet (DN) de l'entrée.</summary>
    Public Property NomDistinctif As String

    Public Overrides Function ToString() As String
        Return If(String.IsNullOrEmpty(NomComplet), Identifiant, NomComplet & " (" & Identifiant & ")")
    End Function

End Class

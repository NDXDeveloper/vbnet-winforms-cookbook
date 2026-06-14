' ============================================================================
'  ServiceAnnuaire.vb  -  Authentification et recherche LDAP.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.DirectoryServices.Protocols
Imports System.Net

''' <summary>
''' Authentifie un compte par « bind » LDAP et interroge l'annuaire. Le bind est
''' la méthode d'authentification de référence : le serveur valide lui-même le
''' couple (DN, mot de passe). On évite donc de manipuler des empreintes de mot de
''' passe côté application.
''' </summary>
Public NotInheritable Class ServiceAnnuaire

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Authentifie un utilisateur : tente un bind avec son DN et son mot de passe,
    ''' puis lit ses attributs. Renvoie un <see cref="ResultatAuthentification"/>.
    ''' </summary>
    Public Shared Function Authentifier(ByVal identifiant As String, ByVal motDePasse As String) As ResultatAuthentification
        If String.IsNullOrWhiteSpace(identifiant) OrElse String.IsNullOrEmpty(motDePasse) Then
            Return ResultatAuthentification.Echec("Identifiant et mot de passe obligatoires.")
        End If

        Dim dn As String = ConfigAnnuaire.DnPour(identifiant)
        Try
            Using cn As LdapConnection = Connexion()
                cn.Bind(New NetworkCredential(dn, motDePasse))   ' échoue => LdapException
                ' Authentification confirmée : on lit la PROPRE entrée du compte
                ' (lecture « self », sans privilège administrateur).
                Dim compte As Utilisateur = LireEntreePropre(cn, dn)
                Return ResultatAuthentification.Succes(compte)
            End Using
        Catch ex As LdapException
            ' Code 49 = identifiants invalides.
            Return ResultatAuthentification.Echec("Échec de l'authentification : " & ex.Message)
        Catch ex As Exception
            Return ResultatAuthentification.Echec("Annuaire injoignable : " & ex.Message)
        End Try
    End Function

    ''' <summary>Liste tous les comptes de l'annuaire (bind administrateur).</summary>
    Public Shared Function ListerUtilisateurs() As List(Of Utilisateur)
        Return Rechercher(Nothing)
    End Function

    ''' <summary>
    ''' Recherche des comptes (bind administrateur). Si <paramref name="terme"/> est
    ''' fourni, filtre sur uid/cn « contenant » le terme — <b>échappé</b> via
    ''' <see cref="EchappeurLdap"/> pour prévenir l'injection LDAP.
    ''' </summary>
    Public Shared Function Rechercher(ByVal terme As String) As List(Of Utilisateur)
        Dim filtre As String
        If String.IsNullOrWhiteSpace(terme) Then
            filtre = "(uid=*)"
        Else
            Dim t As String = EchappeurLdap.EchapperFiltre(terme.Trim())
            filtre = "(|(uid=*" & t & "*)(cn=*" & t & "*))"
        End If
        Dim liste As New List(Of Utilisateur)()
        Using cn As LdapConnection = Connexion()
            cn.Bind(New NetworkCredential(ConfigAnnuaire.AdminDN, ConfigAnnuaire.AdminMotDePasse))
            Dim requete As New SearchRequest(ConfigAnnuaire.OuUtilisateurs, filtre,
                                             SearchScope.Subtree, "uid", "cn", "mail")
            Dim reponse As SearchResponse = CType(cn.SendRequest(requete), SearchResponse)
            For Each entree As SearchResultEntry In reponse.Entries
                liste.Add(VersUtilisateur(entree))
            Next
        End Using
        Return liste
    End Function

    ''' <summary>Vérifie que l'annuaire répond (bind administrateur).</summary>
    Public Shared Function TesterConnexion(ByRef message As String) As Boolean
        Try
            Using cn As LdapConnection = Connexion()
                cn.Bind(New NetworkCredential(ConfigAnnuaire.AdminDN, ConfigAnnuaire.AdminMotDePasse))
                message = "Connexion OK - annuaire " & ConfigAnnuaire.Hote & ":" & ConfigAnnuaire.Port.ToString() &
                          " (base " & ConfigAnnuaire.BaseDN & ")."
                Return True
            End Using
        Catch ex As Exception
            message = "Echec de connexion : " & ex.Message
            Return False
        End Try
    End Function

    Private Shared Function Connexion() As LdapConnection
        Dim identite As New LdapDirectoryIdentifier(ConfigAnnuaire.Hote, ConfigAnnuaire.Port)
        Dim cn As New LdapConnection(identite)
        cn.SessionOptions.ProtocolVersion = 3
        cn.AuthType = AuthType.Basic
        cn.SessionOptions.SecureSocketLayer = False   ' démonstration : LDAP en clair
        cn.Timeout = TimeSpan.FromSeconds(8)
        Return cn
    End Function

    ''' <summary>
    ''' Lit l'entrée du compte tout juste authentifié, par son DN et en portée
    ''' <see cref="SearchScope.Base"/> : c'est une lecture « self », permise sans
    ''' privilège. Renvoie au minimum l'identifiant et le DN.
    ''' </summary>
    Private Shared Function LireEntreePropre(ByVal cn As LdapConnection, ByVal dn As String) As Utilisateur
        Try
            Dim requete As New SearchRequest(dn, "(objectClass=*)", SearchScope.Base, "uid", "cn", "mail")
            Dim reponse As SearchResponse = CType(cn.SendRequest(requete), SearchResponse)
            If reponse.Entries.Count > 0 Then Return VersUtilisateur(reponse.Entries(0))
        Catch
            ' Lecture facultative : on renvoie au moins le DN si elle échoue.
        End Try
        Return New Utilisateur() With {.NomDistinctif = dn, .Identifiant = IdentifiantDepuisDn(dn)}
    End Function

    ''' <summary>Extrait la valeur uid d'un DN « uid=xxx,... ».</summary>
    Private Shared Function IdentifiantDepuisDn(ByVal dn As String) As String
        If String.IsNullOrEmpty(dn) Then Return ""
        Dim premier As String = dn.Split(","c)(0).Trim()
        Dim p As Integer = premier.IndexOf("="c)
        Return If(p >= 0, premier.Substring(p + 1), premier)
    End Function

    Private Shared Function VersUtilisateur(ByVal entree As SearchResultEntry) As Utilisateur
        Return New Utilisateur() With {
            .NomDistinctif = entree.DistinguishedName,
            .Identifiant = Attribut(entree, "uid"),
            .NomComplet = Attribut(entree, "cn"),
            .Courriel = Attribut(entree, "mail")}
    End Function

    Private Shared Function Attribut(ByVal entree As SearchResultEntry, ByVal nom As String) As String
        If Not entree.Attributes.Contains(nom) Then Return ""
        Dim att As DirectoryAttribute = entree.Attributes(nom)
        If att Is Nothing OrElse att.Count = 0 Then Return ""
        Return Convert.ToString(att(0))
    End Function

End Class

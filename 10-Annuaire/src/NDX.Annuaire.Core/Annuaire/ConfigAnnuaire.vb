' ============================================================================
'  ConfigAnnuaire.vb  -  Parametres de connexion a l'annuaire LDAP.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Configuration

''' <summary>
''' Paramètres LDAP lus depuis <c>App.config</c> (clés <c>Ldap.*</c>), avec des
''' valeurs par défaut correspondant au conteneur OpenLDAP de démonstration.
''' </summary>
Public Module ConfigAnnuaire

    Public ReadOnly Property Hote As String
        Get
            Return Lire("Ldap.Hote", "127.0.0.1")
        End Get
    End Property

    Public ReadOnly Property Port As Integer
        Get
            Dim p As Integer
            Return If(Integer.TryParse(Lire("Ldap.Port", "389"), p), p, 389)
        End Get
    End Property

    ''' <summary>Racine de l'annuaire (ex. <c>dc=exemple,dc=test</c>).</summary>
    Public ReadOnly Property BaseDN As String
        Get
            Return Lire("Ldap.BaseDN", "dc=exemple,dc=test")
        End Get
    End Property

    ''' <summary>Unité d'organisation contenant les comptes.</summary>
    Public ReadOnly Property OuUtilisateurs As String
        Get
            Return Lire("Ldap.OuUtilisateurs", "ou=users," & BaseDN)
        End Get
    End Property

    ''' <summary>Gabarit du DN d'un compte ; <c>{0}</c> = identifiant.</summary>
    Public ReadOnly Property GabaritDnUtilisateur As String
        Get
            Return Lire("Ldap.GabaritDnUtilisateur", "uid={0}," & OuUtilisateurs)
        End Get
    End Property

    ''' <summary>DN du compte administrateur (recherche / annuaire).</summary>
    Public ReadOnly Property AdminDN As String
        Get
            Return Lire("Ldap.AdminDN", "cn=admin," & BaseDN)
        End Get
    End Property

    Public ReadOnly Property AdminMotDePasse As String
        Get
            Return Lire("Ldap.AdminMotDePasse", "admin_pwd")
        End Get
    End Property

    ''' <summary>Construit le DN d'un compte à partir de son identifiant.</summary>
    Public Function DnPour(ByVal identifiant As String) As String
        Return String.Format(GabaritDnUtilisateur, identifiant)
    End Function

    Private Function Lire(ByVal cle As String, ByVal defaut As String) As String
        Dim v As String = ConfigurationManager.AppSettings(cle)
        Return If(String.IsNullOrWhiteSpace(v), defaut, v)
    End Function

End Module

' ============================================================================
'  ConfigBdd.vb  -  Configuration d'acces a la base "demarrage".
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Configuration
Imports MySql.Data.MySqlClient

''' <summary>Chaine de connexion (cles <c>Bdd.*</c> ; defaut = conteneur Docker, port 3330).</summary>
Public Module ConfigBdd

    Private Const SERVEUR_DEFAUT As String = "127.0.0.1"
    Private Const PORT_DEFAUT As UInteger = 3330UI
    Private Const BASE_DEFAUT As String = "demarrage"
    Private Const UTILISATEUR_DEFAUT As String = "demarrage_app"
    Private Const MOTDEPASSE_DEFAUT As String = "demarrage_pwd"

    Public ReadOnly Property NomBase As String
        Get
            Return Lire("Bdd.Base", BASE_DEFAUT)
        End Get
    End Property

    Public Function ChaineConnexion() As String
        Dim port As UInteger
        If Not UInteger.TryParse(Lire("Bdd.Port", PORT_DEFAUT.ToString()), port) Then port = PORT_DEFAUT
        Dim builder As New MySqlConnectionStringBuilder() With {
            .Server = Lire("Bdd.Serveur", SERVEUR_DEFAUT),
            .Port = port,
            .Database = NomBase,
            .UserID = Lire("Bdd.Utilisateur", UTILISATEUR_DEFAUT),
            .Password = Lire("Bdd.MotDePasse", MOTDEPASSE_DEFAUT),
            .Pooling = True,
            .ConnectionTimeout = 10,
            .DefaultCommandTimeout = 60,
            .SslMode = MySqlSslMode.Disabled
        }
        Return builder.ConnectionString
    End Function

    Private Function Lire(ByVal cle As String, ByVal defaut As String) As String
        Dim v As String = ConfigurationManager.AppSettings(cle)
        Return If(String.IsNullOrWhiteSpace(v), defaut, v)
    End Function

End Module

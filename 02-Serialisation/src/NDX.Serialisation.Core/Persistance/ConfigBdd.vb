' ============================================================================
'  ConfigBdd.vb
'  Configuration d'acces a la base de demonstration (MariaDB "coffre").
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Configuration
Imports MySql.Data.MySqlClient

''' <summary>
''' Centralise la chaine de connexion a la base de demonstration. Les parametres
''' sont lus depuis la section <c>appSettings</c> du fichier <c>App.config</c>
''' (cles <c>Bdd.*</c>) ; a defaut, des valeurs pointant vers le conteneur Docker
''' fourni (port hote 3308) sont utilisees.
''' </summary>
Public Module ConfigBdd

    Private Const SERVEUR_DEFAUT As String = "127.0.0.1"
    Private Const PORT_DEFAUT As UInteger = 3308UI
    Private Const BASE_DEFAUT As String = "coffre"
    Private Const UTILISATEUR_DEFAUT As String = "coffre_app"
    Private Const MOTDEPASSE_DEFAUT As String = "coffre_pwd"

    ''' <summary>Nom de la base cible.</summary>
    Public ReadOnly Property NomBase As String
        Get
            Return LireParametre("Bdd.Base", BASE_DEFAUT)
        End Get
    End Property

    ''' <summary>Adresse du serveur.</summary>
    Public ReadOnly Property Serveur As String
        Get
            Return LireParametre("Bdd.Serveur", SERVEUR_DEFAUT)
        End Get
    End Property

    ''' <summary>Port TCP du serveur.</summary>
    Public ReadOnly Property Port As UInteger
        Get
            Dim valeur As String = LireParametre("Bdd.Port", PORT_DEFAUT.ToString())
            Dim resultat As UInteger
            If UInteger.TryParse(valeur, resultat) Then Return resultat
            Return PORT_DEFAUT
        End Get
    End Property

    ''' <summary>Compte applicatif.</summary>
    Public ReadOnly Property Utilisateur As String
        Get
            Return LireParametre("Bdd.Utilisateur", UTILISATEUR_DEFAUT)
        End Get
    End Property

    ''' <summary>Mot de passe du compte applicatif.</summary>
    Public ReadOnly Property MotDePasse As String
        Get
            Return LireParametre("Bdd.MotDePasse", MOTDEPASSE_DEFAUT)
        End Get
    End Property

    ''' <summary>Assemble la chaine de connexion via un constructeur typé et sûr.</summary>
    Public Function ChaineConnexion(Optional ByVal pooling As Boolean = True) As String
        Dim builder As New MySqlConnectionStringBuilder() With {
            .Server = Serveur,
            .Port = Port,
            .Database = NomBase,
            .UserID = Utilisateur,
            .Password = MotDePasse,
            .Pooling = pooling,
            .DefaultCommandTimeout = 60,
            .ConnectionTimeout = 10,
            .SslMode = MySqlSslMode.Disabled
        }
        Return builder.ConnectionString
    End Function

    ''' <summary>Lit une cle d'App.config (section appSettings), avec valeur de repli.</summary>
    Private Function LireParametre(ByVal cle As String, ByVal valeurParDefaut As String) As String
        Dim valeur As String = ConfigurationManager.AppSettings(cle)
        If String.IsNullOrWhiteSpace(valeur) Then Return valeurParDefaut
        Return valeur
    End Function

End Module

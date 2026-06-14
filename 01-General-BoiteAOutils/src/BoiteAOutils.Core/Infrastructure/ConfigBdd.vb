Imports System.Configuration
Imports MySql.Data.MySqlClient

''' <summary>
''' Centralise la configuration d'acces a la base de donnees de demonstration
''' (MariaDB <c>etabli</c>).
''' </summary>
''' <remarks>
''' <para>
''' Expose la chaine de connexion ainsi que le nom de la base, informations
''' reutilisees par les rapports d'exception.
''' </para>
''' <para>
''' Les parametres sont lus depuis la section <c>appSettings</c> du fichier
''' <c>App.config</c> ; a defaut, des valeurs par defaut pointant vers le
''' conteneur Docker fourni (port hote 3307) sont utilisees. La chaine finale
''' est assemblee via <see cref="MySqlConnectionStringBuilder"/>, ce qui
''' garantit des mots-cles valides et echappe correctement les valeurs.
''' </para>
''' </remarks>
Public Module ConfigBdd

    ' Valeurs par defaut : elles correspondent exactement au docker-compose.yml
    ' livre avec le projet (service MariaDB expose sur le port hote 3307).
    Private Const SERVEUR_DEFAUT As String = "127.0.0.1"
    Private Const PORT_DEFAUT As UInteger = 3307UI
    Private Const BASE_DEFAUT As String = "etabli"
    Private Const UTILISATEUR_DEFAUT As String = "etabli_app"
    Private Const MOTDEPASSE_DEFAUT As String = "etabli_pwd"

    ''' <summary>Nom de la base de donnees cible (affiche dans les rapports d'exception).</summary>
    Public ReadOnly Property NomBase As String
        Get
            Return LireParametre("Bdd.Base", BASE_DEFAUT)
        End Get
    End Property

    ''' <summary>Adresse du serveur MariaDB.</summary>
    Public ReadOnly Property Serveur As String
        Get
            Return LireParametre("Bdd.Serveur", SERVEUR_DEFAUT)
        End Get
    End Property

    ''' <summary>Port TCP du serveur MariaDB (3307 pour le conteneur Docker fourni).</summary>
    Public ReadOnly Property Port As UInteger
        Get
            Dim valeur As String = LireParametre("Bdd.Port", PORT_DEFAUT.ToString())
            Dim resultat As UInteger
            If UInteger.TryParse(valeur, resultat) Then Return resultat
            Return PORT_DEFAUT
        End Get
    End Property

    ''' <summary>Compte applicatif utilise pour se connecter.</summary>
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

    ''' <summary>
    ''' Construit la chaine de connexion complete a partir des parametres courants.
    ''' </summary>
    ''' <param name="pooling">
    ''' Active (par defaut) ou non le pool de connexions.
    ''' </param>
    ''' <returns>Une chaine de connexion MySql/MariaDB valide.</returns>
    Public Function ChaineConnexion(Optional ByVal pooling As Boolean = True) As String
        ' MySqlConnectionStringBuilder : alternative typee et sure a la
        ' concatenation manuelle "server=...;user id=...;".
        Dim builder As New MySqlConnectionStringBuilder() With {
            .Server = Serveur,
            .Port = Port,
            .Database = NomBase,
            .UserID = Utilisateur,
            .Password = MotDePasse,
            .Pooling = pooling,
            .UseAffectedRows = True,        ' RowsAffected reflete les lignes reellement modifiees.
            .DefaultCommandTimeout = 600,   ' Delai genereux pour les requetes longues.
            .ConnectionTimeout = 10,        ' Echec rapide si le conteneur n'est pas demarre.
            .SslMode = MySqlSslMode.Disabled ' Demonstration locale : pas de TLS a negocier.
        }
        Return builder.ConnectionString
    End Function

    ''' <summary>
    ''' Lit un parametre dans App.config (section appSettings), avec valeur de repli.
    ''' </summary>
    Private Function LireParametre(ByVal cle As String, ByVal valeurParDefaut As String) As String
        Dim valeur As String = ConfigurationManager.AppSettings(cle)
        If String.IsNullOrWhiteSpace(valeur) Then
            Return valeurParDefaut
        End If
        Return valeur
    End Function

End Module

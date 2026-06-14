Imports System.Data
Imports System.Reflection
Imports System.Timers
Imports MySql.Data.MySqlClient

''' <summary>
''' Encapsule une connexion MariaDB/MySQL et son cycle de vie.
''' </summary>
''' <remarks>
''' <para>
''' Encapsule une connexion MariaDB/MySQL et son cycle de vie. Techniques mises
''' en oeuvre :
''' </para>
''' <list type="number">
''' <item><description>
''' implementation rigoureuse du patron <see cref="IDisposable"/> (liberation
''' deterministe de la connexion et du pool) ;
''' </description></item>
''' <item><description>
''' gestion explicite des transactions (<see cref="BeginTransaction"/> /
''' <see cref="Commit"/> / <see cref="Rollback"/>) ;
''' </description></item>
''' <item><description>
''' fermeture automatique apres une periode d'inactivite via un
''' <see cref="Timers.Timer"/> (sauf connexion principale ou transaction en cours) ;
''' </description></item>
''' <item><description>
''' historique glissant des dernieres actions SQL (<see cref="Action"/> et
''' <c>ActionNM1..ActionNM5</c>), precieux pour diagnostiquer une exception ;
''' </description></item>
''' <item><description>
''' identifiant de connexion unique attribue de maniere thread-safe.
''' </description></item>
''' </list>
''' <para>
''' Note : la configuration de session
''' (timeout de verrou InnoDB) est appliquee paresseusement lors de la premiere
''' ouverture reussie, et non dans le constructeur. Cela evite qu'instancier un
''' objet ne provoque une connexion immediate (et donc une erreur) lorsque le
''' conteneur MariaDB n'est pas demarre.
''' </para>
''' </remarks>
Public Class ConnexionMySql
    Implements IDisposable

#Region "Compteur d'identifiants de connexion (thread-safe)"
    ' Numerotation synchronisee (thread-safe) des connexions.
    Private Shared ReadOnly _verrouCompteur As New Object()
    Private Shared _compteurConnexions As Integer = 0

    ''' <summary>Attribue, de facon atomique, un nouvel identifiant de connexion.</summary>
    Public Shared Function NouvelIdConnexion() As Integer
        SyncLock _verrouCompteur
            _compteurConnexions += 1
            Return _compteurConnexions
        End SyncLock
    End Function
#End Region

#Region "Champs"
    ' Delai d'inactivite (ms) au terme duquel une connexion secondaire est fermee.
    Private Const INTERVALLE_FERMETURE As Integer = 60000

    Private ReadOnly _id As Integer
    Private ReadOnly _estPrincipale As Boolean
    Private ReadOnly _minuterieFermeture As Timers.Timer

    Private _conn As MySqlConnection
    Private _tr As MySqlTransaction
    Private _sessionConfiguree As Boolean = False
    Private _dejaLibere As Boolean = False

    ' Historique des actions : protege par un verrou car alimente depuis le code
    ' d'acces aux donnees, potentiellement multi-thread.
    Private ReadOnly _verrouAction As New Object()
    Private _action As String = ""
    Private _actionNM1 As String = ""
    Private _actionNM2 As String = ""
    Private _actionNM3 As String = ""
    Private _actionNM4 As String = ""
    Private _actionNM5 As String = ""
#End Region

#Region "Proprietes"
    ''' <summary>Identifiant unique de cette connexion (utile dans les traces).</summary>
    Public ReadOnly Property Id As Integer
        Get
            Return _id
        End Get
    End Property

    ''' <summary>Indique s'il s'agit de la connexion principale (jamais fermee automatiquement).</summary>
    Public ReadOnly Property EstPrincipale As Boolean
        Get
            Return _estPrincipale
        End Get
    End Property

    ''' <summary>Objet de connexion ADO.NET sous-jacent.</summary>
    Public Property Conn As MySqlConnection
        Get
            Return _conn
        End Get
        Set(value As MySqlConnection)
            _conn = value
        End Set
    End Property

    ''' <summary>Transaction courante, ou Nothing si aucune n'est ouverte.</summary>
    Public Property Tr As MySqlTransaction
        Get
            Return _tr
        End Get
        Set(value As MySqlTransaction)
            _tr = value
        End Set
    End Property

    ''' <summary>Chaine de connexion utilisee.</summary>
    Public ReadOnly Property ChaineConnexion As String
        Get
            Return If(_conn IsNot Nothing, _conn.ConnectionString, String.Empty)
        End Get
    End Property

    ''' <summary>Etat courant de la connexion ADO.NET.</summary>
    Public ReadOnly Property Etat As ConnectionState
        Get
            If _conn Is Nothing Then Return ConnectionState.Closed
            Return _conn.State
        End Get
    End Property

    ''' <summary>Indique si une transaction est actuellement ouverte.</summary>
    Public ReadOnly Property TransactionEnCours As Boolean
        Get
            Return _tr IsNot Nothing
        End Get
    End Property

    ''' <summary>
    ''' Derniere action SQL effectuee. L'affectation decale automatiquement
    ''' l'historique : la valeur precedente devient <see cref="ActionNM1"/>, etc.
    ''' </summary>
    Public Property Action As String
        Get
            Return _action
        End Get
        Set(value As String)
            SyncLock _verrouAction
                _actionNM5 = _actionNM4
                _actionNM4 = _actionNM3
                _actionNM3 = _actionNM2
                _actionNM2 = _actionNM1
                _actionNM1 = _action
                _action = value
            End SyncLock
        End Set
    End Property

    ''' <summary>Action n-1.</summary>
    Public ReadOnly Property ActionNM1 As String
        Get
            Return _actionNM1
        End Get
    End Property
    ''' <summary>Action n-2.</summary>
    Public ReadOnly Property ActionNM2 As String
        Get
            Return _actionNM2
        End Get
    End Property
    ''' <summary>Action n-3.</summary>
    Public ReadOnly Property ActionNM3 As String
        Get
            Return _actionNM3
        End Get
    End Property
    ''' <summary>Action n-4.</summary>
    Public ReadOnly Property ActionNM4 As String
        Get
            Return _actionNM4
        End Get
    End Property
    ''' <summary>Action n-5.</summary>
    Public ReadOnly Property ActionNM5 As String
        Get
            Return _actionNM5
        End Get
    End Property
#End Region

#Region "Construction"
    ''' <summary>
    ''' Cree une connexion sans l'ouvrir.
    ''' </summary>
    ''' <param name="estPrincipale">
    ''' True pour une connexion permanente (non soumise a la fermeture
    ''' automatique) ; False pour une connexion de travail ephemere.
    ''' </param>
    ''' <param name="methodeAppelante">
    ''' Methode a l'origine de la creation (tracee dans le journal). On la passe
    ''' explicitement, car <see cref="MethodBase.GetCurrentMethod"/>
    ''' renverrait ici le constructeur lui-meme.
    ''' </param>
    ''' <param name="chaineConnexion">
    ''' Chaine de connexion explicite ; si vide, celle de <see cref="ConfigBdd"/> est utilisee.
    ''' </param>
    Public Sub New(ByVal estPrincipale As Boolean,
                   ByVal methodeAppelante As MethodBase,
                   Optional ByVal chaineConnexion As String = "")

        _id = NouvelIdConnexion()
        _estPrincipale = estPrincipale

        Dim chaine As String = If(String.IsNullOrWhiteSpace(chaineConnexion),
                                  ConfigBdd.ChaineConnexion(pooling:=True),
                                  chaineConnexion)
        _conn = New MySqlConnection(chaine)

        ' Minuterie de fermeture automatique (connexions secondaires uniquement).
        _minuterieFermeture = New Timers.Timer(INTERVALLE_FERMETURE) With {.AutoReset = False}
        AddHandler _minuterieFermeture.Elapsed, AddressOf SurMinuterieFermeture

        Dim depuis As String = If(methodeAppelante IsNot Nothing,
            methodeAppelante.DeclaringType.Name & "." & methodeAppelante.Name & "()",
            "(inconnu)")
        Journal.Ecrire(String.Format("Nouvelle connexion {0} ID {1} depuis {2}",
                                     If(_estPrincipale, "principale", "secondaire"), _id, depuis),
                       Journal.Niveau.Debogage)
    End Sub
#End Region

#Region "Ouverture / fermeture"
    ''' <summary>Ouvre la connexion si elle ne l'est pas deja, puis (re)arme la minuterie.</summary>
    Public Sub Open(Optional ByVal methodeAppelante As MethodBase = Nothing)
        Try
            If _conn Is Nothing Then Return
            If _conn.State <> ConnectionState.Open Then
                _conn.Open()
                Journal.Ecrire("Ouverture connexion ID " & _id, Journal.Niveau.Debogage)
                ConfigurerSessionUneFois()
            End If
            RearmerMinuterie()
        Catch ex As Exception
            Dim exc As String = GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace)
            GestionExceptions.TraiterException(exc)
        End Try
    End Sub

    ''' <summary>Ferme la connexion (sans liberer l'objet ; il reste reutilisable).</summary>
    Public Sub Close(Optional ByVal methodeAppelante As MethodBase = Nothing)
        Try
            If _conn IsNot Nothing AndAlso _conn.State <> ConnectionState.Closed Then
                _conn.Close()
                Journal.Ecrire("Fermeture connexion ID " & _id, Journal.Niveau.Debogage)
            End If
        Catch ex As Exception
            Dim exc As String = GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace)
            GestionExceptions.TraiterException(exc)
        End Try
    End Sub

    ''' <summary>
    ''' Recree un objet de connexion neuf en conservant la chaine courante.
    ''' Utilise par la logique de "seconde chance" du module <see cref="AccesDonnees"/>.
    ''' </summary>
    Public Sub RegenererConnexion()
        If _conn IsNot Nothing Then
            Dim ancienneChaine As String = _conn.ConnectionString
            _conn = New MySqlConnection(ancienneChaine)
            _sessionConfiguree = False
        End If
    End Sub
#End Region

#Region "Transactions"
    ''' <summary>Demarre une transaction (ouvre la connexion au besoin).</summary>
    ''' <returns>True si la transaction a bien ete ouverte.</returns>
    Public Function BeginTransaction(Optional ByVal methodeAppelante As MethodBase = Nothing) As Boolean
        Try
            If _conn Is Nothing Then Return False
            If _conn.State <> ConnectionState.Open Then Open(methodeAppelante)
            _tr = _conn.BeginTransaction()
            Journal.Ecrire("Debut de transaction sur connexion ID " & _id, Journal.Niveau.Debogage)
            ' Tant qu'une transaction est ouverte, la fermeture automatique est suspendue.
            _minuterieFermeture.Stop()
            Return _tr IsNot Nothing
        Catch ex As Exception
            Dim exc As String = GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace)
            GestionExceptions.TraiterException(exc)
            Return False
        End Try
    End Function

    ''' <summary>Valide la transaction courante.</summary>
    Public Sub Commit(Optional ByVal methodeAppelante As MethodBase = Nothing)
        Try
            If _tr IsNot Nothing Then
                _tr.Commit()
                Journal.Ecrire("Commit sur connexion ID " & _id, Journal.Niveau.Debogage)
                _tr = Nothing
                RearmerMinuterie()
            End If
        Catch ex As Exception
            Dim exc As String = GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace)
            GestionExceptions.TraiterException(exc)
        End Try
    End Sub

    ''' <summary>Annule la transaction courante.</summary>
    Public Sub Rollback(Optional ByVal methodeAppelante As MethodBase = Nothing)
        Try
            If _tr IsNot Nothing Then
                _tr.Rollback()
                Journal.Ecrire("Rollback sur connexion ID " & _id, Journal.Niveau.Avertissement)
                _tr = Nothing
                RearmerMinuterie()
            End If
        Catch ex As Exception
            Dim exc As String = GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace)
            GestionExceptions.TraiterException(exc)
        End Try
    End Sub
#End Region

#Region "Mecanique interne"
    ''' <summary>
    ''' Configure la session MariaDB une seule fois (timeout d'attente de verrou
    ''' InnoDB) une seule fois, a la premiere ouverture reussie.
    ''' </summary>
    Private Sub ConfigurerSessionUneFois()
        If _sessionConfiguree Then Return
        Try
            Using cmd As New MySqlCommand("SET @@session.innodb_lock_wait_timeout = @v;", _conn, _tr)
                cmd.Parameters.AddWithValue("@v", 120)
                cmd.ExecuteScalar()
            End Using
            _sessionConfiguree = True
        Catch
            ' Non bloquant : on retentera a la prochaine ouverture.
        End Try
    End Sub

    ''' <summary>(Re)demarre la minuterie de fermeture pour les connexions secondaires hors transaction.</summary>
    Private Sub RearmerMinuterie()
        _minuterieFermeture.Stop()
        If Not _estPrincipale AndAlso _tr Is Nothing Then
            _minuterieFermeture.Start()
        End If
    End Sub

    ''' <summary>Ferme la connexion apres expiration du delai d'inactivite.</summary>
    Private Sub SurMinuterieFermeture(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        Try
            If _conn IsNot Nothing AndAlso _tr Is Nothing AndAlso Not _estPrincipale Then
                If _conn.State <> ConnectionState.Closed Then
                    _conn.Close()
                    Journal.Ecrire("Fermeture automatique (inactivite) connexion ID " & _id, Journal.Niveau.Debogage)
                End If
            End If
        Catch
            ' Le thread de la minuterie ne doit jamais propager d'exception.
        End Try
    End Sub
#End Region

#Region "IDisposable"
    ''' <summary>Libere la connexion, la minuterie et, le cas echeant, le pool associe.</summary>
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If _dejaLibere Then Return
        If disposing Then
            Try
                If _minuterieFermeture IsNot Nothing Then
                    _minuterieFermeture.Stop()
                    RemoveHandler _minuterieFermeture.Elapsed, AddressOf SurMinuterieFermeture
                    _minuterieFermeture.Dispose()
                End If
                If _conn IsNot Nothing Then
                    If _conn.State <> ConnectionState.Closed Then _conn.Close()
                    ' Nettoyage du pool pour la connexion principale.
                    If _estPrincipale Then MySqlConnection.ClearPool(_conn)
                    _conn.Dispose()
                    _conn = Nothing
                    Journal.Ecrire("Dispose connexion ID " & _id, Journal.Niveau.Debogage)
                End If
            Catch
                ' Liberation best effort.
            End Try
        End If
        _dejaLibere = True
    End Sub

    ''' <summary>Implementation publique de <see cref="IDisposable.Dispose"/>.</summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

Imports System.ComponentModel
Imports System.Threading
Imports MySql.Data.MySqlClient

''' <summary>
''' Surveille en arriere-plan la disponibilite de la base de donnees et notifie
''' les changements d'etat.
''' </summary>
''' <remarks>
''' <para>
''' Illustration du patron <see cref="BackgroundWorker"/>. Un worker
''' execute une boucle de test sur un thread d'arriere-plan, puis remonte les
''' resultats via <see cref="BackgroundWorker.ReportProgress"/> : ceux-ci sont
''' recus sur le thread qui a demarre le worker (le thread d'interface, s'il
''' dispose d'un <see cref="SynchronizationContext"/>), ce qui rend l'evenement
''' <see cref="EtatChange"/> sur a manipuler depuis l'UI.
''' </para>
''' <para>Implemente <see cref="IDisposable"/> pour garantir l'arret du thread.</para>
''' </remarks>
Public Class MoniteurReseau
    Implements IDisposable

    Private ReadOnly _worker As BackgroundWorker
    Private _intervalleMs As Integer = 2000
    Private _dernierEtat As Boolean? = Nothing
    Private _libere As Boolean = False

    ''' <summary>Declenche lorsque l'etat de disponibilite de la base change.</summary>
    Public Event EtatChange(ByVal disponible As Boolean)

    ''' <summary>Intervalle (ms) entre deux verifications. 2000 ms par defaut.</summary>
    Public Property IntervalleMs As Integer
        Get
            Return _intervalleMs
        End Get
        Set(value As Integer)
            _intervalleMs = Math.Max(250, value)
        End Set
    End Property

    Public Sub New()
        _worker = New BackgroundWorker() With {
            .WorkerReportsProgress = True,
            .WorkerSupportsCancellation = True
        }
        AddHandler _worker.DoWork, AddressOf SurDoWork
        AddHandler _worker.ProgressChanged, AddressOf SurProgressChanged
    End Sub

    ''' <summary>Demarre la surveillance (sans effet si deja en cours).</summary>
    Public Sub Demarrer()
        If Not _worker.IsBusy Then
            _worker.RunWorkerAsync()
            Journal.Ecrire("Moniteur reseau demarre.", Journal.Niveau.Information)
        End If
    End Sub

    ''' <summary>Demande l'arret de la surveillance.</summary>
    Public Sub Arreter()
        If _worker.IsBusy Then
            _worker.CancelAsync()
            Journal.Ecrire("Moniteur reseau : arret demande.", Journal.Niveau.Information)
        End If
    End Sub

    ' Boucle executee sur le thread d'arriere-plan.
    Private Sub SurDoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        If Thread.CurrentThread.Name Is Nothing Then
            Thread.CurrentThread.Name = "MoniteurReseau"
        End If
        While Not _worker.CancellationPending
            Dim disponible As Boolean = TesterDisponibilite()
            ' On ne notifie que sur changement d'etat (evite le bruit).
            If Not _dernierEtat.HasValue OrElse _dernierEtat.Value <> disponible Then
                _dernierEtat = disponible
                _worker.ReportProgress(If(disponible, 1, 0))
            End If
            Thread.Sleep(_intervalleMs)
        End While
        e.Cancel = True
    End Sub

    ' Recu sur le thread appelant (UI) : on peut donc lever l'evenement sans risque.
    Private Sub SurProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)
        RaiseEvent EtatChange(e.ProgressPercentage = 1)
    End Sub

    ' Test minimal et rapide, independant de ConnexionMySql pour ne pas saturer
    ' le journal avec une trace toutes les deux secondes.
    Private Function TesterDisponibilite() As Boolean
        Try
            Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion(pooling:=True))
                cn.Open()
                Using cmd As New MySqlCommand("SELECT 1;", cn)
                    cmd.ExecuteScalar()
                End Using
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If _libere Then Return
        Try
            Arreter()
            RemoveHandler _worker.DoWork, AddressOf SurDoWork
            RemoveHandler _worker.ProgressChanged, AddressOf SurProgressChanged
            _worker.Dispose()
        Catch
        End Try
        _libere = True
        GC.SuppressFinalize(Me)
    End Sub

End Class

' ============================================================================
'  UniteDeTravail.vb  -  Unit of Work : une connexion + une transaction partagees.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' Patron <b>Unit of Work</b> : encapsule une connexion ouverte et, optionnellement,
''' une transaction. Plusieurs depots peuvent partager la meme unite afin que leurs
''' operations soient validees (ou annulees) <b>ensemble</b>.
''' </summary>
Public NotInheritable Class UniteDeTravail
    Implements IDisposable

    Private ReadOnly _connexion As MySqlConnection
    Private _transaction As MySqlTransaction
    Private _libere As Boolean = False

    Public Sub New()
        _connexion = New MySqlConnection(ConfigBdd.ChaineConnexion())
        _connexion.Open()
    End Sub

    ''' <summary>Connexion sous-jacente.</summary>
    Public ReadOnly Property Connexion As MySqlConnection
        Get
            Return _connexion
        End Get
    End Property

    ''' <summary>Transaction courante (Nothing hors transaction).</summary>
    Public ReadOnly Property Transaction As MySqlTransaction
        Get
            Return _transaction
        End Get
    End Property

    ''' <summary>Demarre une transaction.</summary>
    Public Sub Commencer()
        If _transaction Is Nothing Then _transaction = _connexion.BeginTransaction()
    End Sub

    ''' <summary>Valide la transaction courante.</summary>
    Public Sub Valider()
        If _transaction IsNot Nothing Then
            _transaction.Commit()
            _transaction.Dispose()
            _transaction = Nothing
        End If
    End Sub

    ''' <summary>Annule la transaction courante.</summary>
    Public Sub Annuler()
        If _transaction IsNot Nothing Then
            _transaction.Rollback()
            _transaction.Dispose()
            _transaction = Nothing
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _libere Then Return
        Try
            If _transaction IsNot Nothing Then
                _transaction.Rollback()   ' transaction non validee -> annulation par securite
                _transaction.Dispose()
                _transaction = Nothing
            End If
        Catch
        End Try
        Try
            If _connexion IsNot Nothing AndAlso _connexion.State <> ConnectionState.Closed Then _connexion.Close()
            _connexion.Dispose()
        Catch
        End Try
        _libere = True
    End Sub

End Class

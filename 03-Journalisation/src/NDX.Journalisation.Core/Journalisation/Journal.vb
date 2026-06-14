' ============================================================================
'  Journal.vb
'  Journal central thread-safe : diffuse chaque entree vers plusieurs puits.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Collections.Generic
Imports System.Diagnostics

''' <summary>
''' Point d'entree de la journalisation. On y enregistre un ou plusieurs
''' <see cref="IPuits"/> ; chaque message dont le niveau atteint
''' <see cref="NiveauMinimal"/> est diffuse a tous les puits, de facon thread-safe.
''' </summary>
Public NotInheritable Class Journal
    Implements IDisposable

    Private ReadOnly _puits As New List(Of IPuits)()
    Private ReadOnly _verrou As New Object()
    Private _libere As Boolean = False

    ''' <summary>Niveau minimal reellement journalise (filtre les entrees moins graves).</summary>
    Public Property NiveauMinimal As Niveau = Niveau.Debogage

    ''' <summary>Ajoute un puits de destination.</summary>
    Public Sub AjouterPuits(ByVal puits As IPuits)
        If puits Is Nothing Then Throw New ArgumentNullException(NameOf(puits))
        SyncLock _verrou
            _puits.Add(puits)
        End SyncLock
    End Sub

    ''' <summary>Journalise une entree de niveau arbitraire.</summary>
    Public Sub Journaliser(ByVal niveau As Niveau, ByVal categorie As String,
                           ByVal message As String, Optional ByVal exception As Exception = Nothing)
        If niveau < NiveauMinimal Then Return
        Dim entree As New EntreeJournal(niveau, categorie, message,
                                        If(exception IsNot Nothing, exception.ToString(), Nothing))
        SyncLock _verrou
            For Each puits As IPuits In _puits
                Try
                    puits.Ecrire(entree)
                Catch ex As Exception
                    ' Un puits defaillant ne doit pas interrompre les autres ni l'appelant.
                    Debug.WriteLine("[Journal] puits en echec : " & ex.Message)
                End Try
            Next
        End SyncLock
    End Sub

    ''' <summary>Journalise au niveau Debogage.</summary>
    Public Sub Debogage(ByVal categorie As String, ByVal message As String)
        Journaliser(Niveau.Debogage, categorie, message)
    End Sub

    ''' <summary>Journalise au niveau Information.</summary>
    Public Sub Information(ByVal categorie As String, ByVal message As String)
        Journaliser(Niveau.Information, categorie, message)
    End Sub

    ''' <summary>Journalise au niveau Avertissement.</summary>
    Public Sub Avertissement(ByVal categorie As String, ByVal message As String)
        Journaliser(Niveau.Avertissement, categorie, message)
    End Sub

    ''' <summary>Journalise au niveau Erreur (avec exception facultative).</summary>
    Public Sub Erreur(ByVal categorie As String, ByVal message As String, Optional ByVal exception As Exception = Nothing)
        Journaliser(Niveau.Erreur, categorie, message, exception)
    End Sub

    ''' <summary>Journalise au niveau Critique (avec exception facultative).</summary>
    Public Sub Critique(ByVal categorie As String, ByVal message As String, Optional ByVal exception As Exception = Nothing)
        Journaliser(Niveau.Critique, categorie, message, exception)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _libere Then Return
        SyncLock _verrou
            For Each puits As IPuits In _puits
                Try
                    puits.Dispose()
                Catch
                End Try
            Next
            _puits.Clear()
        End SyncLock
        _libere = True
    End Sub

End Class

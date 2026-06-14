' ============================================================================
'  PuitsMemoire.vb
'  Puits conservant les dernieres entrees en memoire (affichage en direct).
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Collections.Generic

''' <summary>
''' Puits memoire : conserve un tampon circulaire des dernieres entrees et
''' notifie via <see cref="EntreeAjoutee"/>. Pratique pour un affichage temps reel.
''' </summary>
Public NotInheritable Class PuitsMemoire
    Implements IPuits

    Private ReadOnly _verrou As New Object()
    Private ReadOnly _entrees As New List(Of EntreeJournal)()

    ''' <summary>Nombre maximal d'entrees conservees en memoire.</summary>
    Public Property Capacite As Integer = 500

    ''' <summary>Declenche a chaque nouvelle entree (peut etre recu sur un thread autre que l'UI).</summary>
    Public Event EntreeAjoutee(ByVal entree As EntreeJournal)

    Public Sub Ecrire(ByVal entree As EntreeJournal) Implements IPuits.Ecrire
        SyncLock _verrou
            _entrees.Add(entree)
            While _entrees.Count > Capacite
                _entrees.RemoveAt(0)
            End While
        End SyncLock
        RaiseEvent EntreeAjoutee(entree)
    End Sub

    ''' <summary>Copie instantanee des entrees conservees.</summary>
    Public Function Instantane() As EntreeJournal()
        SyncLock _verrou
            Return _entrees.ToArray()
        End SyncLock
    End Function

    ''' <summary>Vide le tampon.</summary>
    Public Sub Vider()
        SyncLock _verrou
            _entrees.Clear()
        End SyncLock
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Vider()
    End Sub

End Class

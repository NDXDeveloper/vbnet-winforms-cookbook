' ============================================================================
'  Resilience.vb  -  Reexecution sur fautes transitoires (retry + back-off).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Threading
Imports MySql.Data.MySqlClient

''' <summary>
''' Rejoue une operation en cas de <b>faute transitoire</b> (interbloquage,
''' expiration d'attente de verrou…) avec un delai croissant. Le predicat de
''' transitoire est remplacable (utile pour les tests).
''' </summary>
Public Module Resilience

    ''' <summary>Nombre maximal de tentatives.</summary>
    Public Property NbTentativesMax As Integer = 3
    ''' <summary>Delai de base (ms), multiplie par le numero de tentative.</summary>
    Public Property DelaiBaseMs As Integer = 50

    ''' <summary>Execute une operation renvoyant une valeur, avec reessais.</summary>
    Public Function Executer(Of T)(ByVal operation As Func(Of T),
                                   Optional ByVal estTransitoire As Func(Of Exception, Boolean) = Nothing) As T
        If estTransitoire Is Nothing Then estTransitoire = AddressOf EstTransitoireParDefaut
        Dim tentative As Integer = 0
        While True
            Try
                Return operation()
            Catch ex As Exception When estTransitoire(ex) AndAlso tentative < NbTentativesMax - 1
                tentative += 1
                Thread.Sleep(DelaiBaseMs * tentative)
            End Try
        End While
    End Function

    ''' <summary>Execute une operation sans valeur de retour, avec reessais.</summary>
    Public Sub Executer(ByVal operation As Action,
                        Optional ByVal estTransitoire As Func(Of Exception, Boolean) = Nothing)
        Executer(Of Boolean)(Function()
                                 operation()
                                 Return True
                             End Function, estTransitoire)
    End Sub

    ''' <summary>Detection par defaut : interbloquage (1213) ou attente de verrou expiree (1205).</summary>
    Public Function EstTransitoireParDefaut(ByVal ex As Exception) As Boolean
        Dim mse As MySqlException = TryCast(ex, MySqlException)
        Return mse IsNot Nothing AndAlso (mse.Number = 1213 OrElse mse.Number = 1205)
    End Function

End Module

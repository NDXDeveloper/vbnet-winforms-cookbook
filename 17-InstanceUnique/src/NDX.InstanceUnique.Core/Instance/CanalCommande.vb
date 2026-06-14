' ============================================================================
'  CanalCommande.vb  -  Communication inter-processus par tube nomme (named pipe).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.IO
Imports System.IO.Pipes
Imports System.Threading
Imports System.Threading.Tasks

''' <summary>
''' Canal de communication entre instances via un <b>tube nommé</b>. L'instance
''' primaire écoute ; une nouvelle instance lui transmet ses arguments puis se
''' retire. C'est le mécanisme classique pour « réveiller » l'application déjà
''' ouverte au lieu d'en lancer une seconde.
''' </summary>
Public NotInheritable Class CanalCommande

    Private Sub New()
    End Sub

    ''' <summary>Envoie un message à l'instance qui écoute ; renvoie False si personne n'écoute.</summary>
    Public Shared Function Envoyer(ByVal nomCanal As String, ByVal message As String, Optional ByVal timeoutMs As Integer = 2000) As Boolean
        Try
            Using client As New NamedPipeClientStream(".", nomCanal, PipeDirection.Out)
                client.Connect(timeoutMs)
                Using ecrivain As New StreamWriter(client) With {.AutoFlush = True}
                    ecrivain.WriteLine(message)
                End Using
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>Attend UN message (bloquant, avec délai) ; renvoie <c>Nothing</c> si délai dépassé.</summary>
    Public Shared Function RecevoirUn(ByVal nomCanal As String, ByVal timeoutMs As Integer) As String
        Using serveur As New NamedPipeServerStream(nomCanal, PipeDirection.In)
            Dim attente As Task = serveur.WaitForConnectionAsync()
            If Not attente.Wait(timeoutMs) Then Return Nothing
            Using lecteur As New StreamReader(serveur)
                Return lecteur.ReadLine()
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Écoute en continu les messages entrants jusqu'à annulation. Si on l'appelle
    ''' depuis le fil d'interface, <paramref name="surReception"/> y est rappelé
    ''' (l'UI peut donc être mise à jour directement).
    ''' </summary>
    Public Shared Async Function EcouterAsync(ByVal nomCanal As String, ByVal jeton As CancellationToken,
                                              ByVal surReception As Action(Of String)) As Task
        While Not jeton.IsCancellationRequested
            Try
                Using serveur As New NamedPipeServerStream(nomCanal, PipeDirection.In, 1,
                                                           PipeTransmissionMode.Byte, PipeOptions.Asynchronous)
                    Await serveur.WaitForConnectionAsync(jeton)
                    Using lecteur As New StreamReader(serveur)
                        Dim ligne As String = Await lecteur.ReadLineAsync()
                        If ligne IsNot Nothing AndAlso surReception IsNot Nothing Then surReception(ligne)
                    End Using
                End Using
            Catch ex As OperationCanceledException
                Exit While
            Catch
                ' Erreur de canal : on retente une nouvelle connexion.
            End Try
        End While
    End Function

End Class

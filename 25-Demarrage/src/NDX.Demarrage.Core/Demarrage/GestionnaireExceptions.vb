' ============================================================================
'  GestionnaireExceptions.vb  -  Capture globale des exceptions non gerees.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Windows.Forms

''' <summary>
''' Installe une capture <b>globale</b> des exceptions : celles du fil d'interface
''' (<see cref="Application.ThreadException"/>) et celles des autres fils
''' (<see cref="AppDomain.UnhandledException"/>). Plutôt que de planter, l'application
''' les journalise et en informe l'utilisateur.
''' </summary>
Public NotInheritable Class GestionnaireExceptions

    Private Sub New()
    End Sub

    ''' <summary>Installe les gestionnaires globaux ; <paramref name="surException"/> est rappelé pour chaque exception captée.</summary>
    Public Shared Sub Installer(ByVal surException As Action(Of Exception))
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        AddHandler Application.ThreadException,
            Sub(expediteur, e)
                If surException IsNot Nothing Then surException(e.Exception)
            End Sub
        AddHandler AppDomain.CurrentDomain.UnhandledException,
            Sub(expediteur, e)
                Dim ex As Exception = TryCast(e.ExceptionObject, Exception)
                If ex IsNot Nothing AndAlso surException IsNot Nothing Then surException(ex)
            End Sub
    End Sub

    ''' <summary>Description courte et lisible d'une exception (type + message). Pure.</summary>
    Public Shared Function Decrire(ByVal ex As Exception) As String
        If ex Is Nothing Then Return "(aucune exception)"
        Return ex.GetType().Name & " : " & ex.Message
    End Function

End Class

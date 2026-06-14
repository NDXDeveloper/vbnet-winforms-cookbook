Imports System.Diagnostics
Imports System.Reflection

''' <summary>
''' Outils de gestion des processus systeme.
''' </summary>
''' <remarks>
''' La methode <see cref="TuerProcessusParHandleFenetre"/> illustre la combinaison
''' d'un appel natif (<c>GetWindowThreadProcessId</c>) et de l'API managee
''' <see cref="Process"/>.
''' </remarks>
Public Module OutilsProcessus

    ''' <summary>
    ''' Termine tous les processus portant le nom indique (sans l'extension .exe).
    ''' </summary>
    ''' <param name="nomProcessus">Nom du processus (ex. "notepad").</param>
    ''' <returns>Le nombre de processus effectivement termines.</returns>
    Public Function TuerProcessusSiPresent(ByVal nomProcessus As String) As Integer
        Dim termines As Integer = 0
        Try
            For Each proc As Process In Process.GetProcessesByName(nomProcessus)
                If TuerProcessusParHandleFenetre(proc.Handle) Then termines += 1
            Next
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Try
        Return termines
    End Function

    ''' <summary>
    ''' Termine le processus proprietaire d'une fenetre, identifie par son handle.
    ''' </summary>
    ''' <returns>True si le processus a ete termine.</returns>
    Public Function TuerProcessusParHandleFenetre(ByVal hWnd As IntPtr) As Boolean
        Dim idProcessus As Integer = 0
        ' Appel natif : recupere l'ID du processus a partir du handle de fenetre.
        ApiWindows.GetWindowThreadProcessId(hWnd, idProcessus)
        If idProcessus = 0 Then Return False
        Try
            Process.GetProcessById(idProcessus).Kill()
            Return True
        Catch __ As ArgumentException
            ' Le processus n'existe deja plus.
            Return False
        Catch
            Return False
        End Try
    End Function

End Module

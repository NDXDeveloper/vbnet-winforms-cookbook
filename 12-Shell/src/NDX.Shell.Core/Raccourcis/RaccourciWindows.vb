Option Strict Off
' ============================================================================
'  RaccourciWindows.vb  -  Raccourci Windows ".lnk" via Windows Script Host (COM).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  NB : « Option Strict Off » est volontaire et limité à CE fichier. On pilote
'  l'objet COM « WScript.Shell » en LIAISON TARDIVE (late binding), ce qui évite
'  les fragiles déclarations d'interfaces COM (IShellLink) tout en restant 100 %
'  intégré à Windows. Le reste de la bibliothèque demeure en Option Strict On.
' ============================================================================

''' <summary>
''' Crée et lit des raccourcis Windows « .lnk » à l'aide de l'objet COM
''' <c>WScript.Shell</c> (présent sur toutes les installations de Windows).
''' </summary>
Public NotInheritable Class RaccourciWindows

    Private Sub New()
    End Sub

    ''' <summary>Indique si l'objet COM WScript.Shell est disponible.</summary>
    Public Shared Function EstDisponible() As Boolean
        Try
            Dim shell As Object = CreateObject("WScript.Shell")
            Return shell IsNot Nothing
        Catch
            Return False
        End Try
    End Function

    ''' <summary>Crée (ou écrase) un raccourci .lnk vers une cible.</summary>
    Public Shared Sub Creer(ByVal cheminLnk As String, ByVal cible As String,
                            Optional ByVal arguments As String = Nothing,
                            Optional ByVal description As String = Nothing,
                            Optional ByVal dossierTravail As String = Nothing)
        If String.IsNullOrWhiteSpace(cheminLnk) Then Throw New ArgumentException("Chemin du .lnk obligatoire.")
        If String.IsNullOrWhiteSpace(cible) Then Throw New ArgumentException("Cible obligatoire.")
        Dim shell As Object = CreateObject("WScript.Shell")
        Dim lnk As Object = shell.CreateShortcut(cheminLnk)
        lnk.TargetPath = cible
        If Not String.IsNullOrEmpty(arguments) Then lnk.Arguments = arguments
        If Not String.IsNullOrEmpty(description) Then lnk.Description = description
        lnk.WorkingDirectory = If(String.IsNullOrEmpty(dossierTravail), IO.Path.GetDirectoryName(cible), dossierTravail)
        lnk.Save()
    End Sub

    ''' <summary>Lit la cible (TargetPath) d'un raccourci .lnk existant.</summary>
    Public Shared Function LireCible(ByVal cheminLnk As String) As String
        Dim shell As Object = CreateObject("WScript.Shell")
        Dim lnk As Object = shell.CreateShortcut(cheminLnk)
        Return CStr(lnk.TargetPath)
    End Function

End Class

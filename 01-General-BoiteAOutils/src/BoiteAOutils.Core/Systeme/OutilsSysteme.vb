Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports System.Xml.Serialization

''' <summary>
''' Informations sur la machine / l'utilisateur, configuration de culture,
''' gestion memoire et serialisation XML.
''' </summary>
''' <remarks>
''' Informations machine/utilisateur, configuration de culture, ramasse-miettes,
''' desactivation des bips (P/Invoke) et serialisation XML.
''' </remarks>
Public Module OutilsSysteme

    ''' <summary>
    ''' Retourne la memoire physique disponible (en Mo), via un compteur de
    ''' performance Windows.
    ''' </summary>
    Public Function MemoirePhysiqueDisponible() As String
        Try
            Using compteur As New PerformanceCounter("Memory", "Available Bytes")
                Dim octets As Long = CLng(compteur.NextValue())
                Dim mo As Double = octets / 1024.0 / 1024.0
                Return "Memoire physique disponible : " & mo.ToString("N0", CultureInfo.InvariantCulture) & " Mo"
            End Using
        Catch ex As Exception
            ' Les compteurs de performance peuvent etre indisponibles : on degrade proprement.
            Return "Memoire physique disponible : indeterminee (" & ex.Message & ")"
        End Try
    End Function

    ''' <summary>Nom de l'utilisateur Windows connecte.</summary>
    Public Function NomUtilisateur() As String
        Try
            Return Environment.UserName
        Catch
            Return String.Empty
        End Try
    End Function

    ''' <summary>Nom de la machine.</summary>
    Public Function NomMachine() As String
        Try
            Return Environment.MachineName
        Catch
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' Nom de la session ; chaine vide pour une session console locale.
    ''' Utile pour detecter une session de bureau a distance.
    ''' </summary>
    Public Function NomSession() As String
        Try
            Dim s As String = Environment.GetEnvironmentVariable("SessionName")
            If String.IsNullOrEmpty(s) Then Return String.Empty
            If s.Trim().ToLowerInvariant() = "console" Then Return String.Empty
            Return s
        Catch
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' Force la culture courante (et celle de l'interface) a fr-FR, avec un
    ''' separateur decimal "." pour rester coherent avec les valeurs envoyees a
    ''' la base de donnees.
    ''' </summary>
    Public Sub DefinirCulture()
        Dim cultureFr As New CultureInfo("fr-FR")
        cultureFr.NumberFormat.NumberDecimalSeparator = "."
        Thread.CurrentThread.CurrentCulture = cultureFr
        Thread.CurrentThread.CurrentUICulture = cultureFr
    End Sub

    ''' <summary>
    ''' Force le ramasse-miettes a liberer la memoire. A reserver a des points
    ''' precis (apres liberation massive d'objets) : a utiliser avec parcimonie.
    ''' </summary>
    Public Sub ForcerRamasseMiettes()
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()
    End Sub

    ''' <summary>Desactive les bips systeme (via l'API Win32 SystemParametersInfo).</summary>
    Public Sub DesactiverBips()
        Try
            ApiWindows.SystemParametersInfo(ApiWindows.SPI_SETBEEP, 0, 0, 0)
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Try
    End Sub

    ''' <summary>Desactive l'economiseur d'ecran.</summary>
    Public Sub DesactiverEconomiseurEcran()
        ApiWindows.SystemParametersInfo(ApiWindows.SPI_SETSCREENSAVEACTIVE, 0, 0,
                                        ApiWindows.SPIF_UPDATEINIFILE Or ApiWindows.SPIF_SENDWININICHANGE)
    End Sub

    ''' <summary>Reactive l'economiseur d'ecran.</summary>
    Public Sub ReactiverEconomiseurEcran()
        ApiWindows.SystemParametersInfo(ApiWindows.SPI_SETSCREENSAVEACTIVE, 1, 0,
                                        ApiWindows.SPIF_UPDATEINIFILE Or ApiWindows.SPIF_SENDWININICHANGE)
    End Sub

    ''' <summary>Serialise un objet en XML et retourne le document sous forme de chaine.</summary>
    Public Function SerialiserEnXml(ByVal objet As Object) As String
        If objet Is Nothing Then Return String.Empty
        Dim serialiseur As New XmlSerializer(objet.GetType())
        Using ecrivain As New StringWriter()
            serialiseur.Serialize(ecrivain, objet)
            Return ecrivain.ToString()
        End Using
    End Function

End Module

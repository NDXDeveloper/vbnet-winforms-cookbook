Imports System.Configuration
Imports System.Net
Imports System.Net.Mail
Imports System.Reflection
Imports System.Text.RegularExpressions

''' <summary>
''' Outils reseau : adresse IP locale, validation d'adresse IPv4, envoi d'e-mail.
''' </summary>
''' <remarks>
''' Adresse IP locale, validation d'adresse IPv4 (format et bornes) et envoi
''' d'e-mail SMTP.
''' </remarks>
Public Module OutilsReseau

    ' Format general d'une adresse IPv4 (les bornes 0-255 sont verifiees a part).
    Private ReadOnly _regexIPv4 As New Regex("^(\d{1,3}\.){3}\d{1,3}$", RegexOptions.Compiled)

    ''' <summary>
    ''' Retourne la premiere adresse IPv4 non locale-lien associee a la machine.
    ''' </summary>
    Public Function AdresseIpLocale() As String
        Try
            Dim nomHote As String = Dns.GetHostName()
            For Each adresse As IPAddress In Dns.GetHostEntry(nomHote).AddressList
                If Not adresse.IsIPv6LinkLocal Then
                    Dim texte As String = adresse.ToString()
                    If EstAdresseIPv4Valide(texte) Then Return texte
                End If
            Next
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Try
        Return String.Empty
    End Function

    ''' <summary>
    ''' Verifie qu'une chaine est une adresse IPv4 valide (format ET bornes 0-255).
    ''' </summary>
    ''' <remarks>
    ''' On valide non seulement le format mais aussi chaque octet (0-255) : une
    ''' valeur comme 999.1.1.1 est donc rejetee.
    ''' </remarks>
    Public Function EstAdresseIPv4Valide(ByVal adresse As String) As Boolean
        If String.IsNullOrWhiteSpace(adresse) Then Return False
        If Not _regexIPv4.IsMatch(adresse) Then Return False
        For Each octetTexte As String In adresse.Split("."c)
            Dim octet As Integer
            If Not Integer.TryParse(octetTexte, octet) Then Return False
            If octet < 0 OrElse octet > 255 Then Return False
        Next
        Return True
    End Function

    ''' <summary>
    ''' Envoie un e-mail (avec piece jointe facultative) via un serveur SMTP.
    ''' </summary>
    ''' <remarks>
    ''' Hote et port sont lus dans App.config (cles <c>Smtp.Hote</c> /
    ''' <c>Smtp.Port</c>), avec repli sur localhost:25. En l'absence de serveur
    ''' SMTP, l'envoi echoue proprement (exception capturee et journalisee) : la
    ''' methode est ici a vocation de demonstration.
    ''' </remarks>
    ''' <returns>True si l'envoi a reussi.</returns>
    Public Function EnvoyerEmail(ByVal expediteur As String,
                                 ByVal destinataire As String,
                                 ByVal sujet As String,
                                 ByVal corps As String,
                                 Optional ByVal pieceJointe As String = "") As Boolean
        If String.IsNullOrWhiteSpace(expediteur) OrElse String.IsNullOrWhiteSpace(destinataire) Then
            Return False
        End If
        Try
            Dim hote As String = If(ConfigurationManager.AppSettings("Smtp.Hote"), "localhost")
            Dim port As Integer
            If Not Integer.TryParse(ConfigurationManager.AppSettings("Smtp.Port"), port) Then port = 25

            Using client As New SmtpClient(hote, port) With {.EnableSsl = False}
                Using message As New MailMessage(expediteur, destinataire, sujet, corps)
                    message.Priority = MailPriority.Normal
                    If IO.File.Exists(pieceJointe) Then
                        message.Attachments.Add(New Attachment(pieceJointe))
                    End If
                    client.Send(message)
                End Using
            End Using
            Return True
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return False
        End Try
    End Function

End Module

Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Page "Reseau et processus" : IP locale, validation IPv4, moniteur en arriere-plan, e-mail, processus.</summary>
Public NotInheritable Class PageReseauProcessus
    Inherits PageBase

    Private ReadOnly _txtIp As New TextBox()
    Private ReadOnly _txtProcessus As New TextBox()
    Private ReadOnly _lblEtatReseau As New Label()
    Private ReadOnly _console As TextBox
    Private WithEvents _moniteur As New MoniteurReseau()

    Public Sub New()
        MyBase.New("Reseau et processus",
                   "AdresseIpLocale, EstAdresseIPv4Valide, MoniteurReseau (BackgroundWorker), EnvoyerEmail, TuerProcessusSiPresent.")
        _console = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True, .WrapContents = True}

        haut.Controls.Add(Bouton("Adresse IP locale", AddressOf SurIpLocale))

        _txtIp.Width = 140
        _txtIp.Text = "192.168.1.10"
        haut.Controls.Add(_txtIp)
        haut.Controls.Add(Bouton("Valider IPv4", AddressOf SurValiderIp))

        haut.Controls.Add(Bouton("Demarrer moniteur", AddressOf SurDemarrerMoniteur))
        haut.Controls.Add(Bouton("Arreter moniteur", AddressOf SurArreterMoniteur))

        haut.Controls.Add(Bouton("Envoyer e-mail (demo)", AddressOf SurEmail))

        _txtProcessus.Width = 120
        _txtProcessus.Text = "notepad"
        haut.Controls.Add(_txtProcessus)
        haut.Controls.Add(Bouton("Tuer processus", AddressOf SurTuerProcessus))

        _lblEtatReseau.Dock = DockStyle.Top
        _lblEtatReseau.Height = 28
        _lblEtatReseau.TextAlign = ContentAlignment.MiddleLeft
        _lblEtatReseau.Font = New Font("Segoe UI Semibold", 9.5F)
        _lblEtatReseau.Text = "Etat base (moniteur) : inconnu"

        Contenu.Controls.Add(_console)
        Contenu.Controls.Add(_lblEtatReseau)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub Tracer(ByVal texte As String)
        _console.AppendText(texte & Environment.NewLine)
    End Sub

    Private Sub SurIpLocale(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("AdresseIpLocale = " & OutilsReseau.AdresseIpLocale())
    End Sub

    Private Sub SurValiderIp(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("EstAdresseIPv4Valide(""" & _txtIp.Text & """) = " & OutilsReseau.EstAdresseIPv4Valide(_txtIp.Text).ToString())
    End Sub

    Private Sub SurDemarrerMoniteur(ByVal sender As Object, ByVal e As EventArgs)
        _moniteur.Demarrer()
        Tracer("Moniteur reseau demarre (test de la base toutes les 2 s sur un thread d'arriere-plan).")
    End Sub

    Private Sub SurArreterMoniteur(ByVal sender As Object, ByVal e As EventArgs)
        _moniteur.Arreter()
        Tracer("Moniteur reseau : arret demande.")
    End Sub

    ' Recu sur le thread d'interface (BackgroundWorker.ProgressChanged).
    Private Sub SurEtatReseau(ByVal disponible As Boolean) Handles _moniteur.EtatChange
        _lblEtatReseau.ForeColor = If(disponible, Color.Green, Color.Firebrick)
        _lblEtatReseau.Text = "Etat base (moniteur) : " & If(disponible, "DISPONIBLE", "INDISPONIBLE")
    End Sub

    Private Sub SurEmail(ByVal sender As Object, ByVal e As EventArgs)
        Dim ok As Boolean = OutilsReseau.EnvoyerEmail("demo@etabli.local", "admin@etabli.local",
                                                      "Test depuis la galerie", "Ceci est un message de demonstration.")
        Tracer("EnvoyerEmail = " & ok.ToString() &
               If(ok, "", " (echec attendu en l'absence de serveur SMTP local : voir le journal)."))
    End Sub

    Private Sub SurTuerProcessus(ByVal sender As Object, ByVal e As EventArgs)
        Dim nom As String = _txtProcessus.Text.Trim()
        If nom = "" Then Return
        If MessageBox.Show("Tuer tous les processus '" & nom & "' ?", "Confirmation",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            Dim n As Integer = OutilsProcessus.TuerProcessusSiPresent(nom)
            Tracer("TuerProcessusSiPresent('" & nom & "') = " & n.ToString() & " processus termine(s).")
        End If
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            Try
                _moniteur?.Dispose()
            Catch
            End Try
        End If
        MyBase.Dispose(disposing)
    End Sub

End Class

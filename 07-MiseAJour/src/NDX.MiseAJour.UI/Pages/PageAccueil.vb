' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Configuration
Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.MiseAJour

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Mise à jour applicative : versionnage sémantique, manifeste de publication, contrôle d'intégrité.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim versionConfig As String = If(ConfigurationManager.AppSettings("App.VersionActuelle"), "1.0.0")
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.MiseAJour.Core) illustre la détection des mises à jour :" & vbCrLf & vbCrLf &
            "  - VersionSemantique : type comparable (Majeure.Mineure.Corrective)," & vbCrLf &
            "    avec IComparable, surcharge d'opérateurs et analyse tolérante (TryAnalyser) ;" & vbCrLf &
            "  - Publication : entrée du manifeste (version, notes, URL, empreinte, obligatoire) ;" & vbCrLf &
            "  - ServiceMiseAJour : trouve la dernière version applicable, repère les mises à" & vbCrLf &
            "    jour obligatoires, et vérifie l'intégrité d'un paquet (SHA-256)." & vbCrLf & vbCrLf &
            "Version « installée » simulée (App.config) : " & versionConfig & vbCrLf & vbCrLf &
            "Pages : « Versions » (comparer/ordonner) et « Publications »" & vbCrLf &
            "(manifeste en base + recherche d'une mise à jour)." & vbCrLf & vbCrLf &
            "Pré-requis base : démarrez le conteneur (docker compose up -d), puis testez ci-dessous."

        Dim barre As New Panel() With {.Dock = DockStyle.Bottom, .Height = 74}
        Dim btn As Button = Bouton("Tester la connexion à la base", AddressOf SurTest)
        btn.Location = New Point(4, 8)
        barre.Controls.Add(btn)
        barre.Controls.Add(_lblEtat)

        Contenu.Controls.Add(texte)
        Contenu.Controls.Add(barre)
    End Sub

    Private Sub SurTest(ByVal sender As Object, ByVal e As EventArgs)
        Dim message As String = ""
        Dim ok As Boolean = DepotPublication.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

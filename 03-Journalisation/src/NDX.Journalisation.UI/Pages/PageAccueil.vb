' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Journalisation

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Démonstration d'un journal applicatif : niveaux, puits multiples, rotation, base de données.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Journalisation.Core) propose un journal applicatif :" & vbCrLf & vbCrLf &
            "  - 5 niveaux de gravité (Debug, Info, Avertissement, Erreur, Critique) ;" & vbCrLf &
            "  - un journal thread-safe qui diffuse chaque entrée vers plusieurs PUITS ;" & vbCrLf &
            "  - puits fournis : mémoire (affichage live), console, fichier (avec rotation)," & vbCrLf &
            "    et base de données (MariaDB) ;" & vbCrLf &
            "  - un puits ne laisse jamais remonter d'exception : journaliser ne fait pas" & vbCrLf &
            "    tomber l'application." & vbCrLf & vbCrLf &
            "Pages : « Journalisation en direct » (émettre et observer) et « Journal en base »" & vbCrLf &
            "(relire ce qui a été enregistré dans MariaDB)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = PuitsBase.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

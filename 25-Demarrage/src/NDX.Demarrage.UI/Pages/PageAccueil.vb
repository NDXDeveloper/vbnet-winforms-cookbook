' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Demarrage

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Cycle de vie applicatif : écran de démarrage, séquence d'étapes, et capture globale des exceptions.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Demarrage.Core) structure le démarrage d'une application :" & vbCrLf & vbCrLf &
            "  - SequenceDemarrage : exécute des étapes dans l'ordre, collecte un résultat" & vbCrLf &
            "    par étape, et s'arrête (ou non) au premier échec — logique pure, testable ;" & vbCrLf &
            "  - GestionnaireExceptions : capture GLOBALE des exceptions (fil d'interface" & vbCrLf &
            "    via Application.ThreadException, autres fils via AppDomain) — l'appli" & vbCrLf &
            "    journalise et informe au lieu de planter ;" & vbCrLf &
            "  - un écran de démarrage (splash) s'affiche pendant l'initialisation ;" & vbCrLf &
            "  - DepotEvenement : journalise étapes et exceptions en base." & vbCrLf & vbCrLf &
            "La capture globale est installée au démarrage (Program.Main), avant toute fenêtre." & vbCrLf & vbCrLf &
            "Pages : « Séquence de démarrage » et « Exceptions & journal »." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotEvenement.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

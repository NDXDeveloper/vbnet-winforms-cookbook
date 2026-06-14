' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.InstanceUnique

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Instance unique (Mutex) et communication inter-processus (tube nommé) : réveiller l'application déjà ouverte.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.InstanceUnique.Core) illustre l'instance unique :" & vbCrLf & vbCrLf &
            "  - VerrouInstance : un Mutex nommé ; la première instance le crée" & vbCrLf &
            "    (EstPremiere = vrai), les suivantes constatent qu'il existe déjà ;" & vbCrLf &
            "  - CanalCommande : un tube nommé (named pipe) — une 2ᵉ instance transmet" & vbCrLf &
            "    ses arguments à l'instance qui écoute, puis se retire ;" & vbCrLf &
            "  - Commande : encode/décode une liste d'arguments en une ligne" & vbCrLf &
            "    (échappement) — logique pure, testable ;" & vbCrLf &
            "  - DepotCommande : journalise en base les commandes reçues." & vbCrLf & vbCrLf &
            "Pages : « Instance & messages » (écouter / simuler une 2ᵉ instance) et" & vbCrLf &
            "« Journal » (base)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotCommande.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

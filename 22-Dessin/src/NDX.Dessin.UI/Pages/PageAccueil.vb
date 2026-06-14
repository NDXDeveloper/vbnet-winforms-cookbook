' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Dessin

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Éditeur de formes vectorielles : dessin GDI+, test de survol (hit-testing), déplacement et sérialisation.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Dessin.Core) modélise un dessin vectoriel :" & vbCrLf & vbCrLf &
            "  - Forme : rectangle / ellipse / ligne, avec test de contenance" & vbCrLf &
            "    (hit-testing) — « ce point est-il sur moi ? » : logique pure, testable ;" & vbCrLf &
            "  - Scene : collection ordonnée de formes ; TrouverA renvoie la forme la" & vbCrLf &
            "    plus haute sous un point (sélection) ;" & vbCrLf &
            "  - DepotForme : enregistre / recharge la scène (transaction)." & vbCrLf & vbCrLf &
            "Côté interface, un Canevas en double tampon trace les formes au glisser et" & vbCrLf &
            "les déplace au clic (hit-testing)." & vbCrLf & vbCrLf &
            "Pages : « Canevas » (dessiner, déplacer, enregistrer) et « Formes » (base)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotForme.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.ControlesII

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Contrôles avancés : bouton à états (owner-draw + IButtonControl), onglets peints, grille pré-stylée.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.ControlesII.Core) propose des contrôles soignés :" & vbCrLf & vbCrLf &
            "  - BoutonEtat : bouton dessiné à la main qui change selon son état" & vbCrLf &
            "    (normal / survol / enfoncé / désactivé), et implémente IButtonControl ;" & vbCrLf &
            "  - CalculEtat : la machine à états du bouton, isolée et testable ;" & vbCrLf &
            "  - OngletsPeints : un TabControl en owner-draw (onglet actif accentué) ;" & vbCrLf &
            "  - GrillePersonnalisee : un DataGridView pré-stylé (en-tête, lignes alternées) ;" & vbCrLf &
            "  - DepotArticle : les données affichées par la grille." & vbCrLf & vbCrLf &
            "Pages : « Boutons à états », « Onglets peints » et « Grille »." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotArticle.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

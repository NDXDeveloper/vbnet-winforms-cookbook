' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.FormBase

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Héritage visuel de formulaires (fiche de base réutilisable) et thèmes (clair / sombre) appliqués par récursion.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.FormBase.Core) factorise l'apparence des fiches :" & vbCrLf & vbCrLf &
            "  - FormulaireBase : une fiche de base (en-tête + contenu + thème) dont" & vbCrLf &
            "    héritent les fiches concrètes — c'est l'héritage visuel ;" & vbCrLf &
            "  - Theme : un jeu de couleurs (fond, texte, accent) ;" & vbCrLf &
            "  - GestionnaireThemes : thèmes Clair / Sombre + application récursive" & vbCrLf &
            "    à tous les contrôles d'une fenêtre ;" & vbCrLf &
            "  - CouleurHex : conversion Color <-> « #RRGGBB » (pur, testable) ;" & vbCrLf &
            "  - DepotTheme : thèmes persistés en base (couleurs en hexadécimal)." & vbCrLf & vbCrLf &
            "Pages : « Thèmes & héritage » (aperçu, fiche héritée) et « Catalogue » (base)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotTheme.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

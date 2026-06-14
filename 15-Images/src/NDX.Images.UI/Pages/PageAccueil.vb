' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Images

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Traitement d'images avec GDI+ : filtres par matrice de couleur et génération de vignettes.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Images.Core) traite les images via GDI+ :" & vbCrLf & vbCrLf &
            "  - Filtres : niveaux de gris, négatif, luminosité — par ColorMatrix" & vbCrLf &
            "    (transformation linéaire 5×5 appliquée à chaque pixel) ;" & vbCrLf &
            "  - CalculVignette : dimensions d'une vignette (ratio conservé, sans" & vbCrLf &
            "    agrandissement) — calcul pur, testable sans pixels ;" & vbCrLf &
            "  - Vignette : redimensionnement de qualité (bicubique) + export PNG ;" & vbCrLf &
            "  - DepotImage : médiathèque (nom, dimensions, vignette PNG en base)." & vbCrLf & vbCrLf &
            "Pages : « Filtres » (avant/après) et « Médiathèque » (ajouter / relire)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotImage.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

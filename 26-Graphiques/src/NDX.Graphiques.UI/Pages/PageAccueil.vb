' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Graphiques

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Visualisation de données : un contrôle owner-draw trace une série en barres, courbe ou points.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Graphiques.Core) trace des séries de données :" & vbCrLf & vbCrLf &
            "  - EchelleGraphique : convertit une valeur en ordonnée pixel (axes, bornes" & vbCrLf &
            "    min/max, échelle automatique) — calcul PUR, testable sans dessin ;" & vbCrLf &
            "  - SerieDonnees : des couples (libellé, valeur) et une couleur ;" & vbCrLf &
            "  - ControleGraphique : un contrôle owner-draw qui dessine la série en" & vbCrLf &
            "    barres, courbe ou points (double tampon) ;" & vbCrLf &
            "  - DepotMesure : la série lue depuis la base." & vbCrLf & vbCrLf &
            "La différence avec l'éditeur de formes : ici on ne dessine pas des formes" & vbCrLf &
            "libres, on PROJETTE des données (mise à l'échelle valeur → pixel)." & vbCrLf & vbCrLf &
            "Pages : « Graphique » (barres / courbe / points) et « Mesures » (base)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotMesure.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

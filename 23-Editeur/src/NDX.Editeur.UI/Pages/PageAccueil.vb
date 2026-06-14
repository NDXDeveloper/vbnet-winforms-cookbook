' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Editeur

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Éditeur de texte enrichi (RTF) : mise en forme, statistiques en direct, enregistrement en base.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Editeur.Core) accompagne un éditeur de texte enrichi :" & vbCrLf & vbCrLf &
            "  - StatistiquesTexte : compte mots, caractères et lignes — calcul pur," & vbCrLf &
            "    indépendant du contrôle d'édition, donc testable directement ;" & vbCrLf &
            "  - DepotDocument : enregistre / relit le contenu au format RTF (texte" & vbCrLf &
            "    enrichi : gras, italique, couleurs…) en base." & vbCrLf & vbCrLf &
            "Côté interface, un RichTextBox + une barre de mise en forme manipulent" & vbCrLf &
            "SelectionFont / SelectionColor ; les statistiques se mettent à jour à chaque frappe." & vbCrLf & vbCrLf &
            "Pages : « Éditeur RTF » (rédiger, mettre en forme, enregistrer) et" & vbCrLf &
            "« Documents » (liste en base)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotDocument.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

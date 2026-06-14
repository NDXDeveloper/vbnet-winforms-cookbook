' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Arborescence

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Données hiérarchiques : liste d'adjacence (parent_id), reconstruction d'arbre, TreeView et requête récursive.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Arborescence.Core) gère les données hiérarchiques :" & vbCrLf & vbCrLf &
            "  - en base, l'arbre est stocké « à plat » : chaque ligne connaît son" & vbCrLf &
            "    parent (parent_id) — c'est la liste d'adjacence ;" & vbCrLf &
            "  - ConstructeurArbre : reconstruit l'arbre (racines + enfants) en deux" & vbCrLf &
            "    passes O(n) — logique pure, testable ;" & vbCrLf &
            "  - DepotNoeud.Descendants : récupère tous les descendants d'un nœud via" & vbCrLf &
            "    une requête RÉCURSIVE (WITH RECURSIVE), gérée par MariaDB ;" & vbCrLf &
            "  - l'UI projette l'arbre dans un TreeView." & vbCrLf & vbCrLf &
            "Pages : « Arbre » (afficher / ajouter / supprimer) et « Descendants »" & vbCrLf &
            "(requête récursive depuis un nœud choisi)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotNoeud.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

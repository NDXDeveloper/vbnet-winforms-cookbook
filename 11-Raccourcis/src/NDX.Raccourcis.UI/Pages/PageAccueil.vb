' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Raccourcis

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Raccourcis clavier, y compris les accords à plusieurs frappes (ex. Ctrl+K, Ctrl+S), avec détection de conflits.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Raccourcis.Core) gère les raccourcis clavier :" & vbCrLf & vbCrLf &
            "  - Combinaison : une frappe « Ctrl+Maj+P » (analyse, forme canonique) ;" & vbCrLf &
            "  - Raccourci : une suite de combinaisons, ex. « Ctrl+K, Ctrl+S » (accord) ;" & vbCrLf &
            "  - GestionnaireRaccourcis : reconnaît les frappes via une machine à états" & vbCrLf &
            "    (déclenché / en attente / aucun) et détecte les conflits de préfixe ;" & vbCrLf &
            "  - DepotRaccourci : persiste les liaisons action ↔ raccourci en base." & vbCrLf & vbCrLf &
            "L'analyse accepte des alias français : « Maj », « Entrée », « Échap », « Suppr »…" & vbCrLf & vbCrLf &
            "Pages : « Démonstration » (capture clavier en direct) et « Liaisons » (base)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotRaccourci.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

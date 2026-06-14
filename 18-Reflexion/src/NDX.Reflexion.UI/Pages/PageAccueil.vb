' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Reflexion

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Réflexion : inspecter un type, copier des propriétés par nom, purger les abonnés d'un événement.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Reflexion.Core) explore la métaprogrammation :" & vbCrLf & vbCrLf &
            "  - InspecteurType : liste propriétés, champs et événements d'un type" & vbCrLf &
            "    via la réflexion et des BindingFlags ;" & vbCrLf &
            "  - CopieurProprietes : copie les propriétés de même nom et de type" & vbCrLf &
            "    compatible d'un objet vers un autre (mapping automatique) ;" & vbCrLf &
            "  - PurgeEvenements : retire d'un coup tous les abonnés d'un événement" & vbCrLf &
            "    (atteint le champ délégué et le remet à Nothing) ;" & vbCrLf &
            "  - DepotMetadonnees : catalogue les membres d'un type en base." & vbCrLf & vbCrLf &
            "Pages : « Inspecteur de type », « Outils (copie, purge) » et « Catalogue »." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotMetadonnees.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

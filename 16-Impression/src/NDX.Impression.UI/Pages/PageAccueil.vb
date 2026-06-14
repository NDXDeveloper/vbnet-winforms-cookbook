' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Impression

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Impression de documents : pagination, aperçu (PrintPreview) et impression (PrintDocument).")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Impression.Core) gère l'impression de texte :" & vbCrLf & vbCrLf &
            "  - Paginateur : combien de lignes par page, combien de pages, et quelles" & vbCrLf &
            "    lignes pour une page donnée — calcul pur, testable sans imprimante ;" & vbCrLf &
            "  - ImprimeurTexte : construit un PrintDocument qui pagine automatiquement" & vbCrLf &
            "    (dessine les lignes tant qu'elles tiennent, puis HasMorePages) ;" & vbCrLf &
            "  - DepotImpression : conserve les documents soumis à l'impression." & vbCrLf & vbCrLf &
            "L'aperçu utilise PrintPreviewDialog ; l'impression, PrintDialog + PrintDocument." & vbCrLf & vbCrLf &
            "Pages : « Aperçu & impression » et « Historique » (base)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotImpression.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

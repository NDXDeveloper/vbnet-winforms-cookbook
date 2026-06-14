' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Pdf

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Composition de documents PDF : multi-pages, polices standard, retour à la ligne, métadonnées — sans dépendance.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Pdf.Core) compose des fichiers PDF entièrement en VB.NET :" & vbCrLf & vbCrLf &
            "  - DocumentPdf : assemble un PDF valide (en-tête, objets, table xref, trailer) ;" & vbCrLf &
            "  - PagePdf : API de dessin (texte, lignes, rectangles) en repère haut-gauche ;" & vbCrLf &
            "  - PoliceStandard : les polices de base PDF (Helvetica, Times, Courier) en WinAnsi ;" & vbCrLf &
            "  - EnrouleurTexte : retour à la ligne exact pour Courier (chasse fixe), testable ;" & vbCrLf &
            "  - DepotDocument : archive les PDF produits en base (binaire + empreinte SHA-256)." & vbCrLf & vbCrLf &
            "Aucune bibliothèque tierce ni binaire natif : le PDF est généré octet par octet." & vbCrLf & vbCrLf &
            "Pages : « Composer un PDF » (générer, enregistrer, ouvrir, archiver) et" & vbCrLf &
            "« Bibliothèque » (relire les documents archivés)." & vbCrLf & vbCrLf &
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

' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Compression

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Compression de données : algorithmes GZip/Deflate, ratio de gain, et stockage compressé en base.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Compression.Core) illustre la compression de données :" & vbCrLf & vbCrLf &
            "  - compression / décompression d'octets et de texte (UTF-8) ;" & vbCrLf &
            "  - deux algorithmes intégrés à .NET : DEFLATE (RFC 1951) et GZIP (RFC 1952) ;" & vbCrLf &
            "  - calcul du ratio et du pourcentage de gain de place ;" & vbCrLf &
            "  - technique « compress-then-store » : stocker en base la version compressée" & vbCrLf &
            "    avec tailles d'origine / compressée + empreinte SHA-256 de contrôle." & vbCrLf & vbCrLf &
            "Pages : « Compression (mémoire) » (essais sans base) et « Archives (base) »" & vbCrLf &
            "(enregistrer, lister, recharger et vérifier l'intégrité)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotArchive.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

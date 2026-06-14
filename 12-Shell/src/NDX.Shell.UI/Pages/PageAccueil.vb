' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Shell

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Interopérabilité avec le shell Windows : raccourcis d'applications (.lnk) et liens Web (.url).")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Shell.Core) crée et lit des raccourcis Windows :" & vbCrLf & vbCrLf &
            "  - RaccourciWindows : fichiers « .lnk » via l'objet COM WScript.Shell" & vbCrLf &
            "    (liaison tardive — aucune déclaration d'interface COM fragile) ;" & vbCrLf &
            "  - RaccourciInternet : fichiers « .url » (format INI), génération et" & vbCrLf &
            "    analyse en pur texte, donc testables sans toucher au disque ;" & vbCrLf &
            "  - DepotLien : catalogue de liens en base, exportable en fichiers réels." & vbCrLf & vbCrLf &
            "Un « .lnk » pointe vers un exécutable (avec arguments, dossier de travail) ;" & vbCrLf &
            "un « .url » pointe vers une adresse Web." & vbCrLf & vbCrLf &
            "Pages : « Créer / lire un raccourci » et « Catalogue » (base + export sur le Bureau)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotLien.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

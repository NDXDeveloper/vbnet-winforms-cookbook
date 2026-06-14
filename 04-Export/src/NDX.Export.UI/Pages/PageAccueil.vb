' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Export

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Exporter un jeu de données vers CSV, Excel (.xlsx) ou PDF — sans Excel ni bibliothèque tierce.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Export.Core) convertit un DataTable en document :" & vbCrLf & vbCrLf &
            "  - CSV : texte séparé, échappement RFC 4180, BOM UTF-8 (lecture Excel correcte) ;" & vbCrLf &
            "  - Excel .xlsx : assemblé à la main au format Office Open XML (une archive ZIP" & vbCrLf &
            "    de parties XML), sans Excel installé ni dépendance ;" & vbCrLf &
            "  - PDF : généré à la main (objets, table xref, police Courier), tableau paginé." & vbCrLf & vbCrLf &
            "Toutes les sorties partagent la même interface IExportateur : on choisit le format," & vbCrLf &
            "le reste est identique." & vbCrLf & vbCrLf &
            "Page « Exporter des données » : choisissez une source (base entrepot) et un format," & vbCrLf &
            "prévisualisez, puis exportez vers un fichier." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = SourceDonnees.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion + detection d'Excel.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Office

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Tableur .xlsx : lecture/écriture sans Office (OpenXML), et pilotage d'Excel par COM si disponible.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim dispo As String = If(AutomationExcel.EstDisponible(), "OUI", "NON (la voie « sans Office » reste disponible)")
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Office.Core) manipule des classeurs .xlsx :" & vbCrLf & vbCrLf &
            "  - ClasseurXlsx : lit et écrit un .xlsx SANS Office (un .xlsx est une" & vbCrLf &
            "    archive ZIP de parties XML — OpenXML) ;" & vbCrLf &
            "  - ReferenceCellule : conversion « A1 » <-> (colonne, ligne), base 26" & vbCrLf &
            "    bijective — logique pure, testable ;" & vbCrLf &
            "  - AutomationExcel : pilotage d'Excel par COM (liaison tardive) — voie" & vbCrLf &
            "    OPTIONNELLE, utilisable seulement si Excel est installé ;" & vbCrLf &
            "  - DepotClasseur : une table à exporter / importer en .xlsx." & vbCrLf & vbCrLf &
            "Excel installé sur ce poste : " & dispo & "." & vbCrLf & vbCrLf &
            "Pages : « Tableur (.xlsx) » (écrire/lire) et « Données » (base <-> xlsx)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotClasseur.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

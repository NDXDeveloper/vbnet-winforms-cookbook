' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Controles

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Contrôles WinForms personnalisés : dessin propriétaire (owner-draw), double tampon, zoom/déplacement.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Controles.Core) illustre les contrôles personnalisés :" & vbCrLf & vbCrLf &
            "  - BoutonBascule : interrupteur peint à la main (OnPaint), double tampon" & vbCrLf &
            "    anti-scintillement, propriété + événement personnalisés ;" & vbCrLf &
            "  - VisionneuseImage : zoom à la molette et déplacement à la souris," & vbCrLf &
            "    via une transformation géométrique (translation + échelle) ;" & vbCrLf &
            "  - CalculZoom : l'arithmétique du zoom, isolée et testable sans interface ;" & vbCrLf &
            "  - DepotPreferences : persistance clef/valeur de l'état des contrôles (upsert)." & vbCrLf & vbCrLf &
            "Pages : « Interrupteur », « Visionneuse » et « Préférences »." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotPreferences.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

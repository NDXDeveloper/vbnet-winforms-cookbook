' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Asynchrone

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Traitements asynchrones : interface réactive, progression et annulation coopérative.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Asynchrone.Core) illustre l'exécution asynchrone :" & vbCrLf & vbCrLf &
            "  - MoteurTaches.ExecuterAsync : traite une collection en arrière-plan" & vbCrLf &
            "    (Async/Await + Task.Run) sans figer l'interface ;" & vbCrLf &
            "  - progression via IProgress(Of Avancement) (mise à jour de l'UI sur le bon fil) ;" & vbCrLf &
            "  - annulation coopérative via CancellationToken ;" & vbCrLf &
            "  - CalculAvancement : le pourcentage, isolé et testable ;" & vbCrLf &
            "  - DepotTache : une file d'attente persistée, vidée en arrière-plan." & vbCrLf & vbCrLf &
            "Pages : « Progression / annulation » (tâche longue interruptible) et" & vbCrLf &
            "« File de tâches » (empiler puis drainer la file de façon asynchrone)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotTache.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Interop

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Interopérabilité avec l'API Windows (P/Invoke) : inactivité, objets GDI/USER, fenêtre au premier plan.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Interop.Core) appelle des fonctions natives Windows :" & vbCrLf & vbCrLf &
            "  - Inactivite : durée depuis la dernière entrée (GetLastInputInfo), avec" & vbCrLf &
            "    gestion du repli du compteur de ticks 32 bits (calcul isolé, testable) ;" & vbCrLf &
            "  - RessourcesProcessus : nombre d'objets GDI / USER (GetGuiResources)," & vbCrLf &
            "    pour diagnostiquer les fuites de handles graphiques ;" & vbCrLf &
            "  - FenetrePremierPlan : « toujours au premier plan » (SetWindowPos) ;" & vbCrLf &
            "  - DepotSupervision : historise les relevés en base." & vbCrLf & vbCrLf &
            "Chaque appel passe par une déclaration DllImport (P/Invoke) avec le bon" & vbCrLf &
            "marshalling (StructLayout, types non signés)." & vbCrLf & vbCrLf &
            "Pages : « Supervision » (relevés en temps réel) et « Historique » (base)." & vbCrLf & vbCrLf &
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
        Dim ok As Boolean = DepotSupervision.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

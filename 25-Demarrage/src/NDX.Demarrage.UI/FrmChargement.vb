' ============================================================================
'  FrmChargement.vb  -  Ecran de demarrage (splash) sans bordure.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Écran de démarrage affiché pendant l'initialisation de l'application.</summary>
Public NotInheritable Class FrmChargement
    Inherits Form

    Public Sub New()
        Me.FormBorderStyle = FormBorderStyle.None
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(440, 200)
        Me.BackColor = FrmPrincipale.CouleurAccent

        Dim lblTitre As New Label() With {
            .Text = "Démarrage de l'application", .Dock = DockStyle.Top, .Height = 90,
            .ForeColor = Color.White, .Font = New Font("Segoe UI Light", 18.0F),
            .TextAlign = ContentAlignment.MiddleCenter}
        Dim lblMessage As New Label() With {
            .Text = "Initialisation en cours…", .Dock = DockStyle.Fill,
            .ForeColor = Color.Gainsboro, .Font = New Font("Segoe UI", 10.0F),
            .TextAlign = ContentAlignment.TopCenter}

        Me.Controls.Add(lblMessage)
        Me.Controls.Add(lblTitre)
    End Sub

End Class

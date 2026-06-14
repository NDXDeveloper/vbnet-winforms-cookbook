' ============================================================================
'  PageOnglets.vb  -  Demo du TabControl peint (OngletsPeints).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.ControlesII

''' <summary>Affiche un TabControl aux onglets dessinés sur mesure.</summary>
Public NotInheritable Class PageOnglets
    Inherits PageBase

    Public Sub New()
        MyBase.New("Onglets peints", "TabControl en owner-draw : l'onglet actif est accentué et en gras.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim onglets As New OngletsPeints() With {.Dock = DockStyle.Fill, .CouleurAccent = FrmPrincipale.CouleurAccent}
        For Each titre As String In New String() {"Résumé", "Détails", "Paramètres"}
            Dim page As New TabPage(titre) With {.BackColor = Color.White, .Padding = New Padding(12)}
            page.Controls.Add(New Label() With {
                .Text = "Contenu de l'onglet « " & titre & " ».", .AutoSize = True,
                .Location = New Point(14, 14), .Font = New Font("Segoe UI", 10.0F)})
            onglets.TabPages.Add(page)
        Next
        Contenu.Controls.Add(onglets)
    End Sub

End Class

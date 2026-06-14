' ============================================================================
'  FrmExemple.vb  -  Fiche concrete heritant de FormulaireBase (heritage visuel).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.FormBase

''' <summary>
''' Fiche de démonstration : elle <b>hérite</b> de <see cref="FormulaireBase"/>,
''' donc reçoit gratuitement l'en-tête, la zone de contenu et le thème. Elle ne
''' fait qu'ajouter ses propres contrôles.
''' </summary>
Public NotInheritable Class FrmExemple
    Inherits FormulaireBase

    Public Sub New(ByVal titre As String, ByVal theme As Theme)
        Me.TitreFiche = titre

        Dim lbl As New Label() With {
            .Text = "Cette fiche hérite de FormulaireBase : en-tête, contenu et thème sont fournis par la classe de base.",
            .AutoSize = False, .Dock = DockStyle.Top, .Height = 48}
        Dim champ As New TextBox() With {.Dock = DockStyle.Top, .Text = "Champ de saisie d'exemple"}
        Dim espace As New Panel() With {.Dock = DockStyle.Top, .Height = 8}
        Dim fermer As New Button() With {.Text = "Fermer", .AutoSize = True, .Dock = DockStyle.Top}
        AddHandler fermer.Click, Sub(s, e) Me.Close()

        ' L'ordre d'ajout (Dock=Top) empile de haut en bas en sens inverse.
        Me.Contenu.Controls.Add(fermer)
        Me.Contenu.Controls.Add(espace)
        Me.Contenu.Controls.Add(champ)
        Me.Contenu.Controls.Add(lbl)

        ' On applique le thème APRÈS avoir ajouté les contrôles (pour les colorer aussi).
        AppliquerTheme(theme)
    End Sub

End Class

' ============================================================================
'  PageThemes.vb  -  Apercu des themes (en direct) + ouverture d'une fiche heritee.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.FormBase

''' <summary>Applique un thème à un aperçu et ouvre une fiche héritée avec ce thème.</summary>
Public NotInheritable Class PageThemes
    Inherits PageBase

    Private ReadOnly _apercu As New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
    Private ReadOnly _lblApercu As New Label() With {.Text = "Aperçu du thème", .AutoSize = True, .Location = New Point(12, 12), .Font = New Font("Segoe UI Semibold", 12.0F)}
    Private ReadOnly _txtApercu As New TextBox() With {.Location = New Point(12, 48), .Width = 280, .Text = "Texte d'exemple"}
    Private ReadOnly _btnApercu As New Button() With {.Text = "Bouton", .Location = New Point(12, 84), .AutoSize = True}
    Private _themeCourant As Theme = GestionnaireThemes.Clair

    Public Sub New()
        MyBase.New("Thèmes & héritage", "Choisissez un thème (appliqué en direct à l'aperçu), puis ouvrez une fiche qui hérite de FormulaireBase.")
        Construire()
        Appliquer(GestionnaireThemes.Clair)
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(Bouton("Thème clair", AddressOf SurClair))
        haut.Controls.Add(Bouton("Thème sombre", AddressOf SurSombre))
        haut.Controls.Add(Bouton("Ouvrir une fiche héritée", AddressOf SurFiche))

        _apercu.Controls.Add(_btnApercu)
        _apercu.Controls.Add(_txtApercu)
        _apercu.Controls.Add(_lblApercu)

        Contenu.Controls.Add(_apercu)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub Appliquer(ByVal theme As Theme)
        _themeCourant = theme
        GestionnaireThemes.AppliquerSur(_apercu, theme)
    End Sub

    Private Sub SurClair(ByVal sender As Object, ByVal e As EventArgs)
        Appliquer(GestionnaireThemes.Clair)
    End Sub

    Private Sub SurSombre(ByVal sender As Object, ByVal e As EventArgs)
        Appliquer(GestionnaireThemes.Sombre)
    End Sub

    Private Sub SurFiche(ByVal sender As Object, ByVal e As EventArgs)
        Using fiche As New FrmExemple("Fiche héritée — thème « " & _themeCourant.Nom & " »", _themeCourant)
            fiche.ShowDialog(Me)
        End Using
    End Sub

End Class

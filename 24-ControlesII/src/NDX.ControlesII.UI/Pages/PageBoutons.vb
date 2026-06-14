' ============================================================================
'  PageBoutons.vb  -  Demo du BoutonEtat (survol / enfonce / desactive).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.ControlesII

''' <summary>Affiche des BoutonEtat et signale l'action ; l'un d'eux est désactivé.</summary>
Public NotInheritable Class PageBoutons
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 10.0F), .ForeColor = Color.DimGray}

    Public Sub New()
        MyBase.New("Boutons à états", "Survolez, cliquez : la couleur reflète l'état. Le 3ᵉ bouton est désactivé.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim valider As New BoutonEtat() With {.Text = "Valider", .Location = New Point(20, 20)}
        valider.CouleurNormale = Color.FromArgb(67, 160, 71)
        valider.CouleurSurvol = Color.FromArgb(102, 187, 106)
        valider.CouleurEnfonce = Color.FromArgb(46, 125, 50)
        AddHandler valider.Click, Sub(s, e) Afficher("Valider")

        Dim annuler As New BoutonEtat() With {.Text = "Annuler", .Location = New Point(190, 20)}
        AddHandler annuler.Click, Sub(s, e) Afficher("Annuler")

        Dim desactive As New BoutonEtat() With {.Text = "Désactivé", .Location = New Point(360, 20), .Enabled = False}

        _lblEtat.Location = New Point(20, 80)
        _lblEtat.Text = "Cliquez un bouton…"

        Contenu.Controls.Add(valider)
        Contenu.Controls.Add(annuler)
        Contenu.Controls.Add(desactive)
        Contenu.Controls.Add(_lblEtat)
    End Sub

    Private Sub Afficher(ByVal action As String)
        _lblEtat.ForeColor = Color.FromArgb(2, 136, 209)
        _lblEtat.Text = "Action déclenchée : " & action
    End Sub

End Class

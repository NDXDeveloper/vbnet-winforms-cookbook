' ============================================================================
'  FrmPrincipale.vb  -  Fenetre principale : navigation entre les pages.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms

Public Class FrmPrincipale
    Inherits Form

    Private ReadOnly _lstNav As New ListBox()
    Private ReadOnly _pnlHote As New Panel()

    Public Sub New()
        InitialiserComposants()
        RemplirNavigation()
    End Sub

    Private Sub InitialiserComposants()
        Me.Text = "Mise à jour applicative - galerie de demonstration"
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.Size = New Size(1080, 700)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MinimumSize = New Size(900, 560)

        _pnlHote.Dock = DockStyle.Fill
        _pnlHote.Padding = New Padding(8)
        _pnlHote.BackColor = Color.White

        Dim pnlGauche As New Panel() With {.Dock = DockStyle.Left, .Width = 240, .BackColor = Color.FromArgb(238, 242, 248)}
        Dim lblTitre As New Label() With {
            .Text = "Mises à jour", .Dock = DockStyle.Top, .Height = 44,
            .TextAlign = ContentAlignment.MiddleCenter, .Font = New Font("Segoe UI Semibold", 11.0F), .ForeColor = CouleurAccent}
        _lstNav.Dock = DockStyle.Fill
        _lstNav.BorderStyle = BorderStyle.None
        _lstNav.Font = New Font("Segoe UI", 10.0F)
        _lstNav.IntegralHeight = False
        _lstNav.ItemHeight = 28
        AddHandler _lstNav.SelectedIndexChanged, AddressOf SurChangementPage
        pnlGauche.Controls.Add(_lstNav)
        pnlGauche.Controls.Add(lblTitre)

        Me.Controls.Add(_pnlHote)
        Me.Controls.Add(pnlGauche)
    End Sub

    Private Sub RemplirNavigation()
        _lstNav.Items.Add(New ElementNav("Accueil", Function() New PageAccueil()))
        _lstNav.Items.Add(New ElementNav("Versions (comparaison)", Function() New PageVersions()))
        _lstNav.Items.Add(New ElementNav("Publications (base)", Function() New PagePublications()))
        _lstNav.SelectedIndex = 0
    End Sub

    Private Sub SurChangementPage(ByVal sender As Object, ByVal e As EventArgs)
        Dim element As ElementNav = TryCast(_lstNav.SelectedItem, ElementNav)
        If element Is Nothing Then Return
        Dim page As UserControl = element.Obtenir()
        page.Dock = DockStyle.Fill
        _pnlHote.SuspendLayout()
        _pnlHote.Controls.Clear()
        _pnlHote.Controls.Add(page)
        _pnlHote.ResumeLayout()
    End Sub

    Public Shared ReadOnly Property CouleurAccent As Color
        Get
            Return Color.FromArgb(255, 33, 87, 158)
        End Get
    End Property

    Private NotInheritable Class ElementNav
        Private ReadOnly _titre As String
        Private ReadOnly _fabrique As Func(Of UserControl)
        Private _instance As UserControl
        Public Sub New(ByVal titre As String, ByVal fabrique As Func(Of UserControl))
            _titre = titre
            _fabrique = fabrique
        End Sub
        Public Function Obtenir() As UserControl
            If _instance Is Nothing Then _instance = _fabrique()
            Return _instance
        End Function
        Public Overrides Function ToString() As String
            Return "   " & _titre
        End Function
    End Class

End Class

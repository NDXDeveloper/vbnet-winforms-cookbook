' ============================================================================
'  FrmPrincipale.vb
'  Fenetre principale : navigation entre les pages de demonstration.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Fenetre principale de la galerie. L'interface est construite par code, ce qui
''' rend la creation des controles entierement lisible (propos pedagogique).
''' </summary>
Public Class FrmPrincipale
    Inherits Form

    Private ReadOnly _lstNav As New ListBox()
    Private ReadOnly _pnlHote As New Panel()

    Public Sub New()
        InitialiserComposants()
        RemplirNavigation()
    End Sub

    Private Sub InitialiserComposants()
        Me.Text = "Serialisation d'objets - galerie de demonstration"
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.Size = New Size(1080, 700)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MinimumSize = New Size(880, 560)

        _pnlHote.Dock = DockStyle.Fill
        _pnlHote.Padding = New Padding(8)
        _pnlHote.BackColor = Color.White

        Dim pnlGauche As New Panel() With {.Dock = DockStyle.Left, .Width = 250, .BackColor = Color.FromArgb(245, 246, 250)}
        Dim lblTitre As New Label() With {
            .Text = "Techniques de sérialisation",
            .Dock = DockStyle.Top, .Height = 44, .TextAlign = ContentAlignment.MiddleCenter,
            .Font = New Font("Segoe UI Semibold", 11.0F), .ForeColor = CouleurAccent}
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
        _lstNav.Items.Add(New ElementNav("Sérialisation et formats", Function() New PageSerialisation()))
        _lstNav.Items.Add(New ElementNav("Fichiers et stockage isolé", Function() New PageFichiers()))
        _lstNav.Items.Add(New ElementNav("Persistance en base", Function() New PagePersistance()))
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

    ''' <summary>Couleur d'accent commune a l'interface.</summary>
    Public Shared ReadOnly Property CouleurAccent As Color
        Get
            Return Color.FromArgb(255, 33, 72, 110)
        End Get
    End Property

    ''' <summary>Element de navigation : titre affiche + fabrique de page (mise en cache).</summary>
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

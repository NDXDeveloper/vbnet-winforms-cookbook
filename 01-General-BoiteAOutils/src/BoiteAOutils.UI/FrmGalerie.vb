Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Fenetre principale de la galerie : navigation entre les pages de
''' demonstration et affichage en direct du journal applicatif.
''' </summary>
''' <remarks>
''' L'interface est construite par code (et non via le concepteur visuel) :
''' ce choix rend la creation des controles entierement lisible, ce qui sert
''' le propos pedagogique du projet. La fenetre s'abonne a l'evenement
''' <see cref="Journal.LigneAjoutee"/> pour afficher, au fil de l'eau, le cycle
''' de vie des connexions et des requetes.
''' </remarks>
Public Class FrmGalerie
    Inherits Form

    Private ReadOnly _lstNav As New ListBox()
    Private ReadOnly _pnlHote As New Panel()
    Private ReadOnly _txtJournal As New TextBox()

    Public Sub New()
        InitialiserComposants()
        RemplirNavigation()
    End Sub

    ''' <summary>Construit l'ensemble des controles de la fenetre.</summary>
    Private Sub InitialiserComposants()
        Me.Text = "Boite a outils - techniques de programmation .NET"
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.Size = New Size(1120, 740)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MinimumSize = New Size(900, 600)

        ' --- Zone centrale : hote des pages ----------------------------------
        _pnlHote.Dock = DockStyle.Fill
        _pnlHote.Padding = New Padding(8)
        _pnlHote.BackColor = Color.White

        ' --- Bas : journal applicatif en direct ------------------------------
        Dim pnlJournal As New Panel() With {.Dock = DockStyle.Bottom, .Height = 170}
        Dim barreJournal As New Panel() With {.Dock = DockStyle.Top, .Height = 28, .BackColor = OutilsControles.CouleurAccent}
        Dim lblJournal As New Label() With {
            .Text = "  Journal applicatif (cycle de vie des connexions, requetes, exceptions)",
            .ForeColor = Color.White, .Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleLeft}
        Dim btnVider As New Button() With {.Text = "Vider", .Dock = DockStyle.Right, .Width = 80, .FlatStyle = FlatStyle.Flat, .ForeColor = Color.White}
        AddHandler btnVider.Click, Sub()
                                       _txtJournal.Clear()
                                       Journal.Vider()
                                   End Sub
        barreJournal.Controls.Add(lblJournal)
        barreJournal.Controls.Add(btnVider)

        _txtJournal.Multiline = True
        _txtJournal.ReadOnly = True
        _txtJournal.ScrollBars = ScrollBars.Vertical
        _txtJournal.Dock = DockStyle.Fill
        _txtJournal.BackColor = Color.FromArgb(30, 30, 30)
        _txtJournal.ForeColor = Color.Gainsboro
        _txtJournal.Font = New Font("Consolas", 8.5F)
        pnlJournal.Controls.Add(_txtJournal)
        pnlJournal.Controls.Add(barreJournal)

        ' --- Gauche : menu de navigation -------------------------------------
        Dim pnlGauche As New Panel() With {.Dock = DockStyle.Left, .Width = 270, .BackColor = Color.FromArgb(245, 246, 250)}
        Dim lblTitre As New Label() With {
            .Text = "Catalogue des techniques",
            .Dock = DockStyle.Top, .Height = 44, .TextAlign = ContentAlignment.MiddleCenter,
            .Font = New Font("Segoe UI Semibold", 11.0F), .ForeColor = OutilsControles.CouleurAccent}
        _lstNav.Dock = DockStyle.Fill
        _lstNav.BorderStyle = BorderStyle.None
        _lstNav.Font = New Font("Segoe UI", 10.0F)
        _lstNav.IntegralHeight = False
        _lstNav.ItemHeight = 26
        AddHandler _lstNav.SelectedIndexChanged, AddressOf SurChangementPage
        pnlGauche.Controls.Add(_lstNav)
        pnlGauche.Controls.Add(lblTitre)

        ' Ordre d'ajout pensé pour le docking (le dernier ajoute est ancre en premier).
        Me.Controls.Add(_pnlHote)
        Me.Controls.Add(pnlJournal)
        Me.Controls.Add(pnlGauche)

        AddHandler Me.Load, AddressOf SurChargement
        AddHandler Me.FormClosed, AddressOf SurFermeture
    End Sub

    ''' <summary>Declare les pages disponibles et leur fabrique (instanciation paresseuse).</summary>
    Private Sub RemplirNavigation()
        _lstNav.Items.Add(New ElementNav("Accueil", Function() New PageAccueil()))
        _lstNav.Items.Add(New ElementNav("Base de donnees", Function() New PageBaseDeDonnees()))
        _lstNav.Items.Add(New ElementNav("Chaines et conversions", Function() New PageChainesConversions()))
        _lstNav.Items.Add(New ElementNav("Geometrie", Function() New PageGeometrie()))
        _lstNav.Items.Add(New ElementNav("Images", Function() New PageImages()))
        _lstNav.Items.Add(New ElementNav("Controles WinForms", Function() New PageControles()))
        _lstNav.Items.Add(New ElementNav("Systeme et OS", Function() New PageSysteme()))
        _lstNav.Items.Add(New ElementNav("Reseau et processus", Function() New PageReseauProcessus()))
        _lstNav.SelectedIndex = 0
    End Sub

    Private Sub SurChargement(ByVal sender As Object, ByVal e As EventArgs)
        AddHandler Journal.LigneAjoutee, AddressOf SurLigneJournal
        Journal.Ecrire("Galerie demarree. Base cible : " & ConfigBdd.NomBase &
                       " (" & ConfigBdd.Serveur & ":" & ConfigBdd.Port.ToString() & ").",
                       Journal.Niveau.Information)
    End Sub

    Private Sub SurFermeture(ByVal sender As Object, ByVal e As FormClosedEventArgs)
        RemoveHandler Journal.LigneAjoutee, AddressOf SurLigneJournal
    End Sub

    ''' <summary>Affiche la page selectionnee dans la zone centrale.</summary>
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

    ''' <summary>
    ''' Recoit une ligne de journal (potentiellement depuis un autre thread) et
    ''' l'ajoute a la zone de texte en marshalant vers le thread d'interface.
    ''' </summary>
    Private Sub SurLigneJournal(ByVal ligne As String, ByVal niveau As Journal.Niveau)
        If Me.IsDisposed Then Return
        If Me.InvokeRequired Then
            Try
                Me.BeginInvoke(New Action(Of String, Journal.Niveau)(AddressOf SurLigneJournal), ligne, niveau)
            Catch
            End Try
            Return
        End If
        _txtJournal.AppendText(ligne & Environment.NewLine)
    End Sub

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

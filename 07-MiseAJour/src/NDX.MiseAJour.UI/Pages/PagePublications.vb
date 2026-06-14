' ============================================================================
'  PagePublications.vb  -  Manifeste en base + recherche d'une mise a jour.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Configuration
Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.MiseAJour

''' <summary>Liste le manifeste et indique si une mise à jour est disponible.</summary>
Public NotInheritable Class PagePublications
    Inherits PageBase

    Private ReadOnly _txtVersion As New TextBox() With {.Width = 120}
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _sortie As TextBox

    Public Sub New()
        MyBase.New("Publications (base)", "Charge le manifeste des versions publiées et détecte une mise à jour pour la version installée.")
        _sortie = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(New Label() With {.Text = "Version installée :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        _txtVersion.Text = If(ConfigurationManager.AppSettings("App.VersionActuelle"), "1.2.0")
        haut.Controls.Add(_txtVersion)
        haut.Controls.Add(Bouton("Rechercher une mise à jour", AddressOf SurRechercher))
        haut.Controls.Add(Bouton("Rafraîchir la liste", AddressOf SurLister))

        Dim centre As New Panel() With {.Dock = DockStyle.Fill}
        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White
        centre.Controls.Add(_grille)

        Dim bas As New Panel() With {.Dock = DockStyle.Bottom, .Height = 120}
        _sortie.Dock = DockStyle.Fill
        bas.Controls.Add(_sortie)

        Contenu.Controls.Add(centre)
        Contenu.Controls.Add(bas)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotPublication.ListerTable()
        Catch ex As Exception
            _sortie.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurRechercher(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim actuelle As VersionSemantique
            If Not VersionSemantique.TryAnalyser(_txtVersion.Text, actuelle) Then
                _sortie.Text = "Version installée invalide."
                Return
            End If

            Dim publications As List(Of Publication) = DepotPublication.Lister()
            Dim derniere As Publication = ServiceMiseAJour.RechercherDerniere(actuelle, publications)
            Dim obligatoire As Boolean = ServiceMiseAJour.MiseAJourObligatoireEnAttente(actuelle, publications)

            If derniere Is Nothing Then
                _sortie.ForeColor = Color.Green
                _sortie.Text = "Vous êtes à jour (version " & actuelle.ToString() & ")." & vbCrLf &
                               publications.Count.ToString() & " version(s) au manifeste."
            Else
                _sortie.ForeColor = If(obligatoire, Color.Firebrick, Color.FromArgb(33, 87, 158))
                _sortie.Text =
                    "Mise à jour disponible : " & actuelle.ToString() & "  ->  " & derniere.Version.ToString() &
                    If(obligatoire, "   (OBLIGATOIRE)", "") & vbCrLf &
                    "Publiée le : " & derniere.PublieeLe.ToString("yyyy-MM-dd") & vbCrLf &
                    "Empreinte  : " & If(derniere.EmpreinteSha256, "(non fournie)") & vbCrLf &
                    "Notes      : " & If(derniere.Notes, "(aucune)")
            End If
        Catch ex As Exception
            _sortie.ForeColor = Color.Firebrick
            _sortie.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

' ============================================================================
'  PageAnnuaire.vb  -  Liste les comptes de l'annuaire (recherche LDAP).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Annuaire

''' <summary>Affiche les comptes trouvés dans l'annuaire (bind administrateur).</summary>
Public NotInheritable Class PageAnnuaire
    Inherits PageBase

    Private ReadOnly _txtFiltre As New TextBox() With {.Width = 200}
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Annuaire (comptes)", "Recherche des comptes (uid/cn). Le terme saisi est échappé (RFC 4515) avant d'entrer dans le filtre LDAP.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(New Label() With {.Text = "Filtre (uid/cn) :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_txtFiltre)
        haut.Controls.Add(Bouton("Rechercher", AddressOf SurCharger))
        haut.Controls.Add(_lblEtat)

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White

        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurCharger(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim comptes As List(Of Utilisateur) = ServiceAnnuaire.Rechercher(_txtFiltre.Text)
            _grille.DataSource = comptes.ConvertAll(Function(u) New With {
                Key .Identifiant = u.Identifiant,
                Key .Nom = u.NomComplet,
                Key .Courriel = u.Courriel,
                Key .DN = u.NomDistinctif})
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = comptes.Count.ToString() & " compte(s) trouvé(s)."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

' ============================================================================
'  PageExceptions.vb  -  Declenche une exception (gérée globalement) + journal.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Demarrage

''' <summary>Déclenche une exception sur le fil d'interface : le gestionnaire global la capte.</summary>
Public NotInheritable Class PageExceptions
    Inherits PageBase

    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Exceptions & journal", "Le bouton lève une exception non gérée : au lieu de planter, l'application l'intercepte, l'affiche et la journalise.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(Bouton("Déclencher une exception", AddressOf SurDeclencher))
        haut.Controls.Add(Bouton("Rafraîchir le journal", AddressOf SurLister))
        haut.Controls.Add(_lblEtat)

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White

        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurDeclencher(ByVal sender As Object, ByVal e As EventArgs)
        ' Volontairement non protégé : c'est le gestionnaire GLOBAL (installé dans
        ' Program.Main) qui doit l'intercepter, la journaliser et l'afficher.
        Throw New InvalidOperationException("Exception de démonstration déclenchée à " & DateTime.Now.ToString("HH:mm:ss") & ".")
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotEvenement.ListerTable()
            _lblEtat.Text = ""
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

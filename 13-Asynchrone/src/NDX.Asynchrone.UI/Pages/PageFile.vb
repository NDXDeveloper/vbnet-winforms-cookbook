' ============================================================================
'  PageFile.vb  -  File de taches : empiler puis drainer en asynchrone.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms
Imports NDX.Asynchrone

''' <summary>Empile des tâches en base, puis vide la file en arrière-plan avec progression.</summary>
Public NotInheritable Class PageFile
    Inherits PageBase

    Private ReadOnly _barre As New ProgressBar() With {.Minimum = 0, .Maximum = 100, .Width = 360, .Height = 20}
    Private ReadOnly _lbl As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 4, 0, 0)}
    Private ReadOnly _grille As New DataGridView()

    Public Sub New()
        MyBase.New("File de tâches (base)", "Empilez des tâches (état « en_attente »), puis drainez-les en arrière-plan : chacune passe à « traitee ».")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(Bouton("Empiler 10 tâches", AddressOf SurEmpiler))
        haut.Controls.Add(Bouton("Drainer la file (async)", AddressOf SurDrainer))
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))

        Dim zone As New Panel() With {.Dock = DockStyle.Top, .Height = 34}
        _barre.Location = New Point(4, 4)
        zone.Controls.Add(_barre)
        zone.Controls.Add(_lbl)

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White

        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(zone)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurEmpiler(ByVal sender As Object, ByVal e As EventArgs)
        Try
            For i As Integer = 1 To 10
                DepotTache.Empiler("Tâche lot " & DateTime.Now.ToString("HHmmss") & "-" & i.ToString())
            Next
            _lbl.ForeColor = Color.DimGray
            _lbl.Text = "10 tâches empilées."
            SurLister(sender, e)
        Catch ex As Exception
            _lbl.ForeColor = Color.Firebrick
            _lbl.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Async Sub SurDrainer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim enAttente As List(Of Tache) = DepotTache.ListerEnAttente()
            If enAttente.Count = 0 Then
                _lbl.ForeColor = Color.DimGray
                _lbl.Text = "Aucune tâche en attente."
                Return
            End If
            _barre.Value = 0
            Dim progression As New Progress(Of Avancement)(
                Sub(a)
                    _barre.Value = a.Pourcentage
                    _lbl.Text = a.Message
                End Sub)
            Dim faits As Integer = Await MoteurTaches.ExecuterAsync(
                enAttente,
                Sub(t) DepotTache.MarquerTraitee(t.Id),
                progression, CancellationToken.None, delaiMsParElement:=120)
            _lbl.ForeColor = Color.Green
            _lbl.Text = faits.ToString() & " tâche(s) traitée(s)."
            SurLister(sender, e)
        Catch ex As Exception
            _lbl.ForeColor = Color.Firebrick
            _lbl.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotTache.ListerTable()
        Catch ex As Exception
            _lbl.ForeColor = Color.Firebrick
            _lbl.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

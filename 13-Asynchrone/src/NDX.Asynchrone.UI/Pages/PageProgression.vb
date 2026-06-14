' ============================================================================
'  PageProgression.vb  -  Tache longue : progression + annulation, UI reactive.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Linq
Imports System.Threading
Imports System.Windows.Forms
Imports NDX.Asynchrone

''' <summary>Lance un traitement long en arrière-plan ; l'interface reste réactive (annulation possible).</summary>
Public NotInheritable Class PageProgression
    Inherits PageBase

    Private ReadOnly _barre As New ProgressBar() With {.Minimum = 0, .Maximum = 100, .Width = 520, .Height = 24}
    Private ReadOnly _lbl As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .ForeColor = Color.DimGray}
    Private ReadOnly _btnDemarrer As Button
    Private ReadOnly _btnAnnuler As Button
    Private _cts As CancellationTokenSource

    Public Sub New()
        MyBase.New("Progression / annulation", "Un traitement de 50 éléments s'exécute en arrière-plan : la fenêtre reste utilisable, et le bouton Annuler interrompt aussitôt.")
        _btnDemarrer = Bouton("Démarrer", AddressOf SurDemarrer)
        _btnAnnuler = Bouton("Annuler", AddressOf SurAnnuler)
        _btnAnnuler.Enabled = False
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        barre.Controls.Add(_btnDemarrer)
        barre.Controls.Add(_btnAnnuler)

        Dim zone As New Panel() With {.Dock = DockStyle.Top, .Height = 80, .Padding = New Padding(4, 10, 0, 0)}
        _barre.Location = New Point(4, 8)
        _lbl.Location = New Point(4, 44)
        zone.Controls.Add(_barre)
        zone.Controls.Add(_lbl)

        Contenu.Controls.Add(zone)
        Contenu.Controls.Add(barre)
    End Sub

    Private Async Sub SurDemarrer(ByVal sender As Object, ByVal e As EventArgs)
        _btnDemarrer.Enabled = False
        _btnAnnuler.Enabled = True
        _barre.Value = 0
        _cts = New CancellationTokenSource()

        ' Progress(Of T) capture le contexte de l'UI : Report met à jour les contrôles sur le bon fil.
        Dim progression As New Progress(Of Avancement)(
            Sub(a)
                _barre.Value = a.Pourcentage
                _lbl.Text = a.Message
            End Sub)

        Try
            Dim faits As Integer = Await MoteurTaches.ExecuterAsync(
                Enumerable.Range(1, 50),
                Sub(i)
                    ' Travail simulé (le délai par élément est géré par le moteur).
                End Sub,
                progression, _cts.Token, delaiMsParElement:=60)
            _lbl.ForeColor = Color.Green
            _lbl.Text = "Terminé : " & faits.ToString() & " éléments traités."
        Catch ex As OperationCanceledException
            _lbl.ForeColor = Color.Firebrick
            _lbl.Text = "Traitement annulé."
        Finally
            _btnDemarrer.Enabled = True
            _btnAnnuler.Enabled = False
            _cts.Dispose()
            _cts = Nothing
        End Try
    End Sub

    Private Sub SurAnnuler(ByVal sender As Object, ByVal e As EventArgs)
        If _cts IsNot Nothing Then _cts.Cancel()
    End Sub

End Class

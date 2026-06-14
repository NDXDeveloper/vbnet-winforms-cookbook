' ============================================================================
'  PageInstance.vb  -  Ecoute IPC + simulation d'une seconde instance.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports NDX.InstanceUnique

''' <summary>Démarre l'écoute (instance primaire) puis simule une 2ᵉ instance qui envoie ses arguments.</summary>
Public NotInheritable Class PageInstance
    Inherits PageBase

    Private Const NOM_CANAL As String = "ndx-instance-unique-demo"
    Private Const NOM_MUTEX As String = "ndx-instance-unique-demo-mutex"

    Private ReadOnly _console As TextBox
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .ForeColor = Color.DimGray}
    Private _verrou As VerrouInstance
    Private _cts As CancellationTokenSource
    Private _tacheEcoute As Task

    Public Sub New()
        MyBase.New("Instance & messages", "Démarrez l'écoute (instance primaire), puis simulez une 2ᵉ instance : ses arguments arrivent ici via le tube nommé.")
        _console = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(Bouton("Démarrer l'écoute", AddressOf SurDemarrer))
        haut.Controls.Add(Bouton("Simuler une 2ᵉ instance", AddressOf SurSimuler))
        haut.Controls.Add(Bouton("Arrêter l'écoute", AddressOf SurArreter))
        haut.Controls.Add(_lblEtat)

        Contenu.Controls.Add(_console)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurDemarrer(ByVal sender As Object, ByVal e As EventArgs)
        If _cts IsNot Nothing Then
            Journaliser("L'écoute est déjà active.")
            Return
        End If
        _verrou = New VerrouInstance(NOM_MUTEX)
        _cts = New CancellationTokenSource()
        ' Appelée depuis le fil UI : la reprise après Await revient sur le fil UI,
        ' donc SurReception peut toucher les contrôles directement.
        _tacheEcoute = CanalCommande.EcouterAsync(NOM_CANAL, _cts.Token, AddressOf SurReception)
        _lblEtat.ForeColor = If(_verrou.EstPremiere, Color.Green, Color.DarkOrange)
        _lblEtat.Text = If(_verrou.EstPremiere, "Instance PRIMAIRE — écoute démarrée.", "Instance secondaire (le mutex existait déjà) — écoute démarrée.")
        Journaliser("Écoute du canal « " & NOM_CANAL & " »…")
    End Sub

    Private Sub SurSimuler(ByVal sender As Object, ByVal e As EventArgs)
        Dim args As New List(Of String) From {"--ouvrir", "rapport mensuel.txt", "envoyé à " & DateTime.Now.ToString("HH:mm:ss")}
        Dim ok As Boolean = CanalCommande.Envoyer(NOM_CANAL, Commande.Encoder(args))
        If Not ok Then
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Personne n'écoute : démarrez d'abord l'écoute."
        End If
    End Sub

    Private Sub SurReception(ByVal ligne As String)
        Dim args As List(Of String) = Commande.Decoder(ligne)
        Journaliser("Commande reçue : " & String.Join("  ", args))
        Try
            DepotCommande.Enregistrer("instance-2", String.Join(" ", args))
        Catch ex As Exception
            Journaliser("  (journalisation en base impossible : " & ex.Message & ")")
        End Try
    End Sub

    Private Sub SurArreter(ByVal sender As Object, ByVal e As EventArgs)
        If _cts IsNot Nothing Then
            _cts.Cancel()
            _cts.Dispose()
            _cts = Nothing
        End If
        If _verrou IsNot Nothing Then
            _verrou.Dispose()
            _verrou = Nothing
        End If
        _lblEtat.ForeColor = Color.DimGray
        _lblEtat.Text = "Écoute arrêtée."
    End Sub

    Private Sub Journaliser(ByVal ligne As String)
        _console.AppendText(ligne & vbCrLf)
    End Sub

End Class

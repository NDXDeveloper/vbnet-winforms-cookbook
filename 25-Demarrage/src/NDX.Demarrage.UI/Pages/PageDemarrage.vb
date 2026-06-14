' ============================================================================
'  PageDemarrage.vb  -  Execution d'une sequence d'etapes de demarrage.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Demarrage

''' <summary>Construit une séquence d'étapes (dont une échoue) et affiche les résultats.</summary>
Public NotInheritable Class PageDemarrage
    Inherits PageBase

    Private ReadOnly _chkArreter As New CheckBox() With {.Text = "Arrêter au 1er échec", .Checked = True, .AutoSize = True, .Margin = New Padding(8, 9, 8, 0)}
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Séquence de démarrage", "Exécute des étapes dans l'ordre ; l'étape « Module optionnel » échoue volontairement pour illustrer l'arrêt / la poursuite.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(Bouton("Lancer la séquence", AddressOf SurLancer))
        haut.Controls.Add(_chkArreter)
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

    Private Function Etapes() As List(Of EtapeDemarrage)
        Return New List(Of EtapeDemarrage) From {
            New EtapeDemarrage("Vérifier la configuration", Function() True),
            New EtapeDemarrage("Connexion à la base", Function()
                                                          Dim m As String = ""
                                                          Return DepotEvenement.TesterConnexion(m)
                                                      End Function),
            New EtapeDemarrage("Charger les préférences", Function() True),
            New EtapeDemarrage("Vérifier le module optionnel", Function() False),
            New EtapeDemarrage("Préparer le cache", Function() True)}
    End Function

    Private Sub SurLancer(ByVal sender As Object, ByVal e As EventArgs)
        Dim resultats As List(Of ResultatEtape) = SequenceDemarrage.Executer(Etapes(), _chkArreter.Checked)
        _grille.DataSource = resultats

        ' Journalisation (best-effort).
        Try
            For Each r As ResultatEtape In resultats
                DepotEvenement.Enregistrer("etape", r.ToString())
            Next
        Catch
        End Try

        If SequenceDemarrage.ToutEstReussi(resultats) Then
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Toutes les étapes ont réussi."
        Else
            _lblEtat.ForeColor = Color.DarkOrange
            _lblEtat.Text = resultats.Count.ToString() & " étape(s) exécutée(s) ; au moins un échec."
        End If
    End Sub

End Class

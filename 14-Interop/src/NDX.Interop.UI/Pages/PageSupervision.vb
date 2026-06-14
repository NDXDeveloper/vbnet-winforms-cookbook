' ============================================================================
'  PageSupervision.vb  -  Releves en temps reel (GDI/USER + inactivite) + topmost.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Interop

''' <summary>Affiche en direct les compteurs Win32 et permet d'épingler la fenêtre / d'enregistrer un relevé.</summary>
Public NotInheritable Class PageSupervision
    Inherits PageBase

    Private ReadOnly _minuteur As New Timer() With {.Interval = 1000}
    Private ReadOnly _lblGdi As New Label() With {.AutoSize = True, .Font = New Font("Consolas", 12.0F)}
    Private ReadOnly _lblUser As New Label() With {.AutoSize = True, .Font = New Font("Consolas", 12.0F)}
    Private ReadOnly _lblInactif As New Label() With {.AutoSize = True, .Font = New Font("Consolas", 12.0F)}
    Private ReadOnly _chkTopmost As New CheckBox() With {.Text = "Toujours au premier plan", .AutoSize = True}
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .ForeColor = Color.DimGray}

    Public Sub New()
        MyBase.New("Supervision (temps réel)", "Relevés rafraîchis chaque seconde. Bougez/laissez la souris pour voir l'inactivité varier ; ouvrez des fenêtres pour voir les objets GDI/USER.")
        Construire()
        AddHandler _minuteur.Tick, AddressOf SurTick
        _minuteur.Start()
        Rafraichir()
    End Sub

    Private Sub Construire()
        Dim zone As New Panel() With {.Dock = DockStyle.Top, .Height = 130, .Padding = New Padding(8)}
        _lblGdi.Location = New Point(8, 8)
        _lblUser.Location = New Point(8, 38)
        _lblInactif.Location = New Point(8, 68)
        zone.Controls.AddRange(New Control() {_lblGdi, _lblUser, _lblInactif})

        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        _chkTopmost.Margin = New Padding(6, 10, 12, 0)
        AddHandler _chkTopmost.CheckedChanged, AddressOf SurTopmost
        barre.Controls.Add(_chkTopmost)
        barre.Controls.Add(Bouton("Enregistrer un relevé", AddressOf SurEnregistrer))
        barre.Controls.Add(_lblEtat)

        Contenu.Controls.Add(barre)
        Contenu.Controls.Add(zone)
    End Sub

    Private Sub SurTick(ByVal sender As Object, ByVal e As EventArgs)
        Rafraichir()
    End Sub

    Private Sub Rafraichir()
        _lblGdi.Text = "Objets GDI  : " & RessourcesProcessus.ObjetsGdi().ToString()
        _lblUser.Text = "Objets USER : " & RessourcesProcessus.ObjetsUser().ToString()
        _lblInactif.Text = "Inactivité  : " & Inactivite.Duree().TotalSeconds.ToString("0.0") & " s"
    End Sub

    Private Sub SurTopmost(ByVal sender As Object, ByVal e As EventArgs)
        Dim f As Form = Me.FindForm()
        If f Is Nothing Then Return
        If _chkTopmost.Checked Then
            FenetrePremierPlan.Epingler(f.Handle)
        Else
            FenetrePremierPlan.Desepingler(f.Handle)
        End If
    End Sub

    Private Sub SurEnregistrer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim id As Integer = DepotSupervision.Enregistrer(
                RessourcesProcessus.ObjetsGdi(), RessourcesProcessus.ObjetsUser(),
                CLng(Inactivite.Duree().TotalMilliseconds))
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Relevé enregistré (id = " & id.ToString() & ")."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

' ============================================================================
'  PageCanevas.vb  -  Dessin interactif : creation, deplacement, sauvegarde.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Dessin

''' <summary>Trace des formes au glisser, les déplace (hit-testing) et enregistre la scène.</summary>
Public NotInheritable Class PageCanevas
    Inherits PageBase

    Private ReadOnly _canevas As New Canevas()
    Private ReadOnly _cboType As New ComboBox() With {.Width = 110, .DropDownStyle = ComboBoxStyle.DropDownList}
    Private ReadOnly _chkDeplacer As New CheckBox() With {.Text = "Déplacer", .AutoSize = True, .Margin = New Padding(6, 9, 6, 0)}
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Canevas", "Choisissez une forme et glissez pour la tracer. Cochez « Déplacer » puis saisissez une forme pour la bouger.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        _cboType.Items.AddRange(New Object() {"Rectangle", "Ellipse", "Ligne"})
        _cboType.SelectedIndex = 0
        AddHandler _cboType.SelectedIndexChanged, Sub(s, e) _canevas.TypeCourant = CType(_cboType.SelectedIndex, TypeForme)
        AddHandler _chkDeplacer.CheckedChanged, Sub(s, e) _canevas.ModeDeplacement = _chkDeplacer.Checked
        haut.Controls.Add(New Label() With {.Text = "Forme :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_cboType)
        haut.Controls.Add(Bouton("Couleur…", AddressOf SurCouleur))
        haut.Controls.Add(_chkDeplacer)
        haut.Controls.Add(Bouton("Effacer", AddressOf SurEffacer))
        haut.Controls.Add(Bouton("Enregistrer", AddressOf SurEnregistrer))
        haut.Controls.Add(Bouton("Charger", AddressOf SurCharger))
        haut.Controls.Add(_lblEtat)

        _canevas.Dock = DockStyle.Fill

        Contenu.Controls.Add(_canevas)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurCouleur(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New ColorDialog() With {.Color = _canevas.CouleurCourante}
            If dlg.ShowDialog() = DialogResult.OK Then _canevas.CouleurCourante = dlg.Color
        End Using
    End Sub

    Private Sub SurEffacer(ByVal sender As Object, ByVal e As EventArgs)
        _canevas.Scene.Vider()
        _canevas.Invalidate()
    End Sub

    Private Sub SurEnregistrer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            DepotForme.EnregistrerScene(_canevas.Scene)
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = _canevas.Scene.Formes.Count.ToString() & " forme(s) enregistrée(s)."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurCharger(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _canevas.Reinitialiser(DepotForme.ChargerScene())
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = _canevas.Scene.Formes.Count.ToString() & " forme(s) chargée(s)."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

End Class

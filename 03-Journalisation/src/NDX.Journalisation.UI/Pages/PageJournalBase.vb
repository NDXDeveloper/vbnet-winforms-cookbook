' ============================================================================
'  PageJournalBase.vb  -  Relecture des entrees enregistrees en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Journalisation

Public NotInheritable Class PageJournalBase
    Inherits PageBase

    Private ReadOnly _cboNiveauMin As New ComboBox()
    Private ReadOnly _dgv As New DataGridView()
    Private ReadOnly _lblEtat As New Label()

    Public Sub New()
        MyBase.New("Journal en base", "Relire les entrées enregistrées dans MariaDB par le puits base, filtrées par niveau minimal.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True}
        _cboNiveauMin.DropDownStyle = ComboBoxStyle.DropDownList
        _cboNiveauMin.Width = 130
        _cboNiveauMin.DataSource = [Enum].GetValues(GetType(Niveau))
        barre.Controls.Add(New Label() With {.Text = "Niveau minimal :", .AutoSize = True, .Margin = New Padding(4, 9, 0, 0)})
        barre.Controls.Add(_cboNiveauMin)
        barre.Controls.Add(Bouton("Rafraîchir", AddressOf SurRafraichir))

        _lblEtat.Dock = DockStyle.Bottom
        _lblEtat.Height = 24
        _lblEtat.TextAlign = ContentAlignment.MiddleLeft

        _dgv.Dock = DockStyle.Fill
        _dgv.ReadOnly = True
        _dgv.AllowUserToAddRows = False
        _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _dgv.BackgroundColor = Color.White

        Contenu.Controls.Add(_dgv)
        Contenu.Controls.Add(_lblEtat)
        Contenu.Controls.Add(barre)
    End Sub

    Private Sub SurRafraichir(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _dgv.DataSource = PuitsBase.Lire(CType(_cboNiveauMin.SelectedItem, Niveau), 200)
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Liste rafraîchie."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Base indisponible : " & ex.Message
        End Try
    End Sub

End Class

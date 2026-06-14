' ============================================================================
'  PageExport.vb  -  Charger une source, prévisualiser, exporter (CSV/Excel/PDF).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Export

Public NotInheritable Class PageExport
    Inherits PageBase

    Private ReadOnly _cboSource As New ComboBox()
    Private ReadOnly _cboFormat As New ComboBox()
    Private ReadOnly _dgv As New DataGridView()
    Private ReadOnly _lblEtat As New Label()
    Private _table As DataTable

    Public Sub New()
        MyBase.New("Exporter des données", "Choisissez une source et un format, prévisualisez, puis exportez vers un fichier.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True, .WrapContents = True}

        _cboSource.DropDownStyle = ComboBoxStyle.DropDownList
        _cboSource.Width = 200
        _cboSource.Items.AddRange(New Object() {"Toutes les ventes", "Ventes par catégorie"})
        _cboSource.SelectedIndex = 0

        _cboFormat.DropDownStyle = ComboBoxStyle.DropDownList
        _cboFormat.Width = 90
        _cboFormat.DataSource = [Enum].GetValues(GetType(FormatExport))

        barre.Controls.Add(New Label() With {.Text = "Source :", .AutoSize = True, .Margin = New Padding(4, 9, 0, 0)})
        barre.Controls.Add(_cboSource)
        barre.Controls.Add(Bouton("Charger", AddressOf SurCharger))
        barre.Controls.Add(New Label() With {.Text = "Format :", .AutoSize = True, .Margin = New Padding(10, 9, 0, 0)})
        barre.Controls.Add(_cboFormat)
        barre.Controls.Add(Bouton("Exporter vers un fichier...", AddressOf SurExporter))

        _lblEtat.Dock = DockStyle.Bottom
        _lblEtat.Height = 24
        _lblEtat.TextAlign = ContentAlignment.MiddleLeft

        _dgv.Dock = DockStyle.Fill
        _dgv.ReadOnly = True
        _dgv.AllowUserToAddRows = False
        _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _dgv.BackgroundColor = Color.White

        Contenu.Controls.Add(_dgv)
        Contenu.Controls.Add(_lblEtat)
        Contenu.Controls.Add(barre)
    End Sub

    Private Function FormatChoisi() As FormatExport
        Return CType(_cboFormat.SelectedItem, FormatExport)
    End Function

    Private Sub SurCharger(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _table = If(_cboSource.SelectedIndex = 1, SourceDonnees.VentesParCategorie(), SourceDonnees.ToutesLesVentes())
            _dgv.DataSource = _table
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = _table.Rows.Count.ToString() & " ligne(s) chargée(s)."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Base indisponible : " & ex.Message
        End Try
    End Sub

    Private Sub SurExporter(ByVal sender As Object, ByVal e As EventArgs)
        If _table Is Nothing Then SurCharger(sender, e)
        If _table Is Nothing Then Return

        Dim format As FormatExport = FormatChoisi()
        Dim exportateur As IExportateur = Exportateurs.Creer(format)
        Using dlg As New SaveFileDialog()
            dlg.Filter = FiltreFichier(format)
            dlg.FileName = "export" & exportateur.Extension
            If dlg.ShowDialog() = DialogResult.OK Then
                Try
                    Exportateurs.SauverFichier(_table, dlg.FileName, format)
                    _lblEtat.ForeColor = Color.Green
                    _lblEtat.Text = "Exporté (" & format.ToString() & ") : " & dlg.FileName
                Catch ex As Exception
                    _lblEtat.ForeColor = Color.Firebrick
                    _lblEtat.Text = "Échec de l'export : " & ex.Message
                End Try
            End If
        End Using
    End Sub

    Private Function FiltreFichier(ByVal format As FormatExport) As String
        Select Case format
            Case FormatExport.Csv : Return "Fichier CSV|*.csv"
            Case FormatExport.Excel : Return "Classeur Excel|*.xlsx"
            Case FormatExport.Pdf : Return "Document PDF|*.pdf"
            Case Else : Return "Tous les fichiers|*.*"
        End Select
    End Function

End Class

' ============================================================================
'  PagePersistance.vb
'  Demonstration : enregistrer un objet serialise en base (BLOB + empreinte),
'  le lister, le recharger/deserialiser et verifier son integrite.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Serialisation

''' <summary>Page de demonstration de la persistance d'objets serialises en base.</summary>
Public NotInheritable Class PagePersistance
    Inherits PageBase

    Private ReadOnly _cboFormat As New ComboBox()
    Private ReadOnly _cboCategorie As New ComboBox()
    Private ReadOnly _txtLibelle As New TextBox()
    Private ReadOnly _dgv As New DataGridView()
    Private ReadOnly _console As TextBox

    Public Sub New()
        MyBase.New("Persistance en base",
                   "Sérialiser un objet puis l'enregistrer en base (BLOB + SHA-256), le relister, le recharger et vérifier son intégrité.")
        _console = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True, .WrapContents = True}

        _cboFormat.DropDownStyle = ComboBoxStyle.DropDownList
        _cboFormat.Width = 120
        _cboFormat.DataSource = [Enum].GetValues(GetType(FormatSerialisation))

        _cboCategorie.DropDownStyle = ComboBoxStyle.DropDownList
        _cboCategorie.Width = 160

        _txtLibelle.Width = 180
        _txtLibelle.Text = "Catalogue de démonstration"

        barre.Controls.Add(New Label() With {.Text = "Format :", .AutoSize = True, .Margin = New Padding(4, 9, 0, 0)})
        barre.Controls.Add(_cboFormat)
        barre.Controls.Add(New Label() With {.Text = "Catégorie :", .AutoSize = True, .Margin = New Padding(6, 9, 0, 0)})
        barre.Controls.Add(_cboCategorie)
        barre.Controls.Add(New Label() With {.Text = "Libellé :", .AutoSize = True, .Margin = New Padding(6, 9, 0, 0)})
        barre.Controls.Add(_txtLibelle)
        barre.Controls.Add(Bouton("Stocker l'exemple", AddressOf SurStocker))
        barre.Controls.Add(Bouton("Recharger + désérialiser", AddressOf SurRecharger))
        barre.Controls.Add(Bouton("Vérifier l'intégrité", AddressOf SurVerifier))
        barre.Controls.Add(Bouton("Rafraîchir", AddressOf SurRafraichir))

        _dgv.Dock = DockStyle.Fill
        _dgv.ReadOnly = True
        _dgv.AllowUserToAddRows = False
        _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _dgv.BackgroundColor = Color.White

        Dim panneauConsole As New Panel() With {.Dock = DockStyle.Bottom, .Height = 110, .Padding = New Padding(0, 4, 0, 0)}
        panneauConsole.Controls.Add(_console)

        Contenu.Controls.Add(_dgv)
        Contenu.Controls.Add(panneauConsole)
        Contenu.Controls.Add(barre)

        Tracer("Astuce : démarrez le conteneur Docker puis cliquez sur « Rafraîchir ».")
    End Sub

    Private Function FormatChoisi() As FormatSerialisation
        Return CType(_cboFormat.SelectedItem, FormatSerialisation)
    End Function

    Private Function CategorieChoisie() As Integer?
        If _cboCategorie.SelectedValue IsNot Nothing AndAlso TypeOf _cboCategorie.SelectedValue Is Integer Then
            Return CInt(_cboCategorie.SelectedValue)
        End If
        Return Nothing
    End Function

    Private Function IdSelectionne() As Integer?
        If _dgv.CurrentRow IsNot Nothing AndAlso _dgv.CurrentRow.Cells("Id").Value IsNot Nothing Then
            Return Convert.ToInt32(_dgv.CurrentRow.Cells("Id").Value)
        End If
        Return Nothing
    End Function

    Private Sub Tracer(ByVal texte As String)
        _console.AppendText(texte & vbCrLf)
    End Sub

    Private Sub SurRafraichir(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _cboCategorie.DataSource = DepotDocuments.ListerCategories()
            _cboCategorie.DisplayMember = "libelle"
            _cboCategorie.ValueMember = "id"
            _dgv.DataSource = DepotDocuments.Lister()
            Tracer("Liste rafraîchie.")
        Catch ex As Exception
            Tracer("Base indisponible : " & ex.Message)
        End Try
    End Sub

    Private Sub SurStocker(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim libelle As String = If(String.IsNullOrWhiteSpace(_txtLibelle.Text), "Document", _txtLibelle.Text.Trim())
            Dim id As Integer = DepotDocuments.Enregistrer(libelle, Catalogue.Exemple(), FormatChoisi(), CategorieChoisie())
            Tracer("Document enregistré (" & FormatChoisi().ToString() & ") — id = " & id.ToString() & ".")
            _dgv.DataSource = DepotDocuments.Lister()
        Catch ex As Exception
            Tracer("Échec de l'enregistrement : " & ex.Message)
        End Try
    End Sub

    Private Sub SurRecharger(ByVal sender As Object, ByVal e As EventArgs)
        Dim id As Integer? = IdSelectionne()
        If Not id.HasValue Then
            Tracer("Sélectionnez d'abord une ligne dans la liste.")
            Return
        End If
        Try
            Dim cat As Catalogue = DepotDocuments.Recharger(Of Catalogue)(id.Value)
            Tracer("Document " & id.Value.ToString() & " rechargé et désérialisé :")
            Tracer("  " & cat.Nom & " — " & cat.Produits.Count.ToString() & " produits ; 1er = " & cat.Produits(0).ToString())
        Catch ex As Exception
            Tracer("Échec du rechargement (type/format incompatible ?) : " & ex.Message)
        End Try
    End Sub

    Private Sub SurVerifier(ByVal sender As Object, ByVal e As EventArgs)
        Dim id As Integer? = IdSelectionne()
        If Not id.HasValue Then
            Tracer("Sélectionnez d'abord une ligne dans la liste.")
            Return
        End If
        Try
            Dim ok As Boolean = DepotDocuments.VerifierIntegrite(id.Value)
            Tracer("Intégrité du document " & id.Value.ToString() & " : " &
                   If(ok, "OK — l'empreinte SHA-256 correspond", "ALTÉRÉ — empreinte différente"))
        Catch ex As Exception
            Tracer("Échec de la vérification : " & ex.Message)
        End Try
    End Sub

End Class

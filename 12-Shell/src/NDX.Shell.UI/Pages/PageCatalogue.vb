' ============================================================================
'  PageCatalogue.vb  -  Catalogue de liens (base) + export en fichiers reels.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports NDX.Shell

''' <summary>Liste / ajoute / supprime des liens en base, et les exporte sur le Bureau.</summary>
Public NotInheritable Class PageCatalogue
    Inherits PageBase

    Private ReadOnly _cboCategorie As New ComboBox() With {.Width = 130, .DropDownStyle = ComboBoxStyle.DropDownList}
    Private ReadOnly _txtNom As New TextBox() With {.Width = 160}
    Private ReadOnly _txtCible As New TextBox() With {.Width = 280}
    Private ReadOnly _grille As New DataGridView()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Catalogue (base)", "Catalogue de liens en base. La sélection peut être exportée en raccourci réel (.lnk / .url) sur le Bureau.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 80, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        _cboCategorie.Items.AddRange(New Object() {"application", "web"})
        _cboCategorie.SelectedIndex = 0
        haut.Controls.Add(New Label() With {.Text = "Catégorie :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_cboCategorie)
        haut.Controls.Add(New Label() With {.Text = "Nom :", .AutoSize = True, .Margin = New Padding(8, 9, 2, 0)})
        haut.Controls.Add(_txtNom)
        haut.Controls.Add(New Label() With {.Text = "Cible :", .AutoSize = True, .Margin = New Padding(8, 9, 2, 0)})
        haut.Controls.Add(_txtCible)
        haut.Controls.Add(Bouton("Ajouter", AddressOf SurAjouter))
        haut.Controls.Add(Bouton("Rafraîchir", AddressOf SurLister))
        haut.Controls.Add(Bouton("Supprimer la sélection", AddressOf SurSupprimer))
        haut.Controls.Add(Bouton("Exporter sur le Bureau", AddressOf SurExporter))
        haut.Controls.Add(_lblEtat)

        _grille.Dock = DockStyle.Fill
        _grille.ReadOnly = True
        _grille.AllowUserToAddRows = False
        _grille.AllowUserToDeleteRows = False
        _grille.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        _grille.MultiSelect = False
        _grille.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        _grille.BackgroundColor = Color.White

        Contenu.Controls.Add(_grille)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Try
            _grille.DataSource = DepotLien.ListerTable()
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = ""
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurAjouter(ByVal sender As Object, ByVal e As EventArgs)
        If String.IsNullOrWhiteSpace(_txtNom.Text) OrElse String.IsNullOrWhiteSpace(_txtCible.Text) Then
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Nom et cible obligatoires."
            Return
        End If
        Try
            DepotLien.Ajouter(New Lien() With {
                .Categorie = Convert.ToString(_cboCategorie.SelectedItem),
                .Nom = _txtNom.Text.Trim(),
                .Cible = _txtCible.Text.Trim(),
                .Description = ""})
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Lien ajouté."
            SurLister(sender, e)
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurSupprimer(ByVal sender As Object, ByVal e As EventArgs)
        If _grille.CurrentRow Is Nothing Then Return
        Try
            DepotLien.Supprimer(Convert.ToInt32(_grille.CurrentRow.Cells("Id").Value))
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = "Lien supprimé."
            SurLister(sender, e)
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    ''' <summary>Exporte la ligne sélectionnée en raccourci réel sur le Bureau.</summary>
    Private Sub SurExporter(ByVal sender As Object, ByVal e As EventArgs)
        If _grille.CurrentRow Is Nothing Then
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Sélectionnez d'abord une ligne."
            Return
        End If
        Try
            Dim categorie As String = Convert.ToString(_grille.CurrentRow.Cells("Catégorie").Value)
            Dim nom As String = Convert.ToString(_grille.CurrentRow.Cells("Nom").Value)
            Dim cible As String = Convert.ToString(_grille.CurrentRow.Cells("Cible").Value)
            Dim bureau As String = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            Dim chemin As String

            If String.Equals(categorie, "web", StringComparison.OrdinalIgnoreCase) Then
                chemin = Path.Combine(bureau, NomFichierSur(nom) & ".url")
                RaccourciInternet.Ecrire(chemin, cible)
            Else
                If Not RaccourciWindows.EstDisponible() Then
                    _lblEtat.ForeColor = Color.Firebrick
                    _lblEtat.Text = "WScript.Shell indisponible."
                    Return
                End If
                chemin = Path.Combine(bureau, NomFichierSur(nom) & ".lnk")
                RaccourciWindows.Creer(chemin, cible, description:="Exporté par NDX.Shell")
            End If
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Exporté : " & chemin
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Shared Function NomFichierSur(ByVal nom As String) As String
        For Each c As Char In Path.GetInvalidFileNameChars()
            nom = nom.Replace(c, "_"c)
        Next
        Return nom
    End Function

End Class

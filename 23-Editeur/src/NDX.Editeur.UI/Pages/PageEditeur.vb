' ============================================================================
'  PageEditeur.vb  -  Editeur RTF : mise en forme + statistiques + base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Editeur

''' <summary>RichTextBox + barre de mise en forme ; statistiques en direct ; enregistrement RTF en base.</summary>
Public NotInheritable Class PageEditeur
    Inherits PageBase

    Private ReadOnly _rtb As New RichTextBox()
    Private ReadOnly _txtTitre As New TextBox() With {.Width = 180, .Text = "Sans titre"}
    Private ReadOnly _cboDocs As New ComboBox() With {.Width = 200, .DropDownStyle = ComboBoxStyle.DropDownList}
    Private ReadOnly _lblStats As New Label() With {.Dock = DockStyle.Bottom, .Height = 24, .TextAlign = ContentAlignment.MiddleLeft, .ForeColor = Color.DimGray, .Padding = New Padding(6, 0, 0, 0)}

    Public Sub New()
        MyBase.New("Éditeur RTF", "Sélectionnez du texte puis appliquez gras/italique/couleur. Les statistiques se mettent à jour en direct.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        barre.Controls.Add(New Label() With {.Text = "Titre :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        barre.Controls.Add(_txtTitre)
        barre.Controls.Add(Bouton("G", AddressOf SurGras))
        barre.Controls.Add(Bouton("I", AddressOf SurItalique))
        barre.Controls.Add(Bouton("S", AddressOf SurSouligne))
        barre.Controls.Add(Bouton("Couleur…", AddressOf SurCouleur))
        barre.Controls.Add(Bouton("Nouveau", AddressOf SurNouveau))
        barre.Controls.Add(Bouton("Enregistrer", AddressOf SurEnregistrer))
        barre.Controls.Add(_cboDocs)
        barre.Controls.Add(Bouton("Charger", AddressOf SurCharger))

        _rtb.Dock = DockStyle.Fill
        _rtb.Font = New Font("Segoe UI", 11.0F)
        _rtb.Text = "Tapez ici, sélectionnez du texte, puis mettez-le en forme."
        AddHandler _rtb.TextChanged, AddressOf SurTexteModifie

        Contenu.Controls.Add(_rtb)
        Contenu.Controls.Add(_lblStats)
        Contenu.Controls.Add(barre)

        RafraichirListe()
        AfficherStats()
    End Sub

    Private Sub SurTexteModifie(ByVal sender As Object, ByVal e As EventArgs)
        AfficherStats()
    End Sub

    Private Sub AfficherStats()
        _lblStats.Text = StatistiquesTexte.Analyser(_rtb.Text).ToString()
    End Sub

    Private Sub BasculerStyle(ByVal style As FontStyle)
        Dim police As Font = _rtb.SelectionFont
        If police Is Nothing Then Return   ' sélection à styles mixtes
        Dim nouveau As FontStyle = If((police.Style And style) = style, police.Style And Not style, police.Style Or style)
        _rtb.SelectionFont = New Font(police, nouveau)
        _rtb.Focus()
    End Sub

    Private Sub SurGras(ByVal sender As Object, ByVal e As EventArgs)
        BasculerStyle(FontStyle.Bold)
    End Sub

    Private Sub SurItalique(ByVal sender As Object, ByVal e As EventArgs)
        BasculerStyle(FontStyle.Italic)
    End Sub

    Private Sub SurSouligne(ByVal sender As Object, ByVal e As EventArgs)
        BasculerStyle(FontStyle.Underline)
    End Sub

    Private Sub SurCouleur(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New ColorDialog() With {.Color = _rtb.SelectionColor}
            If dlg.ShowDialog() = DialogResult.OK Then
                _rtb.SelectionColor = dlg.Color
                _rtb.Focus()
            End If
        End Using
    End Sub

    Private Sub SurNouveau(ByVal sender As Object, ByVal e As EventArgs)
        _rtb.Clear()
        _txtTitre.Text = "Sans titre"
        AfficherStats()
    End Sub

    Private Sub SurEnregistrer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim id As Integer = DepotDocument.Enregistrer(If(String.IsNullOrWhiteSpace(_txtTitre.Text), "Sans titre", _txtTitre.Text.Trim()), _rtb.Rtf)
            _lblStats.ForeColor = Color.Green
            _lblStats.Text = "Document enregistré (id = " & id.ToString() & "). " & StatistiquesTexte.Analyser(_rtb.Text).ToString()
            RafraichirListe()
        Catch ex As Exception
            _lblStats.ForeColor = Color.Firebrick
            _lblStats.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurCharger(ByVal sender As Object, ByVal e As EventArgs)
        Dim entree = TryCast(_cboDocs.SelectedItem, ElementDoc)
        If entree Is Nothing Then Return
        Try
            Dim rtf As String = DepotDocument.Recharger(entree.Id)
            Try
                _rtb.Rtf = rtf
            Catch
                _rtb.Text = rtf   ' au cas où le contenu ne serait pas du RTF valide
            End Try
            _txtTitre.Text = entree.Titre
            _lblStats.ForeColor = Color.DimGray
            AfficherStats()
        Catch ex As Exception
            _lblStats.ForeColor = Color.Firebrick
            _lblStats.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub RafraichirListe()
        Try
            _cboDocs.Items.Clear()
            For Each entete As KeyValuePair(Of Integer, String) In DepotDocument.ListerEntetes()
                _cboDocs.Items.Add(New ElementDoc(entete.Key, entete.Value))
            Next
            If _cboDocs.Items.Count > 0 Then _cboDocs.SelectedIndex = 0
        Catch
            ' base indisponible : la liste reste vide.
        End Try
    End Sub

    Private NotInheritable Class ElementDoc
        Public ReadOnly Property Id As Integer
        Public ReadOnly Property Titre As String
        Public Sub New(ByVal id As Integer, ByVal titre As String)
            Me.Id = id
            Me.Titre = titre
        End Sub
        Public Overrides Function ToString() As String
            Return "#" & Id.ToString() & " — " & Titre
        End Function
    End Class

End Class

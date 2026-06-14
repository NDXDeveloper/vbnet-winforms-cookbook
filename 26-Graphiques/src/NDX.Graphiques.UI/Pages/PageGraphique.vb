' ============================================================================
'  PageGraphique.vb  -  Trace une serie : barres / courbe / points.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Graphiques

''' <summary>Affiche une série dans le ControleGraphique et permet de changer de mode.</summary>
Public NotInheritable Class PageGraphique
    Inherits PageBase

    Private ReadOnly _graphique As New ControleGraphique() With {.Dock = DockStyle.Fill}
    Private ReadOnly _cboType As New ComboBox() With {.Width = 110, .DropDownStyle = ComboBoxStyle.DropDownList}
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Public Sub New()
        MyBase.New("Graphique", "Choisissez le mode (barres / courbe / points). Chargez les données de démonstration ou celles de la base.")
        Construire()
        _graphique.Serie = SerieDemonstration()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        _cboType.Items.AddRange(New Object() {"Barres", "Courbe", "Points"})
        _cboType.SelectedIndex = 0
        AddHandler _cboType.SelectedIndexChanged, Sub(s, e) _graphique.TypeAffichage = CType(_cboType.SelectedIndex, TypeGraphique)
        haut.Controls.Add(New Label() With {.Text = "Mode :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_cboType)
        haut.Controls.Add(Bouton("Données de démo", AddressOf SurDemo))
        haut.Controls.Add(Bouton("Charger depuis la base", AddressOf SurCharger))
        haut.Controls.Add(_lblEtat)

        Contenu.Controls.Add(_graphique)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurDemo(ByVal sender As Object, ByVal e As EventArgs)
        _graphique.Serie = SerieDemonstration()
        _lblEtat.ForeColor = Color.DimGray
        _lblEtat.Text = "Série de démonstration."
    End Sub

    Private Sub SurCharger(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim serie As SerieDonnees = DepotMesure.ChargerSerie()
            serie.Couleur = FrmPrincipale.CouleurAccent
            _graphique.Serie = serie
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = serie.Nombre.ToString() & " mesure(s) chargée(s)."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Shared Function SerieDemonstration() As SerieDonnees
        Dim serie As New SerieDonnees() With {.Nom = "Démo", .Couleur = Color.FromArgb(0, 151, 167)}
        Dim libelles As String() = {"Jan", "Fév", "Mar", "Avr", "Mai", "Juin"}
        Dim valeurs As Double() = {12, 19, 8, 23, 17, 27}
        For i As Integer = 0 To libelles.Length - 1
            serie.Ajouter(libelles(i), valeurs(i))
        Next
        Return serie
    End Function

End Class

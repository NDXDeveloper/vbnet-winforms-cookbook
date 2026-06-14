Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Page "Controles WinForms" : style de grille, double buffer, bouton arrondi, ComboBox owner-draw.</summary>
Public NotInheritable Class PageControles
    Inherits PageBase

    Private ReadOnly _dgv As New DataGridView()
    Private ReadOnly _combo As New ComboBox()
    Private ReadOnly _btnDemo As New Button()
    Private ReadOnly _nudId As New NumericUpDown()
    Private ReadOnly _lblInfo As New Label()

    Public Sub New()
        MyBase.New("Controles WinForms",
                   "AppliquerStyleGrille, ActiverDoubleBuffer (reflexion), ArrondirBouton (GraphicsPath), ComboBox owner-draw, CentrerFenetre.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True}
        barre.Controls.Add(Bouton("Appliquer style + double buffer", AddressOf SurStyle))
        barre.Controls.Add(Bouton("Arrondir le bouton de droite", AddressOf SurArrondir))
        barre.Controls.Add(Bouton("Lister les controles", AddressOf SurLister))
        barre.Controls.Add(Bouton("Centrer la fenetre", AddressOf SurCentrer))

        _lblInfo.Dock = DockStyle.Bottom
        _lblInfo.Height = 26
        _lblInfo.TextAlign = ContentAlignment.MiddleLeft

        Dim split As New SplitContainer() With {.Dock = DockStyle.Fill}

        ' --- Grille de gauche -------------------------------------------------
        _dgv.Dock = DockStyle.Fill
        _dgv.DataSource = ConstruireDonneesDemo()
        split.Panel1.Controls.Add(_dgv)

        ' --- Panneau de droite : combo owner-draw + bouton arrondi -----------
        Dim droite As New Panel() With {.Dock = DockStyle.Fill, .Padding = New Padding(10)}
        droite.Controls.Add(New Label() With {.Text = "ComboBox dessinee (owner-draw) :", .AutoSize = True, .Location = New Point(8, 8)})

        _combo.DropDownStyle = ComboBoxStyle.DropDownList
        _combo.DrawMode = DrawMode.OwnerDrawFixed
        _combo.Location = New Point(10, 30)
        _combo.Width = 220
        _combo.BackColor = Color.FromArgb(60, 60, 80)
        _combo.ForeColor = Color.White
        AddHandler _combo.DrawItem, AddressOf OutilsControles.DessinerElementCombo
        _combo.DataSource = ConstruireSourceCombo()
        _combo.DisplayMember = "libelle"
        _combo.ValueMember = "id"

        droite.Controls.Add(New Label() With {.Text = "Positionner la combo sur l'ID :", .AutoSize = True, .Location = New Point(8, 70)})
        _nudId.Minimum = 1
        _nudId.Maximum = 5
        _nudId.Value = 3
        _nudId.Location = New Point(10, 92)
        _nudId.Width = 60
        Dim btnPos As Button = Bouton("Positionner", AddressOf SurPositionner)
        btnPos.Location = New Point(80, 90)

        _btnDemo.Text = "Bouton de demo"
        _btnDemo.Size = New Size(160, 48)
        _btnDemo.Location = New Point(10, 150)

        droite.Controls.Add(_combo)
        droite.Controls.Add(_nudId)
        droite.Controls.Add(btnPos)
        droite.Controls.Add(_btnDemo)
        split.Panel2.Controls.Add(droite)

        Contenu.Controls.Add(split)
        Contenu.Controls.Add(_lblInfo)
        Contenu.Controls.Add(barre)
    End Sub

    Private Function ConstruireDonneesDemo() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Article", GetType(String))
        dt.Columns.Add("Quantite", GetType(Integer))
        dt.Columns.Add("Prix U.", GetType(Decimal))
        dt.Rows.Add("Poutre IPE 200", 12, 145.0D)
        dt.Rows.Add("Tube carre 40x40", 60, 12.5D)
        dt.Rows.Add("Tole acier 2mm", 25, 23.9D)
        dt.Rows.Add("Boulon HR M16", 120, 1.2D)
        Return dt
    End Function

    Private Function ConstruireSourceCombo() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("id", GetType(Integer))
        dt.Columns.Add("libelle", GetType(String))
        dt.Rows.Add(1, "Secteur Nord")
        dt.Rows.Add(2, "Secteur Sud")
        dt.Rows.Add(3, "Secteur Est")
        dt.Rows.Add(4, "Secteur Ouest")
        dt.Rows.Add(5, "Secteur Centre")
        Return dt
    End Function

    Private Sub SurStyle(ByVal sender As Object, ByVal e As EventArgs)
        OutilsControles.AppliquerStyleGrille(_dgv)
        OutilsControles.ActiverDoubleBuffer(_dgv)
        _lblInfo.Text = "Style applique et double buffer active (via reflexion sur la propriete protegee DoubleBuffered)."
    End Sub

    Private Sub SurArrondir(ByVal sender As Object, ByVal e As EventArgs)
        OutilsControles.ArrondirBouton(_btnDemo)
        _lblInfo.Text = "Region du bouton recalculee a partir d'un GraphicsPath (coins arrondis)."
    End Sub

    Private Sub SurPositionner(ByVal sender As Object, ByVal e As EventArgs)
        OutilsControles.PositionnerComboSurId(_combo, CInt(_nudId.Value))
        _lblInfo.Text = "Combo positionnee sur l'element dont la valeur (ValueMember) vaut " & _nudId.Value.ToString() & "."
    End Sub

    Private Sub SurLister(ByVal sender As Object, ByVal e As EventArgs)
        Dim controles As List(Of Control) = OutilsControles.TrouverControlesEnfants(Me)
        _lblInfo.Text = "Parcours recursif : " & controles.Count.ToString() & " controles descendants trouves dans cette page."
    End Sub

    Private Sub SurCentrer(ByVal sender As Object, ByVal e As EventArgs)
        Dim f As Form = Me.FindForm()
        If f IsNot Nothing Then
            OutilsControles.CentrerFenetre(f)
            _lblInfo.Text = "Fenetre recentree sur l'ecran principal."
        End If
    End Sub

End Class

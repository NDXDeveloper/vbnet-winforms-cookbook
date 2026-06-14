Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Reflection
Imports System.Windows.Forms

''' <summary>
''' Outils d'aide a la mise en forme des controles WinForms.
''' </summary>
''' <remarks>
''' Mise en forme de <see cref="DataGridView"/>, activation du double buffer par
''' reflexion, boutons aux coins arrondis (<see cref="GraphicsPath"/>),
''' positionnement et dessin personnalise de <see cref="ComboBox"/>, parcours
''' recursif de l'arborescence de controles, centrage de fenetre.
''' </remarks>
Public Module OutilsControles

    ''' <summary>Couleur d'accent reutilisee (bleu nuit).</summary>
    Public ReadOnly CouleurAccent As Color = Color.FromArgb(255, 53, 54, 82)

    ''' <summary>Applique un style de base lisible a une grille de donnees.</summary>
    Public Sub AppliquerStyleGrille(ByRef grille As DataGridView)
        Try
            grille.DefaultCellStyle.ForeColor = Color.Black
            grille.DefaultCellStyle.SelectionForeColor = Color.White
            grille.DefaultCellStyle.SelectionBackColor = CouleurAccent
            grille.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Empty
            grille.RowsDefaultCellStyle.BackColor = Color.White
            grille.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 246, 250)
            grille.EnableHeadersVisualStyles = False
            grille.ColumnHeadersDefaultCellStyle.BackColor = CouleurAccent
            grille.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            grille.BackgroundColor = Color.White
            grille.BorderStyle = BorderStyle.None
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Try
    End Sub

    ''' <summary>
    ''' Active le double buffer d'un <see cref="DataGridView"/> pour reduire le
    ''' scintillement. La propriete <c>DoubleBuffered</c> etant protegee, on
    ''' passe par la reflexion (technique classique mais a manier avec soin).
    ''' </summary>
    Public Sub ActiverDoubleBuffer(ByVal grille As DataGridView)
        Try
            Dim prop As PropertyInfo = grille.GetType().GetProperty(
                "DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic)
            prop?.SetValue(grille, True, Nothing)
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Try
    End Sub

    ''' <summary>
    ''' Donne des coins arrondis a un bouton en lui affectant une region calculee
    ''' a partir d'un <see cref="GraphicsPath"/>.
    ''' </summary>
    Public Sub ArrondirBouton(ByVal bouton As Button, Optional ByVal rayon As Integer = 20)
        Try
            bouton.FlatStyle = FlatStyle.Flat
            bouton.FlatAppearance.BorderSize = 0
            bouton.BackColor = Color.FromArgb(255, 247, 215, 179)
            bouton.ForeColor = CouleurAccent
            bouton.Cursor = Cursors.Hand

            Dim chemin As New GraphicsPath()
            chemin.StartFigure()
            chemin.AddArc(New Rectangle(0, 0, rayon, rayon), 180, 90)
            chemin.AddLine(rayon, 0, bouton.Width - rayon, 0)
            chemin.AddArc(New Rectangle(bouton.Width - rayon, 0, rayon, rayon), -90, 90)
            chemin.AddLine(bouton.Width, rayon, bouton.Width, bouton.Height - rayon)
            chemin.AddArc(New Rectangle(bouton.Width - rayon, bouton.Height - rayon, rayon, rayon), 0, 90)
            chemin.AddLine(bouton.Width - rayon, bouton.Height, rayon, bouton.Height)
            chemin.AddArc(New Rectangle(0, bouton.Height - rayon, rayon, rayon), 90, 90)
            chemin.CloseFigure()
            bouton.Region = New Region(chemin)
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Try
    End Sub

    ''' <summary>
    ''' Selectionne, dans une <see cref="ComboBox"/> liee a une source de
    ''' donnees, l'element dont la valeur (<c>SelectedValue</c>) correspond a l'ID.
    ''' </summary>
    Public Sub PositionnerComboSurId(ByRef combo As ComboBox, ByVal id As Integer)
        Try
            For i As Integer = 0 To combo.Items.Count - 1
                combo.SelectedIndex = i
                If combo.SelectedValue IsNot Nothing AndAlso Convert.ToInt32(combo.SelectedValue) = id Then
                    Return
                End If
            Next
            If combo.Items.Count > 0 Then combo.SelectedIndex = 0
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Try
    End Sub

    ''' <summary>
    ''' Parcourt recursivement l'arborescence d'un controle et ajoute chaque
    ''' enfant (et petit-enfant...) a la liste fournie.
    ''' </summary>
    Public Sub ListerTousLesControles(ByVal racine As Control, ByRef controles As List(Of Control))
        If controles Is Nothing Then controles = New List(Of Control)()
        For Each enfant As Control In racine.Controls
            controles.Add(enfant)
            ListerTousLesControles(enfant, controles)   ' recursion
        Next
    End Sub

    ''' <summary>Retourne la liste de tous les controles descendants d'un controle.</summary>
    Public Function TrouverControlesEnfants(ByVal racine As Control) As List(Of Control)
        Dim liste As New List(Of Control)()
        ListerTousLesControles(racine, liste)
        Return liste
    End Function

    ''' <summary>
    ''' Gestionnaire de dessin personnalise pour une <see cref="ComboBox"/> en
    ''' mode <see cref="DrawMode.OwnerDrawFixed"/> : fond sombre, texte clair,
    ''' surbrillance a la selection.
    ''' </summary>
    Public Sub DessinerElementCombo(ByVal sender As Object, ByVal e As DrawItemEventArgs)
        If e.Index < 0 Then Return
        Dim combo As ComboBox = TryCast(sender, ComboBox)
        If combo Is Nothing Then Return

        Dim selectionne As Boolean = (e.State And DrawItemState.Selected) = DrawItemState.Selected
        Dim couleurFond As Color = If(selectionne, CouleurAccent, combo.BackColor)
        Dim couleurTexte As Color = If(selectionne, Color.White, combo.ForeColor)

        Using pinceau As New SolidBrush(couleurFond)
            e.Graphics.FillRectangle(pinceau, e.Bounds)
        End Using
        Using pinceauTexte As New SolidBrush(couleurTexte)
            e.Graphics.DrawString(combo.Items(e.Index).ToString(), combo.Font, pinceauTexte,
                                  New PointF(e.Bounds.X + 2, e.Bounds.Y + 1))
        End Using
        e.DrawFocusRectangle()
    End Sub

    ''' <summary>Centre une fenetre sur la zone de travail de l'ecran principal.</summary>
    Public Sub CentrerFenetre(ByRef fenetre As Form)
        Dim zone As Rectangle = Screen.PrimaryScreen.WorkingArea
        Dim x As Integer = (zone.Width - fenetre.Width) \ 2
        Dim y As Integer = (zone.Height - fenetre.Height) \ 2
        fenetre.Location = New Point(zone.Left + x, zone.Top + y)
    End Sub

    ''' <summary>Met en surbrillance un bouton lorsqu'il prend le focus.</summary>
    Public Sub Bouton_PriseFocus(ByVal sender As Object, ByVal e As EventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then b.BackColor = Color.FromArgb(255, 247, 215, 179)
    End Sub

    ''' <summary>Restaure l'apparence d'un bouton lorsqu'il perd le focus.</summary>
    Public Sub Bouton_PerteFocus(ByVal sender As Object, ByVal e As EventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then b.BackColor = Color.Transparent
    End Sub

End Module

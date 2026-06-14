Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Classe de base des pages de la galerie : en-tete (titre + description) et
''' zone de contenu prete a accueillir les controles specifiques de la page.
''' </summary>
Public MustInherit Class PageBase
    Inherits UserControl

    ''' <summary>Zone ou chaque page derivee ajoute ses controles.</summary>
    Protected ReadOnly Contenu As New Panel()

    Protected Sub New(ByVal titre As String, ByVal description As String)
        Me.BackColor = Color.White

        Dim entete As New Panel() With {.Dock = DockStyle.Top, .Height = 62, .BackColor = OutilsControles.CouleurAccent}
        Dim lblTitre As New Label() With {
            .Text = titre, .ForeColor = Color.White, .Font = New Font("Segoe UI Semibold", 13.0F),
            .AutoSize = False, .Dock = DockStyle.Top, .Height = 34,
            .TextAlign = ContentAlignment.BottomLeft, .Padding = New Padding(12, 0, 0, 0)}
        Dim lblDescription As New Label() With {
            .Text = description, .ForeColor = Color.Gainsboro, .Font = New Font("Segoe UI", 8.5F),
            .AutoSize = False, .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.TopLeft, .Padding = New Padding(14, 2, 6, 4)}
        entete.Controls.Add(lblDescription)
        entete.Controls.Add(lblTitre)

        Contenu.Dock = DockStyle.Fill
        Contenu.Padding = New Padding(10)
        Contenu.BackColor = Color.White

        Me.Controls.Add(Contenu)
        Me.Controls.Add(entete)
    End Sub

    ''' <summary>Fabrique un bouton d'action homogene.</summary>
    Protected Function Bouton(ByVal texte As String, ByVal gestionnaire As EventHandler) As Button
        Dim b As New Button() With {
            .Text = texte, .AutoSize = True, .Margin = New Padding(4),
            .Padding = New Padding(8, 4, 8, 4), .FlatStyle = FlatStyle.System}
        AddHandler b.Click, gestionnaire
        Return b
    End Function

    ''' <summary>Fabrique une zone de texte de sortie (console) en lecture seule.</summary>
    Protected Function Console() As TextBox
        Return New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Both, .WordWrap = False,
            .Dock = DockStyle.Fill, .BackColor = Color.FromArgb(248, 248, 248),
            .Font = New Font("Consolas", 9.5F)}
    End Function

End Class

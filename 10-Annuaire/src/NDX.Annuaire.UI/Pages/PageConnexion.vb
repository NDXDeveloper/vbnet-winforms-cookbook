' ============================================================================
'  PageConnexion.vb  -  Formulaire de connexion (authentification LDAP).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Annuaire

''' <summary>Saisit un identifiant et un mot de passe, puis tente un bind LDAP.</summary>
Public NotInheritable Class PageConnexion
    Inherits PageBase

    Private ReadOnly _txtId As New TextBox() With {.Width = 220, .Text = "alice"}
    Private ReadOnly _txtMdp As New TextBox() With {.Width = 220, .UseSystemPasswordChar = True, .Text = "alice_pwd"}
    Private ReadOnly _sortie As New TextBox()

    Public Sub New()
        MyBase.New("Connexion", "Tente d'authentifier le compte par « bind » LDAP, puis affiche ses attributs en cas de succès.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim grille As New TableLayoutPanel() With {.Dock = DockStyle.Top, .Height = 96, .ColumnCount = 2, .RowCount = 3, .Padding = New Padding(6)}
        grille.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 140))
        grille.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        grille.Controls.Add(Etiquette("Identifiant :"), 0, 0)
        grille.Controls.Add(_txtId, 1, 0)
        grille.Controls.Add(Etiquette("Mot de passe :"), 0, 1)
        grille.Controls.Add(_txtMdp, 1, 1)
        grille.Controls.Add(New Label(), 0, 2)
        grille.Controls.Add(Bouton("Se connecter", AddressOf SurConnexion), 1, 2)

        _sortie.Multiline = True
        _sortie.ReadOnly = True
        _sortie.ScrollBars = ScrollBars.Vertical
        _sortie.Dock = DockStyle.Fill
        _sortie.BackColor = Color.FromArgb(248, 248, 248)
        _sortie.Font = New Font("Consolas", 10.0F)

        Contenu.Controls.Add(_sortie)
        Contenu.Controls.Add(grille)
    End Sub

    Private Function Etiquette(ByVal texte As String) As Label
        Return New Label() With {.Text = texte, .AutoSize = True, .Anchor = AnchorStyles.Left,
                                 .Font = New Font("Segoe UI", 10.0F), .Padding = New Padding(0, 6, 0, 0)}
    End Function

    Private Sub SurConnexion(ByVal sender As Object, ByVal e As EventArgs)
        Dim r As ResultatAuthentification = ServiceAnnuaire.Authentifier(_txtId.Text, _txtMdp.Text)
        If r.Reussite Then
            _sortie.ForeColor = Color.DarkGreen
            _sortie.Text =
                "✓ " & r.Message & vbCrLf & vbCrLf &
                "Identifiant : " & r.Compte.Identifiant & vbCrLf &
                "Nom complet : " & r.Compte.NomComplet & vbCrLf &
                "Courriel    : " & If(String.IsNullOrEmpty(r.Compte.Courriel), "(non renseigné)", r.Compte.Courriel) & vbCrLf &
                "DN          : " & r.Compte.NomDistinctif
        Else
            _sortie.ForeColor = Color.Firebrick
            _sortie.Text = "✗ " & r.Message
        End If
    End Sub

End Class

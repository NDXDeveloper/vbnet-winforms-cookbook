' ============================================================================
'  PageBascule.vb  -  Demo du controle BoutonBascule (owner-draw).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Controles

''' <summary>Affiche des interrupteurs et persiste leur état dans les préférences.</summary>
Public NotInheritable Class PageBascule
    Inherits PageBase

    Private Const CLE_NOTIF As String = "bascule.notifications"
    Private Const CLE_THEME As String = "bascule.theme_sombre"

    Private ReadOnly _bascNotif As New BoutonBascule()
    Private ReadOnly _bascTheme As New BoutonBascule()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .ForeColor = Color.DimGray}

    Public Sub New()
        MyBase.New("Interrupteur (owner-draw)", "Interrupteur dessiné à la main. Cliquez pour basculer ; enregistrez puis rechargez l'état depuis la base.")
        Construire()
        ChargerDepuisPreferences()
    End Sub

    Private Sub Construire()
        Dim grille As New TableLayoutPanel() With {.Dock = DockStyle.Top, .Height = 110, .ColumnCount = 2, .RowCount = 2, .Padding = New Padding(6)}
        grille.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 220))
        grille.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))

        grille.Controls.Add(Etiquette("Activer les notifications"), 0, 0)
        grille.Controls.Add(_bascNotif, 1, 0)
        grille.Controls.Add(Etiquette("Thème sombre"), 0, 1)
        grille.Controls.Add(_bascTheme, 1, 1)

        AddHandler _bascNotif.BasculeModifiee, AddressOf SurBascule
        AddHandler _bascTheme.BasculeModifiee, AddressOf SurBascule

        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44}
        barre.Controls.Add(Bouton("Enregistrer dans les préférences", AddressOf SurEnregistrer))
        barre.Controls.Add(Bouton("Recharger depuis la base", AddressOf SurRecharger))
        barre.Controls.Add(_lblEtat)

        Contenu.Controls.Add(barre)
        Contenu.Controls.Add(grille)
    End Sub

    Private Function Etiquette(ByVal texte As String) As Label
        Return New Label() With {.Text = texte, .AutoSize = True, .Anchor = AnchorStyles.Left,
                                 .Font = New Font("Segoe UI", 10.0F), .Padding = New Padding(0, 6, 0, 0)}
    End Function

    Private Sub SurBascule(ByVal sender As Object, ByVal e As EventArgs)
        _lblEtat.ForeColor = Color.DimGray
        _lblEtat.Text = "Notifications : " & Oui(_bascNotif.Actif) & "   |   Thème sombre : " & Oui(_bascTheme.Actif)
    End Sub

    Private Shared Function Oui(ByVal b As Boolean) As String
        Return If(b, "activé", "désactivé")
    End Function

    Private Sub SurEnregistrer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            DepotPreferences.Ecrire(CLE_NOTIF, _bascNotif.Actif.ToString())
            DepotPreferences.Ecrire(CLE_THEME, _bascTheme.Actif.ToString())
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "État enregistré."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurRecharger(ByVal sender As Object, ByVal e As EventArgs)
        ChargerDepuisPreferences()
    End Sub

    Private Sub ChargerDepuisPreferences()
        Try
            _bascNotif.Actif = ValeurBool(DepotPreferences.Lire(CLE_NOTIF, "False"))
            _bascTheme.Actif = ValeurBool(DepotPreferences.Lire(CLE_THEME, "False"))
            _lblEtat.ForeColor = Color.DimGray
            _lblEtat.Text = "État chargé depuis la base."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Base indisponible : " & ex.Message
        End Try
    End Sub

    Private Shared Function ValeurBool(ByVal texte As String) As Boolean
        Dim b As Boolean
        Return Boolean.TryParse(texte, b) AndAlso b
    End Function

End Class

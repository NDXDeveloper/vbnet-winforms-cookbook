' ============================================================================
'  FormulaireBase.vb  -  Fiche de base reutilisable par heritage visuel.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Formulaire de base dont héritent les fiches concrètes : il fournit un en-tête
''' (bandeau + titre) et une zone de contenu communs, ainsi que l'application d'un
''' thème. C'est l'<b>héritage visuel</b> : on factorise l'apparence une seule fois.
''' </summary>
Public Class FormulaireBase
    Inherits Form

    Private ReadOnly _entete As New Panel()
    Private ReadOnly _lblTitre As New Label()

    ''' <summary>Zone où les fiches dérivées placent leurs contrôles.</summary>
    Protected ReadOnly Contenu As New Panel()

    Public Sub New()
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.Size = New Size(560, 420)
        Me.StartPosition = FormStartPosition.CenterParent

        _entete.Dock = DockStyle.Top
        _entete.Height = 54
        _lblTitre.Dock = DockStyle.Fill
        _lblTitre.TextAlign = ContentAlignment.MiddleLeft
        _lblTitre.Font = New Font("Segoe UI Semibold", 13.0F)
        _lblTitre.ForeColor = Color.White
        _lblTitre.Padding = New Padding(14, 0, 0, 0)
        _entete.Controls.Add(_lblTitre)

        Contenu.Dock = DockStyle.Fill
        Contenu.Padding = New Padding(14)

        Me.Controls.Add(Contenu)
        Me.Controls.Add(_entete)

        AppliquerTheme(GestionnaireThemes.Clair)
    End Sub

    ''' <summary>Titre affiché dans l'en-tête de la fiche.</summary>
    Public Property TitreFiche As String
        Get
            Return _lblTitre.Text
        End Get
        Set(ByVal value As String)
            _lblTitre.Text = value
        End Set
    End Property

    ''' <summary>Applique un thème : fond/texte sur le contenu, couleur d'accent sur l'en-tête.</summary>
    Public Sub AppliquerTheme(ByVal theme As Theme)
        If theme Is Nothing Then Return
        Me.BackColor = theme.Fond
        GestionnaireThemes.AppliquerSur(Contenu, theme)
        _entete.BackColor = theme.Accent
        _lblTitre.ForeColor = Color.White
    End Sub

End Class

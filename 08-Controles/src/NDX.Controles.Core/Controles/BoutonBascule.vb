' ============================================================================
'  BoutonBascule.vb  -  Interrupteur "on/off" dessine entierement (owner-draw).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Interrupteur bascule (« toggle ») peint à la main. Illustre le dessin
''' personnalisé (<see cref="OnPaint"/>), le double tampon (anti-scintillement)
''' et l'exposition d'une propriété + d'un événement personnalisés.
''' </summary>
<DefaultEvent("BasculeModifiee")>
Public Class BoutonBascule
    Inherits Control

    Private _actif As Boolean

    Public Sub New()
        ' AllPaintingInWmPaint + UserPaint + OptimizedDoubleBuffer = dessin sans scintillement.
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        Me.Size = New Size(64, 28)
        Me.Cursor = Cursors.Hand
    End Sub

    ''' <summary>État de l'interrupteur (allumé / éteint).</summary>
    <DefaultValue(False)>
    Public Property Actif As Boolean
        Get
            Return _actif
        End Get
        Set(ByVal value As Boolean)
            If _actif <> value Then
                _actif = value
                Invalidate()
                RaiseEvent BasculeModifiee(Me, EventArgs.Empty)
            End If
        End Set
    End Property

    ''' <summary>Couleur du fond lorsque l'interrupteur est allumé.</summary>
    Public Property CouleurActive As Color = Color.FromArgb(76, 175, 80)

    ''' <summary>Couleur du fond lorsque l'interrupteur est éteint.</summary>
    Public Property CouleurInactive As Color = Color.FromArgb(189, 189, 189)

    ''' <summary>Déclenché à chaque changement d'état.</summary>
    Public Event BasculeModifiee As EventHandler

    ''' <summary>Inverse l'état courant.</summary>
    Public Sub Basculer()
        Actif = Not Actif
    End Sub

    Protected Overrides Sub OnClick(ByVal e As EventArgs)
        Basculer()
        MyBase.OnClick(e)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim rect As New Rectangle(0, 0, Math.Max(1, Me.Width - 1), Math.Max(1, Me.Height - 1))
        Using chemin As GraphicsPath = CheminArrondi(rect, rect.Height \ 2)
            Using pinceau As New SolidBrush(If(_actif, CouleurActive, CouleurInactive))
                g.FillPath(pinceau, chemin)
            End Using
        End Using
        Dim marge As Integer = 2
        Dim diametre As Integer = Me.Height - 2 * marge
        Dim x As Integer = If(_actif, Me.Width - diametre - marge, marge)
        Using pastille As New SolidBrush(Color.White)
            g.FillEllipse(pastille, x, marge, diametre, diametre)
        End Using
    End Sub

    ''' <summary>Construit un rectangle aux coins arrondis.</summary>
    Private Shared Function CheminArrondi(ByVal rect As Rectangle, ByVal rayon As Integer) As GraphicsPath
        Dim chemin As New GraphicsPath()
        Dim d As Integer = Math.Max(1, rayon * 2)
        chemin.AddArc(rect.X, rect.Y, d, d, 90, 90)
        chemin.AddArc(rect.X, rect.Bottom - d, d, d, 180, 90)
        chemin.AddArc(rect.Right - d, rect.Bottom - d, d, d, 270, 90)
        chemin.AddArc(rect.Right - d, rect.Y, d, d, 0, 90)
        chemin.CloseFigure()
        Return chemin
    End Function

End Class

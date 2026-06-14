' ============================================================================
'  VisionneuseImage.vb  -  Visionneuse avec zoom (molette) et deplacement (drag).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Affiche une image avec zoom à la molette et déplacement à la souris. Illustre
''' le double tampon, une transformation géométrique (translation + échelle) et la
''' gestion des événements souris. L'arithmétique du zoom est déléguée à
''' <see cref="CalculZoom"/> (testable séparément).
''' </summary>
Public Class VisionneuseImage
    Inherits Control

    Private _image As Image
    Private _zoom As Double = 1.0
    Private _decalage As PointF = PointF.Empty
    Private _glisse As Boolean
    Private _origineSouris As Point
    Private _origineDecalage As PointF

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        Me.BackColor = Color.FromArgb(38, 38, 38)
        Me.Cursor = Cursors.SizeAll
    End Sub

    ''' <summary>Image affichée (le zoom et le déplacement sont réinitialisés).</summary>
    <Browsable(False)>
    Public Property Image As Image
        Get
            Return _image
        End Get
        Set(ByVal value As Image)
            _image = value
            Recentrer()
        End Set
    End Property

    ''' <summary>Facteur de zoom courant (1.0 = taille réelle).</summary>
    <Browsable(False)>
    Public ReadOnly Property Zoom As Double
        Get
            Return _zoom
        End Get
    End Property

    ''' <summary>Déclenché quand le zoom change (pour affichage / sauvegarde).</summary>
    Public Event ZoomModifie As EventHandler

    ''' <summary>Réinitialise le zoom à 100 % et recentre l'image.</summary>
    Public Sub Recentrer()
        _zoom = 1.0
        _decalage = PointF.Empty
        Invalidate()
        RaiseEvent ZoomModifie(Me, EventArgs.Empty)
    End Sub

    Protected Overrides Sub OnMouseWheel(ByVal e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        If _image Is Nothing Then Return
        Dim crans As Integer = e.Delta \ 120
        Dim avant As Double = _zoom
        _zoom = CalculZoom.AppliquerCrans(_zoom, crans)
        If _zoom <> avant Then
            Invalidate()
            RaiseEvent ZoomModifie(Me, EventArgs.Empty)
        End If
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button = MouseButtons.Left Then
            _glisse = True
            _origineSouris = e.Location
            _origineDecalage = _decalage
        End If
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If _glisse Then
            _decalage = New PointF(_origineDecalage.X + (e.X - _origineSouris.X),
                                   _origineDecalage.Y + (e.Y - _origineSouris.Y))
            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        _glisse = False
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        If _image Is Nothing Then
            TextRenderer.DrawText(g, "Aucune image", Me.Font, Me.ClientRectangle, Color.Gainsboro,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            Return
        End If
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        ' Centre + déplacement, puis mise à l'échelle : l'image est dessinée
        ' centrée sur son origine pour zoomer "autour" du centre de la vue.
        g.TranslateTransform(Me.Width / 2.0F + _decalage.X, Me.Height / 2.0F + _decalage.Y)
        g.ScaleTransform(CSng(_zoom), CSng(_zoom))
        g.DrawImage(_image, -_image.Width / 2.0F, -_image.Height / 2.0F, _image.Width, _image.Height)
        g.ResetTransform()
    End Sub

End Class

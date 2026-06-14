' ============================================================================
'  Canevas.vb  -  Zone de dessin double tampon : creation et deplacement de formes.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Dessin

''' <summary>
''' Panneau de dessin (double tampon) : en mode création, on glisse pour tracer une
''' forme ; en mode déplacement, on saisit la forme la plus haute sous le curseur
''' (hit-testing via <see cref="Scene.TrouverA"/>) et on la déplace.
''' </summary>
Public NotInheritable Class Canevas
    Inherits Panel

    Public ReadOnly Property Scene As New Scene()
    Public Property TypeCourant As TypeForme = TypeForme.Rectangle
    Public Property CouleurCourante As Color = Color.FromArgb(21, 101, 192)
    Public Property ModeDeplacement As Boolean = False

    Private _enCours As Forme
    Private _selection As Forme
    Private _depart As Point
    Private _decalage As Size

    Public Sub New()
        Me.DoubleBuffered = True
        Me.BackColor = Color.White
        Me.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Public Sub Reinitialiser(ByVal nouvelle As Scene)
        Scene.Vider()
        If nouvelle IsNot Nothing Then
            For Each f As Forme In nouvelle.Formes
                Scene.Ajouter(f)
            Next
        End If
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button <> MouseButtons.Left Then Return
        If ModeDeplacement Then
            _selection = Scene.TrouverA(e.X, e.Y)
            If _selection IsNot Nothing Then _decalage = New Size(e.X - _selection.X, e.Y - _selection.Y)
        Else
            _depart = e.Location
            _enCours = New Forme() With {
                .Type = TypeCourant, .X = e.X, .Y = e.Y, .Largeur = 0, .Hauteur = 0,
                .CouleurHex = ColorTranslator.ToHtml(CouleurCourante)}
        End If
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If ModeDeplacement Then
            If _selection IsNot Nothing AndAlso e.Button = MouseButtons.Left Then
                _selection.X = e.X - _decalage.Width
                _selection.Y = e.Y - _decalage.Height
                Invalidate()
            End If
        ElseIf _enCours IsNot Nothing Then
            _enCours.Largeur = e.X - _depart.X
            _enCours.Hauteur = e.Y - _depart.Y
            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        If Not ModeDeplacement AndAlso _enCours IsNot Nothing Then
            If Math.Abs(_enCours.Largeur) >= 3 OrElse Math.Abs(_enCours.Hauteur) >= 3 Then Scene.Ajouter(_enCours)
            _enCours = Nothing
            Invalidate()
        End If
        _selection = Nothing
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Scene.DessinerSur(e.Graphics)
        If _enCours IsNot Nothing Then _enCours.DessinerSur(e.Graphics)
    End Sub

End Class

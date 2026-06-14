' ============================================================================
'  BoutonEtat.vb  -  Bouton dessine a la main, avec etats et IButtonControl.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Bouton peint à la main qui change d'apparence selon son <see cref="EtatBouton"/>
''' (normal / survol / enfoncé / désactivé). Implémente <see cref="IButtonControl"/>
''' pour servir de bouton par défaut / d'annulation dans une boîte de dialogue.
''' L'état est calculé par <see cref="CalculEtat"/> (testable séparément).
''' </summary>
Public Class BoutonEtat
    Inherits Control
    Implements IButtonControl

    Private _survol As Boolean
    Private _enfonce As Boolean

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or
                 ControlStyles.SupportsTransparentBackColor, True)
        Me.Size = New Size(150, 40)
        Me.Cursor = Cursors.Hand
        Me.ForeColor = Color.White
        Me.Font = New Font("Segoe UI Semibold", 9.5F)
    End Sub

    Public Property CouleurNormale As Color = Color.FromArgb(33, 150, 243)
    Public Property CouleurSurvol As Color = Color.FromArgb(66, 165, 245)
    Public Property CouleurEnfonce As Color = Color.FromArgb(21, 101, 192)
    Public Property CouleurDesactive As Color = Color.FromArgb(189, 189, 189)

    ''' <summary>État visuel courant (déduit par <see cref="CalculEtat"/>).</summary>
    Public ReadOnly Property EtatCourant As EtatBouton
        Get
            Return CalculEtat.Determiner(Me.Enabled, _survol, _enfonce)
        End Get
    End Property

    Private Function CouleurEtat() As Color
        Select Case EtatCourant
            Case EtatBouton.Survol : Return CouleurSurvol
            Case EtatBouton.Enfonce : Return CouleurEnfonce
            Case EtatBouton.Desactive : Return CouleurDesactive
            Case Else : Return CouleurNormale
        End Select
    End Function

    Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)
        _survol = True : Invalidate() : MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
        _survol = False : Invalidate() : MyBase.OnMouseLeave(e)
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then _enfonce = True : Invalidate()
        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        _enfonce = False : Invalidate() : MyBase.OnMouseUp(e)
    End Sub

    Protected Overrides Sub OnEnabledChanged(ByVal e As EventArgs)
        Invalidate() : MyBase.OnEnabledChanged(e)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim rect As New Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1))
        Using chemin As GraphicsPath = CheminArrondi(rect, 6)
            Using pinceau As New SolidBrush(CouleurEtat())
                e.Graphics.FillPath(pinceau, chemin)
            End Using
        End Using
        TextRenderer.DrawText(e.Graphics, Me.Text, Me.Font, Me.ClientRectangle, Me.ForeColor,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    Private Shared Function CheminArrondi(ByVal rect As Rectangle, ByVal rayon As Integer) As GraphicsPath
        Dim chemin As New GraphicsPath()
        Dim d As Integer = Math.Max(1, rayon * 2)
        chemin.AddArc(rect.X, rect.Y, d, d, 180, 90)
        chemin.AddArc(rect.Right - d, rect.Y, d, d, 270, 90)
        chemin.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90)
        chemin.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90)
        chemin.CloseFigure()
        Return chemin
    End Function

    ' --- IButtonControl : permet d'être le bouton « par défaut » / « annuler » d'un formulaire ---
    Public Property DialogResult As DialogResult Implements IButtonControl.DialogResult

    Public Sub NotifyDefault(ByVal value As Boolean) Implements IButtonControl.NotifyDefault
        ' On pourrait souligner le bouton par défaut ; sans effet visuel ici.
    End Sub

    Public Sub PerformClick() Implements IButtonControl.PerformClick
        If Me.Enabled Then OnClick(EventArgs.Empty)
    End Sub

End Class

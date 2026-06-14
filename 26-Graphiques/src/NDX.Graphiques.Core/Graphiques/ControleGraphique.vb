' ============================================================================
'  ControleGraphique.vb  -  Controle owner-draw tracant une serie de donnees.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Trace une <see cref="SerieDonnees"/> sous forme de barres, de courbe ou de
''' points (double tampon). La conversion valeur → pixel est déléguée à
''' <see cref="EchelleGraphique"/> (testable à part) ; ici on ne fait que dessiner.
''' </summary>
Public Class ControleGraphique
    Inherits Control

    Private _serie As SerieDonnees

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        Me.BackColor = Color.White
        Me.Size = New Size(480, 280)
    End Sub

    Public Property Serie As SerieDonnees
        Get
            Return _serie
        End Get
        Set(ByVal value As SerieDonnees)
            _serie = value
            Invalidate()
        End Set
    End Property

    Private _type As TypeGraphique = TypeGraphique.Barres
    Public Property TypeAffichage As TypeGraphique
        Get
            Return _type
        End Get
        Set(ByVal value As TypeGraphique)
            _type = value
            Invalidate()
        End Set
    End Property

    Private Const MARGE_GAUCHE As Integer = 40
    Private Const MARGE_BAS As Integer = 24
    Private Const MARGE_HAUT As Integer = 14
    Private Const MARGE_DROITE As Integer = 14

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        If _serie Is Nothing OrElse _serie.Nombre = 0 Then
            TextRenderer.DrawText(g, "Aucune donnée à afficher.", Me.Font, Me.ClientRectangle, Color.Gray,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            Return
        End If

        Dim x0 As Integer = MARGE_GAUCHE
        Dim y0 As Integer = MARGE_HAUT
        Dim largeur As Integer = Math.Max(1, Me.Width - MARGE_GAUCHE - MARGE_DROITE)
        Dim hauteur As Integer = Math.Max(1, Me.Height - MARGE_HAUT - MARGE_BAS)

        Dim echelle As New EchelleGraphique(EchelleGraphique.Auto(_serie.Valeurs, hauteur).Bas,
                                             EchelleGraphique.Auto(_serie.Valeurs, hauteur).Haut, hauteur)
        Dim yZero As Single = CSng(y0 + echelle.VersY(0))

        ' Axes.
        Using stylo As New Pen(Color.Gainsboro)
            g.DrawLine(stylo, x0, y0, x0, y0 + hauteur)             ' axe Y
            g.DrawLine(stylo, x0, yZero, x0 + largeur, yZero)        ' axe X (valeur 0)
        End Using
        ' Repères min/max.
        TextRenderer.DrawText(g, echelle.Haut.ToString("0.#"), Me.Font, New Point(2, y0 - 2), Color.Gray)
        TextRenderer.DrawText(g, echelle.Bas.ToString("0.#"), Me.Font, New Point(2, y0 + hauteur - 14), Color.Gray)

        Dim n As Integer = _serie.Nombre
        Dim pas As Single = largeur / CSng(n)

        Select Case _type
            Case TypeGraphique.Barres
                Using pinceau As New SolidBrush(_serie.Couleur)
                    For i As Integer = 0 To n - 1
                        Dim cx As Single = x0 + (i + 0.5F) * pas
                        Dim yVal As Single = CSng(y0 + echelle.VersY(_serie.Valeurs(i)))
                        Dim largeurBarre As Single = pas * 0.6F
                        Dim haut As Single = Math.Min(yVal, yZero)
                        Dim bas As Single = Math.Max(yVal, yZero)
                        g.FillRectangle(pinceau, cx - largeurBarre / 2, haut, largeurBarre, Math.Max(1, bas - haut))
                    Next
                End Using

            Case TypeGraphique.Courbe, TypeGraphique.Points
                Dim points As New List(Of PointF)()
                For i As Integer = 0 To n - 1
                    points.Add(New PointF(x0 + (i + 0.5F) * pas, CSng(y0 + echelle.VersY(_serie.Valeurs(i)))))
                Next
                If _type = TypeGraphique.Courbe AndAlso points.Count >= 2 Then
                    Using stylo As New Pen(_serie.Couleur, 2.0F)
                        g.DrawLines(stylo, points.ToArray())
                    End Using
                End If
                Using pinceau As New SolidBrush(_serie.Couleur)
                    For Each p As PointF In points
                        g.FillEllipse(pinceau, p.X - 3, p.Y - 3, 6, 6)
                    Next
                End Using
        End Select

        ' Etiquettes sous l'axe (si elles tiennent).
        If pas >= 24 Then
            For i As Integer = 0 To n - 1
                Dim etiquette As String = If(i < _serie.Libelles.Count, _serie.Libelles(i), (i + 1).ToString())
                Dim cx As Integer = CInt(x0 + (i + 0.5F) * pas)
                TextRenderer.DrawText(g, etiquette, Me.Font, New Rectangle(cx - CInt(pas / 2), y0 + hauteur + 2, CInt(pas), 18),
                                      Color.DimGray, TextFormatFlags.HorizontalCenter)
            Next
        End If
    End Sub

End Class

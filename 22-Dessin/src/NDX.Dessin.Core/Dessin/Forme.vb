' ============================================================================
'  Forme.vb  -  Une forme : geometrie, couleur, dessin et test de survol.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D

''' <summary>
''' Une forme vectorielle (rectangle, ellipse ou ligne) définie par sa position et
''' sa taille. Sait se dessiner et répondre au <b>test de contenance</b>
''' (hit-testing) : « ce point est-il sur moi ? » — logique pure, donc testable.
''' </summary>
Public NotInheritable Class Forme

    Public Property Type As TypeForme
    Public Property X As Integer
    Public Property Y As Integer
    Public Property Largeur As Integer
    Public Property Hauteur As Integer
    Public Property CouleurHex As String = "#1565C0"

    ''' <summary>Tolérance (px) pour cliquer « sur » une ligne.</summary>
    Public Const TOLERANCE_LIGNE As Double = 5.0

    ''' <summary>Indique si le point (px, py) est sur/dans la forme.</summary>
    Public Function Contient(ByVal px As Double, ByVal py As Double) As Boolean
        Select Case Type
            Case TypeForme.Rectangle
                Dim x1 = Math.Min(X, X + Largeur), x2 = Math.Max(X, X + Largeur)
                Dim y1 = Math.Min(Y, Y + Hauteur), y2 = Math.Max(Y, Y + Hauteur)
                Return px >= x1 AndAlso px <= x2 AndAlso py >= y1 AndAlso py <= y2
            Case TypeForme.Ellipse
                Dim rx As Double = Largeur / 2.0, ry As Double = Hauteur / 2.0
                If rx = 0 OrElse ry = 0 Then Return False
                Dim cx As Double = X + rx, cy As Double = Y + ry
                Dim dx As Double = (px - cx) / rx, dy As Double = (py - cy) / ry
                Return (dx * dx + dy * dy) <= 1.0
            Case TypeForme.Ligne
                Return DistanceAuSegment(px, py, X, Y, X + Largeur, Y + Hauteur) <= TOLERANCE_LIGNE
            Case Else
                Return False
        End Select
    End Function

    ''' <summary>Couleur .NET dérivée de la couleur hexadécimale.</summary>
    Public Function Couleur() As Color
        Try
            Return ColorTranslator.FromHtml(CouleurHex)
        Catch
            Return Color.SteelBlue
        End Try
    End Function

    ''' <summary>Dessine la forme sur un contexte graphique.</summary>
    Public Sub DessinerSur(ByVal g As Graphics)
        Using stylo As New Pen(Couleur(), 2.0F)
            Select Case Type
                Case TypeForme.Rectangle
                    Using pinceau As New SolidBrush(Color.FromArgb(50, Couleur()))
                        g.FillRectangle(pinceau, Normalise())
                    End Using
                    g.DrawRectangle(stylo, Normalise())
                Case TypeForme.Ellipse
                    Using pinceau As New SolidBrush(Color.FromArgb(50, Couleur()))
                        g.FillEllipse(pinceau, Normalise())
                    End Using
                    g.DrawEllipse(stylo, Normalise())
                Case TypeForme.Ligne
                    g.DrawLine(stylo, X, Y, X + Largeur, Y + Hauteur)
            End Select
        End Using
    End Sub

    Private Function Normalise() As Rectangle
        Return New Rectangle(Math.Min(X, X + Largeur), Math.Min(Y, Y + Hauteur), Math.Abs(Largeur), Math.Abs(Hauteur))
    End Function

    Private Shared Function DistanceAuSegment(ByVal px As Double, ByVal py As Double,
                                              ByVal ax As Double, ByVal ay As Double,
                                              ByVal bx As Double, ByVal by As Double) As Double
        Dim dx As Double = bx - ax, dy As Double = by - ay
        Dim longueur2 As Double = dx * dx + dy * dy
        If longueur2 = 0 Then Return Math.Sqrt((px - ax) ^ 2 + (py - ay) ^ 2)
        Dim t As Double = ((px - ax) * dx + (py - ay) * dy) / longueur2
        t = Math.Max(0.0, Math.Min(1.0, t))
        Dim projX As Double = ax + t * dx, projY As Double = ay + t * dy
        Return Math.Sqrt((px - projX) ^ 2 + (py - projY) ^ 2)
    End Function

End Class

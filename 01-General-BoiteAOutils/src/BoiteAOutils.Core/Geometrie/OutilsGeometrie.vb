Imports System.Drawing
Imports System.Reflection

''' <summary>
''' Calculs geometriques sur des points 2D.
''' </summary>
''' <remarks>
''' Operations elementaires sur des points 2D : point milieu, distance au carre
''' et rotation autour d'un centre.
''' </remarks>
Public Module OutilsGeometrie

    ''' <summary>Retourne le point milieu du segment [a, b].</summary>
    Public Function PointMilieu(ByVal a As Point, ByVal b As Point) As Point
        Try
            Return New Point(CInt((a.X + b.X) / 2), CInt((a.Y + b.Y) / 2))
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return Point.Empty
        End Try
    End Function

    ''' <summary>
    ''' Distance euclidienne <b>au carre</b> entre deux points. On evite la
    ''' racine carree : pour comparer des distances, le carre suffit et coute
    ''' moins cher en calcul.
    ''' </summary>
    Public Function DistanceAuCarre(ByVal pt1 As Point, ByVal pt2 As Point) As Integer
        Try
            Dim dx As Integer = pt1.X - pt2.X
            Dim dy As Integer = pt1.Y - pt2.Y
            Return dx * dx + dy * dy
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' Fait pivoter un point autour d'un centre, selon un angle en degres
    ''' (sens trigonometrique). Applique la matrice de rotation classique.
    ''' </summary>
    Public Function PivoterPoint(ByVal pointAPivoter As Point,
                                 ByVal centre As Point,
                                 ByVal angleEnDegres As Double) As Point
        Try
            Dim angleRad As Double = angleEnDegres * (Math.PI / 180.0)
            Dim cos As Double = Math.Cos(angleRad)
            Dim sin As Double = Math.Sin(angleRad)
            Dim dx As Double = pointAPivoter.X - centre.X
            Dim dy As Double = pointAPivoter.Y - centre.Y
            Return New Point(
                CInt(cos * dx - sin * dy + centre.X),
                CInt(sin * dx + cos * dy + centre.Y))
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return pointAPivoter
        End Try
    End Function

End Module

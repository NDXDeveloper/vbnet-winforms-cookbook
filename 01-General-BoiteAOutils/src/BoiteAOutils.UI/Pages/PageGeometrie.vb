Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>Page "Geometrie" : point milieu, distance au carre, rotation de point (dessin live).</summary>
Public NotInheritable Class PageGeometrie
    Inherits PageBase

    Private ReadOnly _pnlDessin As New Panel()
    Private ReadOnly _trkAngle As New TrackBar()
    Private ReadOnly _lblInfo As New Label()

    ' Points de demonstration (coordonnees relatives au panneau de dessin).
    Private ReadOnly _a As New Point(120, 120)
    Private ReadOnly _b As New Point(360, 300)

    Public Sub New()
        MyBase.New("Geometrie",
                   "PointMilieu, DistanceAuCarre et PivoterPoint. Deplacez le curseur pour faire varier l'angle de rotation.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New Panel() With {.Dock = DockStyle.Top, .Height = 64}
        haut.Controls.Add(New Label() With {.Text = "Angle de rotation (degres) :", .AutoSize = True, .Location = New Point(2, 6)})
        _trkAngle.Minimum = 0
        _trkAngle.Maximum = 360
        _trkAngle.Value = 30
        _trkAngle.TickFrequency = 30
        _trkAngle.Width = 420
        _trkAngle.Location = New Point(0, 24)
        AddHandler _trkAngle.ValueChanged, Sub() _pnlDessin.Invalidate()
        _lblInfo.AutoSize = True
        _lblInfo.Location = New Point(440, 28)
        haut.Controls.Add(_trkAngle)
        haut.Controls.Add(_lblInfo)

        _pnlDessin.Dock = DockStyle.Fill
        _pnlDessin.BackColor = Color.White
        AddHandler _pnlDessin.Paint, AddressOf SurDessin

        Contenu.Controls.Add(_pnlDessin)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurDessin(ByVal sender As Object, ByVal e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        ' --- Segment [A,B] et son milieu -------------------------------------
        Dim milieu As Point = OutilsGeometrie.PointMilieu(_a, _b)
        Using styloBleu As New Pen(OutilsControles.CouleurAccent, 2)
            g.DrawLine(styloBleu, _a, _b)
        End Using
        DessinerPoint(g, _a, "A", Color.SteelBlue)
        DessinerPoint(g, _b, "B", Color.SteelBlue)
        DessinerPoint(g, milieu, "Milieu", Color.SeaGreen)

        ' --- Rotation d'un point autour d'un centre --------------------------
        Dim centre As New Point(Math.Max(220, _pnlDessin.Width \ 2), Math.Max(160, _pnlDessin.Height \ 2))
        Dim rayonPoint As New Point(centre.X + 130, centre.Y)
        Dim pivote As Point = OutilsGeometrie.PivoterPoint(rayonPoint, centre, _trkAngle.Value)

        Using styloGris As New Pen(Color.Silver, 1)
            g.DrawEllipse(styloGris, centre.X - 130, centre.Y - 130, 260, 260)
            g.DrawLine(styloGris, centre, rayonPoint)
        End Using
        Using styloRouge As New Pen(Color.Firebrick, 2)
            g.DrawLine(styloRouge, centre, pivote)
        End Using
        DessinerPoint(g, centre, "Centre", Color.DimGray)
        DessinerPoint(g, pivote, "P (" & _trkAngle.Value.ToString() & " deg)", Color.Firebrick)

        _lblInfo.Text = "DistanceAuCarre(A,B) = " & OutilsGeometrie.DistanceAuCarre(_a, _b).ToString()
    End Sub

    Private Sub DessinerPoint(ByVal g As Graphics, ByVal p As Point, ByVal etiquette As String, ByVal couleur As Color)
        Using pinceau As New SolidBrush(couleur)
            g.FillEllipse(pinceau, p.X - 4, p.Y - 4, 8, 8)
            g.DrawString(etiquette, Me.Font, pinceau, p.X + 6, p.Y - 6)
        End Using
    End Sub

End Class

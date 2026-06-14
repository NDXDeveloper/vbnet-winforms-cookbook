' ============================================================================
'  PageVisionneuse.vb  -  Demo du controle VisionneuseImage (zoom/pan).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.Windows.Forms
Imports NDX.Controles

''' <summary>Affiche une image générée et permet zoom (molette) + déplacement (souris).</summary>
Public NotInheritable Class PageVisionneuse
    Inherits PageBase

    Private Const CLE_ZOOM As String = "visionneuse.dernier_zoom"

    Private ReadOnly _visionneuse As New VisionneuseImage()
    Private ReadOnly _lblZoom As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .ForeColor = Color.DimGray, .Padding = New Padding(8, 8, 0, 0)}

    Public Sub New()
        MyBase.New("Visionneuse (zoom/pan)", "Molette = zoom, glisser = déplacer. Le double tampon évite le scintillement pendant le rendu.")
        Construire()
    End Sub

    Private Sub Construire()
        _visionneuse.Dock = DockStyle.Fill
        _visionneuse.Image = ImageDemonstration()
        AddHandler _visionneuse.ZoomModifie, AddressOf SurZoom

        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Bottom, .Height = 44}
        barre.Controls.Add(Bouton("Recentrer (100 %)", AddressOf SurRecentrer))
        barre.Controls.Add(Bouton("Enregistrer le zoom", AddressOf SurEnregistrer))
        barre.Controls.Add(_lblZoom)

        Contenu.Controls.Add(_visionneuse)
        Contenu.Controls.Add(barre)
        AfficherZoom()
    End Sub

    Private Sub SurZoom(ByVal sender As Object, ByVal e As EventArgs)
        AfficherZoom()
    End Sub

    Private Sub AfficherZoom()
        _lblZoom.ForeColor = Color.DimGray
        _lblZoom.Text = "Zoom : " & (_visionneuse.Zoom * 100.0).ToString("0") & " %"
    End Sub

    Private Sub SurRecentrer(ByVal sender As Object, ByVal e As EventArgs)
        _visionneuse.Recentrer()
    End Sub

    Private Sub SurEnregistrer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            DepotPreferences.Ecrire(CLE_ZOOM, _visionneuse.Zoom.ToString("0.000", CultureInfo.InvariantCulture))
            _lblZoom.ForeColor = Color.Green
            _lblZoom.Text = "Zoom enregistré : " & (_visionneuse.Zoom * 100.0).ToString("0") & " %"
        Catch ex As Exception
            _lblZoom.ForeColor = Color.Firebrick
            _lblZoom.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    ''' <summary>Construit une image de démonstration (dégradé + formes) sans fichier externe.</summary>
    Private Shared Function ImageDemonstration() As Image
        Dim bmp As New Bitmap(480, 320)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            Using fond As New LinearGradientBrush(New Rectangle(0, 0, bmp.Width, bmp.Height),
                                                  Color.FromArgb(94, 53, 177), Color.FromArgb(33, 150, 243), 45.0F)
                g.FillRectangle(fond, 0, 0, bmp.Width, bmp.Height)
            End Using
            Using b As New SolidBrush(Color.FromArgb(180, Color.White))
                g.FillEllipse(b, 60, 60, 160, 160)
                g.FillRectangle(b, 280, 120, 140, 140)
            End Using
            Using stylo As New Pen(Color.White, 3.0F)
                g.DrawLine(stylo, 20, 290, 460, 30)
            End Using
            Using police As New Font("Segoe UI Semibold", 18.0F)
                g.DrawString("Zoom & déplacement", police, Brushes.White, 70, 270)
            End Using
        End Using
        Return bmp
    End Function

End Class

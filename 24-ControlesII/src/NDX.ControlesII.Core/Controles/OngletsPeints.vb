' ============================================================================
'  OngletsPeints.vb  -  TabControl aux onglets dessines a la main (owner-draw).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Onglets peints sur mesure : en activant <see cref="TabDrawMode.OwnerDrawFixed"/>,
''' on prend en charge le rendu de chaque onglet (couleur d'accent pour l'onglet
''' actif). Illustre l'owner-draw d'un contrôle composite standard.
''' </summary>
Public Class OngletsPeints
    Inherits TabControl

    Public Sub New()
        Me.DrawMode = TabDrawMode.OwnerDrawFixed
        Me.SizeMode = TabSizeMode.Fixed
        Me.ItemSize = New Size(140, 30)
        Me.Font = New Font("Segoe UI", 9.5F)
    End Sub

    Public Property CouleurAccent As Color = Color.FromArgb(33, 150, 243)
    Public Property CouleurInactif As Color = Color.FromArgb(236, 239, 241)

    Protected Overrides Sub OnDrawItem(ByVal e As DrawItemEventArgs)
        Dim rect As Rectangle = Me.GetTabRect(e.Index)
        Dim actif As Boolean = (e.Index = Me.SelectedIndex)
        Using fond As New SolidBrush(If(actif, CouleurAccent, CouleurInactif))
            e.Graphics.FillRectangle(fond, rect)
        End Using
        Using police As New Font(Me.Font, If(actif, FontStyle.Bold, FontStyle.Regular))
            TextRenderer.DrawText(e.Graphics, Me.TabPages(e.Index).Text, police, rect,
                                  If(actif, Color.White, Color.FromArgb(69, 90, 100)),
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End Using
    End Sub

End Class

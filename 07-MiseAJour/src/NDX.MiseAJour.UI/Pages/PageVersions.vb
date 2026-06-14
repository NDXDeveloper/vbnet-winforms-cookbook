' ============================================================================
'  PageVersions.vb  -  Demo du type VersionSemantique (comparaison, tri).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports NDX.MiseAJour

''' <summary>Compare deux versions et trie une liste : illustre IComparable et les opérateurs.</summary>
Public NotInheritable Class PageVersions
    Inherits PageBase

    Private ReadOnly _txtA As New TextBox() With {.Width = 120, .Text = "1.2.0"}
    Private ReadOnly _txtB As New TextBox() With {.Width = 120, .Text = "1.10.0"}
    Private ReadOnly _txtListe As New TextBox()
    Private ReadOnly _sortie As TextBox

    Public Sub New()
        MyBase.New("Versions (comparaison)", "Compare deux versions sémantiques et trie une liste. Note : 1.10.0 > 1.2.0 (comparaison numérique, pas alphabétique).")
        _sortie = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 44, .FlowDirection = FlowDirection.LeftToRight}
        haut.Controls.Add(New Label() With {.Text = "Version A :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_txtA)
        haut.Controls.Add(New Label() With {.Text = "Version B :", .AutoSize = True, .Margin = New Padding(8, 9, 2, 0)})
        haut.Controls.Add(_txtB)
        haut.Controls.Add(Bouton("Comparer", AddressOf SurComparer))

        Dim milieu As New Panel() With {.Dock = DockStyle.Top, .Height = 110}
        _txtListe.Multiline = True
        _txtListe.ScrollBars = ScrollBars.Vertical
        _txtListe.Dock = DockStyle.Fill
        _txtListe.Font = New Font("Consolas", 9.5F)
        _txtListe.Text = "1.2.0" & vbCrLf & "1.10.0" & vbCrLf & "1.2.10" & vbCrLf & "2.0.0" & vbCrLf & "v1.9.3" & vbCrLf & "0.9.0"
        Dim barreListe As New Panel() With {.Dock = DockStyle.Bottom, .Height = 36}
        barreListe.Controls.Add(Bouton("Trier la liste (croissant)", AddressOf SurTrier))
        milieu.Controls.Add(_txtListe)
        milieu.Controls.Add(barreListe)

        Contenu.Controls.Add(_sortie)
        Contenu.Controls.Add(milieu)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurComparer(ByVal sender As Object, ByVal e As EventArgs)
        Dim a, b As VersionSemantique
        If Not VersionSemantique.TryAnalyser(_txtA.Text, a) Then
            _sortie.Text = "Version A invalide." : Return
        End If
        If Not VersionSemantique.TryAnalyser(_txtB.Text, b) Then
            _sortie.Text = "Version B invalide." : Return
        End If
        Dim relation As String
        If a > b Then
            relation = a.ToString() & "  >  " & b.ToString() & "   (A est plus récente)"
        ElseIf a < b Then
            relation = a.ToString() & "  <  " & b.ToString() & "   (B est plus récente)"
        Else
            relation = a.ToString() & "  =  " & b.ToString() & "   (versions identiques)"
        End If
        _sortie.Text = "Normalisées : A = " & a.ToString() & " , B = " & b.ToString() & vbCrLf &
                       "Relation    : " & relation & vbCrLf &
                       "CompareTo   : " & a.CompareTo(b).ToString()
    End Sub

    Private Sub SurTrier(ByVal sender As Object, ByVal e As EventArgs)
        Dim lignes As String() = _txtListe.Lines.Where(Function(l) Not String.IsNullOrWhiteSpace(l)).ToArray()
        Dim versions As New List(Of VersionSemantique)()
        Dim invalides As New List(Of String)()
        For Each ligne As String In lignes
            Dim v As VersionSemantique
            If VersionSemantique.TryAnalyser(ligne, v) Then versions.Add(v) Else invalides.Add(ligne.Trim())
        Next
        versions.Sort()    ' utilise IComparable(Of VersionSemantique)
        Dim sb As New StringBuilder()
        sb.AppendLine("Tri croissant (" & versions.Count.ToString() & " versions) :")
        For Each v As VersionSemantique In versions
            sb.AppendLine("   " & v.ToString())
        Next
        If invalides.Count > 0 Then
            sb.AppendLine()
            sb.AppendLine("Ignorées (invalides) : " & String.Join(", ", invalides))
        End If
        _sortie.Text = sb.ToString()
    End Sub

End Class

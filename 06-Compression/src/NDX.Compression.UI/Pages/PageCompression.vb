' ============================================================================
'  PageCompression.vb  -  Essais de compression en memoire (sans base).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports NDX.Compression

''' <summary>Compresse un texte saisi, affiche le gain et verifie l'aller-retour.</summary>
Public NotInheritable Class PageCompression
    Inherits PageBase

    Private ReadOnly _txtSource As New TextBox()
    Private ReadOnly _rbGZip As New RadioButton() With {.Text = "GZip", .AutoSize = True, .Checked = True}
    Private ReadOnly _rbDeflate As New RadioButton() With {.Text = "Deflate", .AutoSize = True}
    Private ReadOnly _sortie As TextBox

    Public Sub New()
        MyBase.New("Compression (mémoire)", "Compresse le texte saisi, mesure le gain de place et vérifie l'identité après décompression.")
        _sortie = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New Panel() With {.Dock = DockStyle.Top, .Height = 150}

        _txtSource.Multiline = True
        _txtSource.ScrollBars = ScrollBars.Vertical
        _txtSource.Dock = DockStyle.Fill
        _txtSource.Font = New Font("Consolas", 9.5F)
        _txtSource.Text = TexteExemple()

        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Bottom, .Height = 40, .FlowDirection = FlowDirection.LeftToRight}
        barre.Controls.Add(New Label() With {.Text = "Algorithme :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        barre.Controls.Add(_rbGZip)
        barre.Controls.Add(_rbDeflate)
        barre.Controls.Add(Bouton("Compresser et vérifier", AddressOf SurCompresser))
        barre.Controls.Add(Bouton("Effacer", AddressOf SurEffacer))

        haut.Controls.Add(_txtSource)
        haut.Controls.Add(barre)

        Contenu.Controls.Add(_sortie)
        Contenu.Controls.Add(haut)
    End Sub

    Private Function AlgoChoisi() As AlgorithmeCompression
        Return If(_rbDeflate.Checked, AlgorithmeCompression.Deflate, AlgorithmeCompression.GZip)
    End Function

    Private Sub SurCompresser(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim algo As AlgorithmeCompression = AlgoChoisi()
            Dim source As String = _txtSource.Text
            Dim compresse As Byte() = Compresseur.CompresserTexte(source, algo)
            Dim aller As String = Compresseur.DecompresserTexte(compresse, algo)

            Dim tailleOrigine As Long = Encoding.UTF8.GetByteCount(source)
            Dim sb As New StringBuilder()
            sb.AppendLine("Algorithme         : " & algo.ToString())
            sb.AppendLine("Octets d'origine   : " & tailleOrigine.ToString("N0"))
            sb.AppendLine("Octets compressés  : " & compresse.Length.ToString("N0"))
            sb.AppendLine("Ratio (compr/orig) : " & Compresseur.Ratio(tailleOrigine, compresse.Length).ToString("0.000"))
            sb.AppendLine("Gain de place      : " & Compresseur.PourcentageGain(tailleOrigine, compresse.Length).ToString("0.0") & " %")
            sb.AppendLine("Aller-retour       : " & If(String.Equals(aller, source, StringComparison.Ordinal), "IDENTIQUE ✓", "DIFFÉRENT ✗"))
            sb.AppendLine()
            sb.AppendLine("Aperçu compressé (hex, 64 premiers octets) :")
            sb.AppendLine(Hex(compresse, 64))
            _sortie.Text = sb.ToString()
        Catch ex As Exception
            _sortie.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurEffacer(ByVal sender As Object, ByVal e As EventArgs)
        _sortie.Clear()
    End Sub

    Private Shared Function Hex(ByVal donnees As Byte(), ByVal maxi As Integer) As String
        Dim n As Integer = Math.Min(maxi, donnees.Length)
        Dim sb As New StringBuilder(n * 3)
        For i As Integer = 0 To n - 1
            sb.Append(donnees(i).ToString("X2")).Append(" "c)
            If (i + 1) Mod 16 = 0 Then sb.AppendLine()
        Next
        Return sb.ToString().TrimEnd()
    End Function

    Private Shared Function TexteExemple() As String
        ' Texte tres repetitif : la compression y est particulierement efficace.
        Dim sb As New StringBuilder()
        For i As Integer = 1 To 40
            sb.AppendLine("Ligne " & i.ToString() & " : la compression réduit les répétitions, la compression réduit les répétitions.")
        Next
        Return sb.ToString()
    End Function

End Class

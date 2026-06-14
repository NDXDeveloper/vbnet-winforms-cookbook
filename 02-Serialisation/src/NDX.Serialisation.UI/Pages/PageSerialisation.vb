' ============================================================================
'  PageSerialisation.vb
'  Demonstration des formats : serialiser un objet, verifier l'aller-retour,
'  comparer les tailles.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports NDX.Serialisation

''' <summary>Page de demonstration de la serialisation et des differents formats.</summary>
Public NotInheritable Class PageSerialisation
    Inherits PageBase

    Private ReadOnly _cboFormat As New ComboBox()
    Private ReadOnly _console As TextBox

    Public Sub New()
        MyBase.New("Sérialisation et formats",
                   "Sérialiser un objet d'exemple selon 4 formats, vérifier l'aller-retour et comparer les tailles.")
        _console = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True, .WrapContents = True}

        _cboFormat.DropDownStyle = ComboBoxStyle.DropDownList
        _cboFormat.Width = 130
        _cboFormat.DataSource = [Enum].GetValues(GetType(FormatSerialisation))

        barre.Controls.Add(New Label() With {.Text = "Format :", .AutoSize = True, .Margin = New Padding(4, 9, 0, 0)})
        barre.Controls.Add(_cboFormat)
        barre.Controls.Add(Bouton("Sérialiser l'exemple", AddressOf SurSerialiser))
        barre.Controls.Add(Bouton("Aller-retour (vérifier)", AddressOf SurAllerRetour))
        barre.Controls.Add(Bouton("Comparer les tailles", AddressOf SurComparer))
        barre.Controls.Add(Bouton("Effacer", Sub() _console.Clear()))

        Contenu.Controls.Add(_console)
        Contenu.Controls.Add(barre)
    End Sub

    Private Function FormatChoisi() As FormatSerialisation
        Return CType(_cboFormat.SelectedItem, FormatSerialisation)
    End Function

    Private Sub Tracer(ByVal texte As String)
        _console.AppendText(texte & vbCrLf)
    End Sub

    Private Sub SurSerialiser(ByVal sender As Object, ByVal e As EventArgs)
        Dim cat As Catalogue = Catalogue.Exemple()
        Dim format As FormatSerialisation = FormatChoisi()
        Dim octets As Byte() = Serialiseur.VersOctets(cat, format)

        Tracer("=== Sérialisation au format " & format.ToString() & " — " & octets.Length.ToString() & " octets ===")
        If Serialiseur.EstFormatTexte(format) Then
            Tracer(Serialiseur.VersChaine(cat, format))
        Else
            Tracer("(aperçu hexadécimal du contenu binaire)")
            Tracer(EnHexa(octets, 512))
        End If
        Tracer("")
    End Sub

    Private Sub SurAllerRetour(ByVal sender As Object, ByVal e As EventArgs)
        Dim cat As Catalogue = Catalogue.Exemple()
        Dim format As FormatSerialisation = FormatChoisi()

        Dim octets As Byte() = Serialiseur.VersOctets(cat, format)
        Dim copie As Catalogue = Serialiseur.DepuisOctets(Of Catalogue)(octets, format)

        Dim identique As Boolean =
            copie IsNot Nothing AndAlso
            copie.Nom = cat.Nom AndAlso
            copie.Produits.Count = cat.Produits.Count AndAlso
            copie.Produits(0).Reference = cat.Produits(0).Reference AndAlso
            copie.Produits(0).PrixHt = cat.Produits(0).PrixHt

        Tracer("Aller-retour (" & format.ToString() & ") : " &
               If(identique, "OK — objet reconstruit à l'identique", "ÉCHEC"))
        If identique Then
            Tracer("  " & copie.Nom & " — " & copie.Produits.Count.ToString() & " produits ; 1er = " & copie.Produits(0).ToString())
        End If
        Tracer("")
    End Sub

    Private Sub SurComparer(ByVal sender As Object, ByVal e As EventArgs)
        Dim cat As Catalogue = Catalogue.Exemple()
        Tracer("=== Taille du même objet selon le format ===")
        For Each format As FormatSerialisation In [Enum].GetValues(GetType(FormatSerialisation))
            Dim taille As Integer = Serialiseur.VersOctets(cat, format).Length
            Tracer(String.Format("  {0,-12} : {1,6} octets", format.ToString(), taille))
        Next
        Tracer("")
    End Sub

    ''' <summary>Rend un apercu hexadecimal lisible (16 octets par ligne).</summary>
    Private Function EnHexa(ByVal octets As Byte(), ByVal maximum As Integer) As String
        Dim sb As New StringBuilder()
        Dim limite As Integer = Math.Min(octets.Length, maximum)
        For i As Integer = 0 To limite - 1
            sb.Append(octets(i).ToString("X2")).Append(" "c)
            If (i + 1) Mod 16 = 0 Then sb.AppendLine()
        Next
        If octets.Length > maximum Then
            sb.AppendLine()
            sb.Append("... (" & (octets.Length - maximum).ToString() & " octets supplémentaires)")
        End If
        Return sb.ToString()
    End Function

End Class

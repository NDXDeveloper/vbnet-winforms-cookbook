' ============================================================================
'  CouleurHex.vb  -  Conversion (pure) entre Color et notation hexadecimale.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Globalization

''' <summary>
''' Convertit une <see cref="Color"/> en notation « #RRGGBB » et inversement.
''' Logique pure (chaînes/octets), testable sans interface graphique. Sert à
''' stocker un thème en base sous forme lisible.
''' </summary>
Public NotInheritable Class CouleurHex

    Private Sub New()
    End Sub

    ''' <summary>Couleur -> « #RRGGBB ».</summary>
    Public Shared Function VersHex(ByVal couleur As Color) As String
        Return "#" & couleur.R.ToString("X2") & couleur.G.ToString("X2") & couleur.B.ToString("X2")
    End Function

    ''' <summary>« #RRGGBB » (ou « RRGGBB ») -> Couleur ; lève <see cref="FormatException"/> si invalide.</summary>
    Public Shared Function DepuisHex(ByVal hex As String) As Color
        If String.IsNullOrWhiteSpace(hex) Then Throw New FormatException("Couleur vide.")
        Dim s As String = hex.Trim()
        If s.StartsWith("#") Then s = s.Substring(1)
        If s.Length <> 6 Then Throw New FormatException("Format attendu : #RRGGBB.")
        Try
            Dim r As Integer = Integer.Parse(s.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)
            Dim v As Integer = Integer.Parse(s.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)
            Dim b As Integer = Integer.Parse(s.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)
            Return Color.FromArgb(r, v, b)
        Catch ex As Exception
            Throw New FormatException("Hexadécimal invalide : « " & hex & " ».", ex)
        End Try
    End Function

End Class

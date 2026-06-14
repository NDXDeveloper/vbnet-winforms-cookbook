' ============================================================================
'  PagePdf.vb  -  Une page : API de dessin (texte, lignes, rectangles).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Globalization
Imports System.Text

''' <summary>
''' Une page d'un <see cref="DocumentPdf"/>. Construit le « flux de contenu » PDF
''' (suite d'opérateurs). L'API expose un repère <b>origine en haut à gauche</b>
''' (Y vers le bas), plus intuitif ; la conversion vers le repère PDF natif
''' (origine en bas à gauche) est faite en interne.
''' </summary>
Public NotInheritable Class PagePdf

    Private ReadOnly _contenu As New StringBuilder()

    Friend Sub New(ByVal largeur As Double, ByVal hauteur As Double)
        Me.Largeur = largeur
        Me.Hauteur = hauteur
    End Sub

    ''' <summary>Largeur de la page, en points (1/72 de pouce).</summary>
    Public ReadOnly Property Largeur As Double

    ''' <summary>Hauteur de la page, en points.</summary>
    Public ReadOnly Property Hauteur As Double

    ''' <summary>Écrit une ligne de texte ; <paramref name="y"/> est la ligne de base, mesurée depuis le haut.</summary>
    Public Sub EcrireTexte(ByVal x As Double, ByVal y As Double, ByVal texte As String,
                           Optional ByVal police As PoliceStandard = PoliceStandard.Helvetica,
                           Optional ByVal taille As Double = 12)
        _contenu.Append("BT").Append(vbLf)
        _contenu.Append("/").Append(PolicesPdf.Reference(police)).Append(" ").Append(Fmt(taille)).Append(" Tf").Append(vbLf)
        _contenu.Append("1 0 0 1 ").Append(Fmt(x)).Append(" ").Append(Fmt(Hauteur - y)).Append(" Tm").Append(vbLf)
        _contenu.Append("(").Append(Echapper(texte)).Append(") Tj").Append(vbLf)
        _contenu.Append("ET").Append(vbLf)
    End Sub

    ''' <summary>
    ''' Écrit un paragraphe avec retour à la ligne automatique dans
    ''' <paramref name="largeur"/> points (police Courier, chasse fixe).
    ''' Renvoie la position Y (depuis le haut) sous le dernier texte écrit.
    ''' </summary>
    Public Function EcrireParagraphe(ByVal x As Double, ByVal y As Double, ByVal largeur As Double, ByVal texte As String,
                                     Optional ByVal taille As Double = 11,
                                     Optional ByVal interligne As Double = 0) As Double
        If interligne <= 0 Then interligne = taille * 1.35
        Dim yCourant As Double = y
        For Each ligne As String In EnrouleurTexte.Enrouler(texte, largeur, taille)
            EcrireTexte(x, yCourant, ligne, PoliceStandard.Courier, taille)
            yCourant += interligne
        Next
        Return yCourant
    End Function

    ''' <summary>Trace un segment de droite.</summary>
    Public Sub TracerLigne(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double,
                           Optional ByVal epaisseur As Double = 1)
        _contenu.Append(Fmt(epaisseur)).Append(" w").Append(vbLf)
        _contenu.Append(Fmt(x1)).Append(" ").Append(Fmt(Hauteur - y1)).Append(" m").Append(vbLf)
        _contenu.Append(Fmt(x2)).Append(" ").Append(Fmt(Hauteur - y2)).Append(" l").Append(vbLf)
        _contenu.Append("S").Append(vbLf)
    End Sub

    ''' <summary>Trace un rectangle (origine haut-gauche), rempli ou seulement contouré.</summary>
    Public Sub TracerRectangle(ByVal x As Double, ByVal y As Double, ByVal largeur As Double, ByVal hauteur As Double,
                               Optional ByVal rempli As Boolean = False, Optional ByVal epaisseur As Double = 1)
        Dim basY As Double = Me.Hauteur - (y + hauteur)
        _contenu.Append(Fmt(epaisseur)).Append(" w").Append(vbLf)
        _contenu.Append(Fmt(x)).Append(" ").Append(Fmt(basY)).Append(" ").
                 Append(Fmt(largeur)).Append(" ").Append(Fmt(hauteur)).Append(" re").Append(vbLf)
        _contenu.Append(If(rempli, "f", "S")).Append(vbLf)
    End Sub

    ''' <summary>Flux de contenu accumulé (usage interne par l'assembleur).</summary>
    Friend ReadOnly Property FluxContenu As String
        Get
            Return _contenu.ToString()
        End Get
    End Property

    Private Shared Function Fmt(ByVal valeur As Double) As String
        Return valeur.ToString("0.###", CultureInfo.InvariantCulture)
    End Function

    ''' <summary>Échappe les caractères spéciaux d'une chaîne littérale PDF.</summary>
    Private Shared Function Echapper(ByVal texte As String) As String
        If texte Is Nothing Then Return ""
        Dim sb As New StringBuilder(texte.Length + 8)
        For Each c As Char In texte
            Select Case c
                Case "\"c : sb.Append("\\")
                Case "("c : sb.Append("\(")
                Case ")"c : sb.Append("\)")
                Case ChrW(13) : sb.Append("\r")
                Case ChrW(10) : sb.Append("\n")
                Case ChrW(9) : sb.Append("\t")
                Case Else : sb.Append(c)
            End Select
        Next
        Return sb.ToString()
    End Function

End Class

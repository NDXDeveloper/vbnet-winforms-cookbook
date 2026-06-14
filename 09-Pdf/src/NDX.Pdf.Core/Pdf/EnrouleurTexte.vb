' ============================================================================
'  EnrouleurTexte.vb  -  Retour a la ligne (word-wrap) pour police a chasse fixe.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic

''' <summary>
''' Découpe un texte en lignes tenant dans une largeur donnée, pour une police à
''' chasse fixe (Courier). La largeur d'un caractère Courier vaut exactement
''' 600/1000 de la taille de police, ce qui rend le calcul <b>exact</b> — donc
''' testable sans rien dessiner.
''' </summary>
Public NotInheritable Class EnrouleurTexte

    Private Sub New()
    End Sub

    ''' <summary>Largeur d'un caractère Courier (en points) pour une taille donnée.</summary>
    Public Const FacteurChasse As Double = 0.6

    ''' <summary>Nombre maximal de caractères Courier tenant dans une largeur (en points).</summary>
    Public Shared Function CaracteresParLigne(ByVal largeurPts As Double, ByVal taillePolice As Double) As Integer
        If taillePolice <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(taillePolice))
        Dim parChar As Double = FacteurChasse * taillePolice
        Return Math.Max(1, CInt(Math.Floor(largeurPts / parChar)))
    End Function

    ''' <summary>
    ''' Découpe <paramref name="texte"/> en lignes d'au plus <paramref name="largeurPts"/>
    ''' points de large (police Courier de <paramref name="taillePolice"/>). Respecte
    ''' les sauts de ligne existants et coupe les mots trop longs.
    ''' </summary>
    Public Shared Function Enrouler(ByVal texte As String, ByVal largeurPts As Double, ByVal taillePolice As Double) As List(Of String)
        Dim lignes As New List(Of String)()
        If String.IsNullOrEmpty(texte) Then Return lignes
        Dim maxi As Integer = CaracteresParLigne(largeurPts, taillePolice)

        ' On traite chaque paragraphe (separe par un saut de ligne) independamment.
        Dim paragraphes As String() = texte.Replace(vbCrLf, vbLf).Split(vbLf(0))
        For Each paragraphe As String In paragraphes
            EnroulerParagraphe(paragraphe, maxi, lignes)
        Next
        Return lignes
    End Function

    Private Shared Sub EnroulerParagraphe(ByVal paragraphe As String, ByVal maxi As Integer, ByVal lignes As List(Of String))
        If paragraphe.Length = 0 Then
            lignes.Add("")
            Return
        End If
        Dim mots As String() = paragraphe.Split(" "c)
        Dim courante As String = ""
        For Each mot As String In mots
            ' Mot plus long que la largeur : on le coupe en morceaux.
            While mot.Length > maxi
                If courante.Length > 0 Then
                    lignes.Add(courante)
                    courante = ""
                End If
                lignes.Add(mot.Substring(0, maxi))
                mot = mot.Substring(maxi)
            End While

            Dim candidate As String = If(courante.Length = 0, mot, courante & " " & mot)
            If candidate.Length > maxi Then
                lignes.Add(courante)
                courante = mot
            Else
                courante = candidate
            End If
        Next
        lignes.Add(courante)
    End Sub

End Class

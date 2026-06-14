' ============================================================================
'  EchelleGraphique.vb  -  Mise a l'echelle (pure) valeur -> pixel.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' Convertit une valeur de données en ordonnée pixel dans une zone de tracé de
''' hauteur donnée (le bas de l'échelle est en bas du graphique). Le calcul est
''' <b>pur</b> : aucune dépendance au dessin, donc testable directement.
''' </summary>
Public NotInheritable Class EchelleGraphique

    Public Sub New(ByVal bas As Double, ByVal haut As Double, ByVal hauteurPixels As Integer)
        Me.Bas = bas
        Me.Haut = haut
        Me.HauteurPixels = hauteurPixels
    End Sub

    ''' <summary>Valeur correspondant au bas de la zone de tracé.</summary>
    Public ReadOnly Property Bas As Double
    ''' <summary>Valeur correspondant au haut de la zone de tracé.</summary>
    Public ReadOnly Property Haut As Double
    ''' <summary>Hauteur de la zone de tracé, en pixels.</summary>
    Public ReadOnly Property HauteurPixels As Integer

    ''' <summary>Ordonnée pixel (0 = haut) pour une valeur donnée ; bornée à la zone.</summary>
    Public Function VersY(ByVal valeur As Double) As Double
        Dim etendue As Double = Haut - Bas
        If etendue <= 0 Then Return HauteurPixels   ' échelle plate : tout en bas
        Dim fraction As Double = (valeur - Bas) / etendue
        fraction = Math.Max(0.0, Math.Min(1.0, fraction))
        Return HauteurPixels * (1.0 - fraction)
    End Function

    ''' <summary>
    ''' Construit une échelle automatique à partir de valeurs : le bas vaut min(0, plus
    ''' petite valeur) — pratique pour des barres — et le haut, la plus grande valeur.
    ''' </summary>
    Public Shared Function Auto(ByVal valeurs As IEnumerable(Of Double), ByVal hauteurPixels As Integer) As EchelleGraphique
        Dim liste As List(Of Double) = If(valeurs, Enumerable.Empty(Of Double)()).ToList()
        If liste.Count = 0 Then Return New EchelleGraphique(0, 1, hauteurPixels)
        Dim bas As Double = Math.Min(0.0, liste.Min())
        Dim haut As Double = liste.Max()
        If haut <= bas Then haut = bas + 1.0
        Return New EchelleGraphique(bas, haut, hauteurPixels)
    End Function

End Class

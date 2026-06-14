' ============================================================================
'  Scene.vb  -  Collection de formes (ajout, selection par point, dessin).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing

''' <summary>
''' Ensemble ordonné de formes (la dernière est « au-dessus »). Le test de
''' sélection renvoie la forme la plus haute sous un point — logique pure,
''' testable sans interface.
''' </summary>
Public NotInheritable Class Scene

    Private ReadOnly _formes As New List(Of Forme)()

    Public ReadOnly Property Formes As IReadOnlyList(Of Forme)
        Get
            Return _formes
        End Get
    End Property

    Public Sub Ajouter(ByVal forme As Forme)
        If forme IsNot Nothing Then _formes.Add(forme)
    End Sub

    Public Sub Retirer(ByVal forme As Forme)
        _formes.Remove(forme)
    End Sub

    Public Sub Vider()
        _formes.Clear()
    End Sub

    ''' <summary>Forme la plus haute contenant le point (ou <c>Nothing</c>).</summary>
    Public Function TrouverA(ByVal px As Double, ByVal py As Double) As Forme
        For i As Integer = _formes.Count - 1 To 0 Step -1
            If _formes(i).Contient(px, py) Then Return _formes(i)
        Next
        Return Nothing
    End Function

    ''' <summary>Dessine toutes les formes (de la plus basse à la plus haute).</summary>
    Public Sub DessinerSur(ByVal g As Graphics)
        For Each forme As Forme In _formes
            forme.DessinerSur(g)
        Next
    End Sub

End Class

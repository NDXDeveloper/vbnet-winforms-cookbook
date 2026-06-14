' ============================================================================
'  Paginateur.vb  -  Decoupage (pur) d'un texte en pages.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' Arithmétique de la pagination, isolée du rendu d'impression pour être testable
''' sans imprimante : combien de lignes tiennent par page, combien de pages, et
''' quelles lignes pour une page donnée.
''' </summary>
Public NotInheritable Class Paginateur

    Private Sub New()
    End Sub

    ''' <summary>Nombre de lignes tenant dans une hauteur disponible.</summary>
    Public Shared Function LignesParPage(ByVal hauteurDisponible As Double, ByVal hauteurLigne As Double) As Integer
        If hauteurLigne <= 0 Then Return 1
        Return Math.Max(1, CInt(Math.Floor(hauteurDisponible / hauteurLigne)))
    End Function

    ''' <summary>Nombre total de pages.</summary>
    Public Shared Function NombrePages(ByVal nbLignes As Integer, ByVal lignesParPage As Integer) As Integer
        If lignesParPage <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(lignesParPage))
        If nbLignes <= 0 Then Return 0
        Return CInt(Math.Ceiling(nbLignes / CDbl(lignesParPage)))
    End Function

    ''' <summary>Lignes de la page <paramref name="page"/> (1-based) ; liste vide hors limites.</summary>
    Public Shared Function LignesDeLaPage(ByVal lignes As IList(Of String), ByVal page As Integer, ByVal lignesParPage As Integer) As List(Of String)
        If lignes Is Nothing OrElse page < 1 OrElse lignesParPage <= 0 Then Return New List(Of String)()
        Dim debut As Integer = (page - 1) * lignesParPage
        If debut >= lignes.Count Then Return New List(Of String)()
        Return lignes.Skip(debut).Take(lignesParPage).ToList()
    End Function

End Class

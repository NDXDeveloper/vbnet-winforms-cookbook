' ============================================================================
'  ConstructeurArbre.vb  -  Reconstruction (pure) d'un arbre depuis une liste plate.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' Reconstruit un arbre (racines + enfants) à partir d'une liste plate de nœuds
''' reliés par <c>ParentId</c>. Algorithme en deux passes via un dictionnaire :
''' O(n), déterministe. Logique pure, testable sans base ni interface.
''' </summary>
Public NotInheritable Class ConstructeurArbre

    Private Sub New()
    End Sub

    ''' <summary>Construit la forêt (liste des racines) à partir des nœuds plats.</summary>
    Public Shared Function Construire(ByVal plats As IEnumerable(Of Noeud)) As List(Of NoeudArbre)
        Dim liste As List(Of Noeud) = If(plats, Enumerable.Empty(Of Noeud)()).ToList()

        ' 1re passe : un NoeudArbre par identifiant.
        Dim parId As New Dictionary(Of Integer, NoeudArbre)()
        For Each n As Noeud In liste
            parId(n.Id) = New NoeudArbre(n)
        Next

        ' 2e passe : rattacher chaque nœud à son parent (ou en faire une racine).
        Dim racines As New List(Of NoeudArbre)()
        For Each n As Noeud In liste
            Dim courant As NoeudArbre = parId(n.Id)
            If n.ParentId.HasValue AndAlso parId.ContainsKey(n.ParentId.Value) Then
                parId(n.ParentId.Value).Enfants.Add(courant)
            Else
                racines.Add(courant)   ' racine, ou orphelin (parent absent)
            End If
        Next
        Return racines
    End Function

End Class

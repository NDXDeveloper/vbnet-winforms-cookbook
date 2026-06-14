' ============================================================================
'  NoeudArbre.vb  -  Noeud d'arbre reconstruit (avec ses enfants).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic

''' <summary>Nœud d'arbre « gonflé » : le nœud plat + la liste de ses enfants.</summary>
Public NotInheritable Class NoeudArbre

    Public Sub New(ByVal noeud As Noeud)
        Me.Noeud = noeud
    End Sub

    Public ReadOnly Property Noeud As Noeud
    Public ReadOnly Property Enfants As New List(Of NoeudArbre)()

    Public ReadOnly Property Libelle As String
        Get
            Return Noeud.Libelle
        End Get
    End Property

    ''' <summary>Nombre total de descendants (récursif).</summary>
    Public ReadOnly Property NombreDescendants As Integer
        Get
            Dim total As Integer = Enfants.Count
            For Each enfant As NoeudArbre In Enfants
                total += enfant.NombreDescendants
            Next
            Return total
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Libelle
    End Function

End Class

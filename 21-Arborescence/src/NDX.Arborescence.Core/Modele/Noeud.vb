' ============================================================================
'  Noeud.vb  -  Noeud d'une arborescence (liste d'adjacence : parent_id).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>
''' Un nœud d'arbre stocké « à plat » : il connaît son parent par
''' <see cref="ParentId"/> (liste d'adjacence). <c>Nothing</c> = racine.
''' </summary>
Public NotInheritable Class Noeud

    Public Property Id As Integer
    Public Property ParentId As Integer?
    Public Property Libelle As String
    Public Property Categorie As String

    Public Overrides Function ToString() As String
        Return Libelle
    End Function

End Class

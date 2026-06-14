' ============================================================================
'  Lien.vb  -  Entree du catalogue de liens / raccourcis.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Une entrée du catalogue : un raccourci d'application (.lnk) ou un lien Web (.url).</summary>
Public NotInheritable Class Lien

    ''' <summary>Catégorie : <c>application</c> (.lnk) ou <c>web</c> (.url).</summary>
    Public Property Categorie As String

    ''' <summary>Nom affiché (et nom de fichier du raccourci).</summary>
    Public Property Nom As String

    ''' <summary>Cible : chemin de l'exécutable (.lnk) ou URL (.url).</summary>
    Public Property Cible As String

    ''' <summary>Description facultative.</summary>
    Public Property Description As String

    Public Overrides Function ToString() As String
        Return Nom & " (" & Categorie & ") → " & Cible
    End Function

End Class

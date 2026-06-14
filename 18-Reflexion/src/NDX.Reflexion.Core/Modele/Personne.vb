' ============================================================================
'  Personne.vb  -  Type d'exemple a inspecter / copier.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Type de démonstration : sert de cible à l'inspection et à la copie.</summary>
Public NotInheritable Class Personne

    Public Property Nom As String
    Public Property Prenom As String
    Public Property Age As Integer
    Public Property Courriel As String

    Public Overrides Function ToString() As String
        Return (Prenom & " " & Nom).Trim()
    End Function

End Class

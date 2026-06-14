' ============================================================================
'  Tache.vb  -  Une tache de la file d'attente.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Une tâche à traiter (élément de la file d'attente persistée).</summary>
Public NotInheritable Class Tache

    Public Property Id As Integer
    Public Property Libelle As String
    Public Property Charge As String
    Public Property Etat As String
    Public Property CreeLe As DateTime

    Public Overrides Function ToString() As String
        Return "#" & Id.ToString() & " " & Libelle & " [" & Etat & "]"
    End Function

End Class

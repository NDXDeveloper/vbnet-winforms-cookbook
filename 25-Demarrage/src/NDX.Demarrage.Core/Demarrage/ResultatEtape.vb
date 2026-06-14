' ============================================================================
'  ResultatEtape.vb  -  Resultat d'une etape de demarrage.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Résultat de l'exécution d'une étape de démarrage.</summary>
Public NotInheritable Class ResultatEtape

    Public Sub New(ByVal nom As String, ByVal reussi As Boolean, ByVal message As String)
        Me.Nom = nom
        Me.Reussi = reussi
        Me.Message = message
    End Sub

    Public ReadOnly Property Nom As String
    Public ReadOnly Property Reussi As Boolean
    Public ReadOnly Property Message As String

    Public Overrides Function ToString() As String
        Return (If(Reussi, "OK  ", "ÉCHEC ")) & Nom & " — " & Message
    End Function

End Class

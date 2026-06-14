' ============================================================================
'  Article.vb  -  Type d'exemple a inspecter / copier.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Type de démonstration : sert de cible à l'inspection et à la copie.</summary>
Public NotInheritable Class Article

    Public Property Reference As String
    Public Property Designation As String
    Public Property PrixHT As Decimal
    Public Property EnStock As Boolean
    Public Property CreeLe As DateTime

    ''' <summary>Événement de démonstration (utilisé pour la purge d'abonnés).</summary>
    Public Event PrixModifie As EventHandler

    Public Sub DeclencherPrixModifie()
        RaiseEvent PrixModifie(Me, EventArgs.Empty)
    End Sub

End Class

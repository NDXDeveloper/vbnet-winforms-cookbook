' ============================================================================
'  EtapeDemarrage.vb  -  Une etape de demarrage (nom + verification).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>
''' Une étape de la séquence de démarrage : un nom et une vérification qui renvoie
''' <c>True</c> en cas de succès (ou lève une exception en cas d'erreur).
''' </summary>
Public NotInheritable Class EtapeDemarrage

    Public Sub New(ByVal nom As String, ByVal verification As Func(Of Boolean))
        If verification Is Nothing Then Throw New ArgumentNullException(NameOf(verification))
        Me.Nom = nom
        Me.Verification = verification
    End Sub

    Public ReadOnly Property Nom As String
    Public ReadOnly Property Verification As Func(Of Boolean)

End Class

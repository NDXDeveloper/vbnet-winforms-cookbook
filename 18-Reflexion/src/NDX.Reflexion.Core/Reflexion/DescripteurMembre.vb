' ============================================================================
'  DescripteurMembre.vb  -  Description d'un membre decouvert par reflexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Description simple d'un membre de type (nom, genre, type associé).</summary>
Public NotInheritable Class DescripteurMembre

    Public Sub New(ByVal nom As String, ByVal genre As String, ByVal typeAssocie As String)
        Me.Nom = nom
        Me.Genre = genre
        Me.TypeAssocie = typeAssocie
    End Sub

    ''' <summary>Nom du membre.</summary>
    Public ReadOnly Property Nom As String
    ''' <summary>Genre : « Propriété », « Champ » ou « Événement ».</summary>
    Public ReadOnly Property Genre As String
    ''' <summary>Type associé (type de la propriété/champ, ou du délégué d'événement).</summary>
    Public ReadOnly Property TypeAssocie As String

    Public Overrides Function ToString() As String
        Return Genre & " : " & Nom & " (" & TypeAssocie & ")"
    End Function

End Class

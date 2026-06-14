' ============================================================================
'  Raccourci.vb  -  Un raccourci = une suite de combinaisons (accord / chord).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' Un raccourci, éventuellement à plusieurs frappes (« accord »), comme
''' <c>Ctrl+K, Ctrl+S</c>. Les combinaisons sont séparées par une virgule ou une
''' espace.
''' </summary>
Public NotInheritable Class Raccourci

    Private ReadOnly _combinaisons As List(Of Combinaison)

    Public Sub New(ByVal combinaisons As IEnumerable(Of Combinaison))
        _combinaisons = New List(Of Combinaison)(combinaisons)
        If _combinaisons.Count = 0 Then Throw New ArgumentException("Un raccourci comporte au moins une combinaison.")
    End Sub

    ''' <summary>Combinaisons composant l'accord (au moins une).</summary>
    Public ReadOnly Property Combinaisons As IReadOnlyList(Of Combinaison)
        Get
            Return _combinaisons
        End Get
    End Property

    ''' <summary>Nombre de frappes de l'accord.</summary>
    Public ReadOnly Property NombreFrappes As Integer
        Get
            Return _combinaisons.Count
        End Get
    End Property

    Public Shared Function Analyser(ByVal texte As String) As Raccourci
        Dim r As Raccourci = Nothing
        If Not TryAnalyser(texte, r) Then Throw New FormatException("Raccourci invalide : « " & If(texte, "") & " ».")
        Return r
    End Function

    Public Shared Function TryAnalyser(ByVal texte As String, ByRef resultat As Raccourci) As Boolean
        resultat = Nothing
        If String.IsNullOrWhiteSpace(texte) Then Return False
        Dim jetons As String() = texte.Split(New Char() {","c, " "c, ";"c}, StringSplitOptions.RemoveEmptyEntries)
        If jetons.Length = 0 Then Return False
        Dim combos As New List(Of Combinaison)()
        For Each jeton As String In jetons
            Dim c As Combinaison
            If Not Combinaison.TryAnalyser(jeton, c) Then Return False
            combos.Add(c)
        Next
        resultat = New Raccourci(combos)
        Return True
    End Function

    ''' <summary>Forme canonique : « Ctrl+K, Ctrl+S ».</summary>
    Public ReadOnly Property Texte As String
        Get
            Return String.Join(", ", _combinaisons.Select(Function(c) c.ToString()))
        End Get
    End Property

    ''' <summary>Vrai si cet accord est un préfixe (éventuellement strict) de <paramref name="autre"/>.</summary>
    Public Function EstPrefixeDe(ByVal autre As Raccourci) As Boolean
        If autre Is Nothing OrElse _combinaisons.Count > autre._combinaisons.Count Then Return False
        For i As Integer = 0 To _combinaisons.Count - 1
            If Not _combinaisons(i).Equals(autre._combinaisons(i)) Then Return False
        Next
        Return True
    End Function

    Public Overrides Function ToString() As String
        Return Texte
    End Function

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim autre = TryCast(obj, Raccourci)
        Return autre IsNot Nothing AndAlso _combinaisons.SequenceEqual(autre._combinaisons)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Dim h As Integer = 17
        For Each c As Combinaison In _combinaisons
            h = h * 31 Xor c.GetHashCode()
        Next
        Return h
    End Function

End Class

' ============================================================================
'  VersionSemantique.vb  -  Version "Majeure.Mineure.Corrective" comparable.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Globalization

''' <summary>
''' Numero de version sémantique <c>Majeure.Mineure.Corrective</c> (ex. 1.4.2),
''' comparable et ordonnable. Illustre <see cref="IComparable(Of T)"/>, la
''' surcharge d'opérateurs et le motif <c>TryAnalyser</c>.
''' </summary>
Public Structure VersionSemantique
    Implements IComparable(Of VersionSemantique), IEquatable(Of VersionSemantique)

    Public ReadOnly Property Majeure As Integer
    Public ReadOnly Property Mineure As Integer
    Public ReadOnly Property Corrective As Integer

    Public Sub New(ByVal majeure As Integer, ByVal mineure As Integer, ByVal corrective As Integer)
        If majeure < 0 OrElse mineure < 0 OrElse corrective < 0 Then
            Throw New ArgumentException("Les composantes de version ne peuvent pas être négatives.")
        End If
        Me.Majeure = majeure
        Me.Mineure = mineure
        Me.Corrective = corrective
    End Sub

    ''' <summary>Analyse "1", "1.2" ou "1.2.3" (un "v" initial est toléré). Lève si invalide.</summary>
    Public Shared Function Analyser(ByVal texte As String) As VersionSemantique
        Dim resultat As VersionSemantique
        If Not TryAnalyser(texte, resultat) Then
            Throw New FormatException("Version invalide : « " & If(texte, "") & " ».")
        End If
        Return resultat
    End Function

    ''' <summary>Variante sans exception : renvoie False si le texte est invalide.</summary>
    Public Shared Function TryAnalyser(ByVal texte As String, ByRef resultat As VersionSemantique) As Boolean
        resultat = New VersionSemantique()
        If String.IsNullOrWhiteSpace(texte) Then Return False
        Dim net As String = texte.Trim()
        If net.StartsWith("v", StringComparison.OrdinalIgnoreCase) Then net = net.Substring(1)
        Dim parties As String() = net.Split("."c)
        If parties.Length < 1 OrElse parties.Length > 3 Then Return False
        Dim composantes As Integer() = {0, 0, 0}
        For i As Integer = 0 To parties.Length - 1
            Dim v As Integer
            If Not Integer.TryParse(parties(i), NumberStyles.None, CultureInfo.InvariantCulture, v) Then Return False
            composantes(i) = v
        Next
        resultat = New VersionSemantique(composantes(0), composantes(1), composantes(2))
        Return True
    End Function

    Public Function CompareTo(ByVal autre As VersionSemantique) As Integer Implements IComparable(Of VersionSemantique).CompareTo
        If Majeure <> autre.Majeure Then Return Majeure.CompareTo(autre.Majeure)
        If Mineure <> autre.Mineure Then Return Mineure.CompareTo(autre.Mineure)
        Return Corrective.CompareTo(autre.Corrective)
    End Function

    Public Overloads Function Equals(ByVal autre As VersionSemantique) As Boolean Implements IEquatable(Of VersionSemantique).Equals
        Return CompareTo(autre) = 0
    End Function

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return TypeOf obj Is VersionSemantique AndAlso Equals(DirectCast(obj, VersionSemantique))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return (Majeure * 73856093) Xor (Mineure * 19349663) Xor (Corrective * 83492791)
    End Function

    Public Overrides Function ToString() As String
        Return String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", Majeure, Mineure, Corrective)
    End Function

    Public Shared Operator =(ByVal a As VersionSemantique, ByVal b As VersionSemantique) As Boolean
        Return a.CompareTo(b) = 0
    End Operator

    Public Shared Operator <>(ByVal a As VersionSemantique, ByVal b As VersionSemantique) As Boolean
        Return a.CompareTo(b) <> 0
    End Operator

    Public Shared Operator >(ByVal a As VersionSemantique, ByVal b As VersionSemantique) As Boolean
        Return a.CompareTo(b) > 0
    End Operator

    Public Shared Operator <(ByVal a As VersionSemantique, ByVal b As VersionSemantique) As Boolean
        Return a.CompareTo(b) < 0
    End Operator

    Public Shared Operator >=(ByVal a As VersionSemantique, ByVal b As VersionSemantique) As Boolean
        Return a.CompareTo(b) >= 0
    End Operator

    Public Shared Operator <=(ByVal a As VersionSemantique, ByVal b As VersionSemantique) As Boolean
        Return a.CompareTo(b) <= 0
    End Operator

End Structure

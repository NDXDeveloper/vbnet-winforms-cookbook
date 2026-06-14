' ============================================================================
'  Combinaison.vb  -  Une combinaison de touches (modificateurs + touche).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Text
Imports System.Windows.Forms

''' <summary>
''' Une combinaison de touches du type <c>Ctrl+Maj+P</c> : une touche principale
''' éventuellement accompagnée des modificateurs Ctrl, Alt et Maj. S'appuie sur
''' l'énumération <see cref="Keys"/> (valeur = touche + indicateurs de modificateurs),
''' ce qui correspond exactement à <c>KeyEventArgs.KeyData</c>.
''' </summary>
Public Structure Combinaison
    Implements IEquatable(Of Combinaison)

    ''' <summary>Valeur brute (touche + modificateurs), comme <c>KeyData</c>.</summary>
    Public ReadOnly Property Valeur As Keys

    Public Sub New(ByVal valeur As Keys)
        Me.Valeur = valeur
    End Sub

    ''' <summary>Touche principale, sans les modificateurs.</summary>
    Public ReadOnly Property Touche As Keys
        Get
            Return Valeur And Keys.KeyCode
        End Get
    End Property

    Public ReadOnly Property AvecControle As Boolean
        Get
            Return (Valeur And Keys.Control) = Keys.Control
        End Get
    End Property

    Public ReadOnly Property AvecAlt As Boolean
        Get
            Return (Valeur And Keys.Alt) = Keys.Alt
        End Get
    End Property

    Public ReadOnly Property AvecMaj As Boolean
        Get
            Return (Valeur And Keys.Shift) = Keys.Shift
        End Get
    End Property

    ''' <summary>Analyse « Ctrl+Maj+P » ; lève <see cref="FormatException"/> si invalide.</summary>
    Public Shared Function Analyser(ByVal texte As String) As Combinaison
        Dim c As Combinaison
        If Not TryAnalyser(texte, c) Then Throw New FormatException("Combinaison invalide : « " & If(texte, "") & " ».")
        Return c
    End Function

    ''' <summary>Variante sans exception.</summary>
    Public Shared Function TryAnalyser(ByVal texte As String, ByRef resultat As Combinaison) As Boolean
        resultat = New Combinaison(Keys.None)
        If String.IsNullOrWhiteSpace(texte) Then Return False
        Dim modificateurs As Keys = Keys.None
        Dim touche As Keys = Keys.None
        Dim aTouche As Boolean = False
        For Each brut As String In texte.Split("+"c)
            Dim jeton As String = brut.Trim()
            If jeton.Length = 0 Then Return False   ' « Ctrl+ » ou « Ctrl++ »
            Select Case jeton.ToLowerInvariant()
                Case "ctrl", "control", "ctl" : modificateurs = modificateurs Or Keys.Control
                Case "alt" : modificateurs = modificateurs Or Keys.Alt
                Case "maj", "shift" : modificateurs = modificateurs Or Keys.Shift
                Case Else
                    If aTouche Then Return False     ' deux touches principales
                    Dim k As Keys = ConvertirTouche(jeton)
                    If k = Keys.None Then Return False
                    touche = k
                    aTouche = True
            End Select
        Next
        If Not aTouche Then Return False             ' uniquement des modificateurs
        resultat = New Combinaison(touche Or modificateurs)
        Return True
    End Function

    Private Shared Function ConvertirTouche(ByVal jeton As String) As Keys
        ' Alias français / courants.
        Select Case jeton.ToLowerInvariant()
            Case "entree", "entrée", "enter", "retour" : Return Keys.Enter
            Case "espace", "space" : Return Keys.Space
            Case "echap", "échap", "esc", "escape" : Return Keys.Escape
            Case "tab", "tabulation" : Return Keys.Tab
            Case "suppr", "delete", "del" : Return Keys.Delete
            Case "inser", "inser.", "insert" : Return Keys.Insert
            Case "haut", "up" : Return Keys.Up
            Case "bas", "down" : Return Keys.Down
            Case "gauche", "left" : Return Keys.Left
            Case "droite", "right" : Return Keys.Right
        End Select
        If jeton.Length = 1 Then
            Dim c As Char = Char.ToUpperInvariant(jeton(0))
            If c >= "A"c AndAlso c <= "Z"c Then Return CType(Keys.A + (Asc(c) - Asc("A"c)), Keys)
            If c >= "0"c AndAlso c <= "9"c Then Return CType(Keys.D0 + (Asc(c) - Asc("0"c)), Keys)
        End If
        Dim k As Keys
        If [Enum].TryParse(Of Keys)(jeton, True, k) AndAlso k <> Keys.None Then Return k
        Return Keys.None
    End Function

    ''' <summary>Forme canonique : « Ctrl+Alt+Maj+Touche » (ordre fixe des modificateurs).</summary>
    Public Overrides Function ToString() As String
        Dim sb As New StringBuilder()
        If AvecControle Then sb.Append("Ctrl+")
        If AvecAlt Then sb.Append("Alt+")
        If AvecMaj Then sb.Append("Maj+")
        sb.Append(NomTouche(Touche))
        Return sb.ToString()
    End Function

    Private Shared Function NomTouche(ByVal touche As Keys) As String
        If touche >= Keys.A AndAlso touche <= Keys.Z Then Return Chr(Asc("A"c) + (touche - Keys.A)).ToString()
        If touche >= Keys.D0 AndAlso touche <= Keys.D9 Then Return Chr(Asc("0"c) + (touche - Keys.D0)).ToString()
        Return touche.ToString()
    End Function

    Public Overloads Function Equals(ByVal autre As Combinaison) As Boolean Implements IEquatable(Of Combinaison).Equals
        Return Valeur = autre.Valeur
    End Function

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return TypeOf obj Is Combinaison AndAlso Equals(DirectCast(obj, Combinaison))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return CInt(Valeur)
    End Function

End Structure

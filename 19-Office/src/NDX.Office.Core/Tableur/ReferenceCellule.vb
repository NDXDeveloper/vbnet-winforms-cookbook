' ============================================================================
'  ReferenceCellule.vb  -  Conversion (pure) entre reference A1 et (colonne, ligne).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Text

''' <summary>
''' Référence d'une cellule de tableur au format « A1 » (ex. <c>B3</c>, <c>AA10</c>).
''' Convertit la lettre de colonne en index et inversement (base 26 bijective).
''' Logique pure, testable sans fichier.
''' </summary>
Public Structure ReferenceCellule

    ''' <summary>Index de colonne (1 = A).</summary>
    Public ReadOnly Property Colonne As Integer
    ''' <summary>Numéro de ligne (1-based).</summary>
    Public ReadOnly Property Ligne As Integer

    Public Sub New(ByVal colonne As Integer, ByVal ligne As Integer)
        If colonne < 1 OrElse ligne < 1 Then Throw New ArgumentOutOfRangeException("Colonne et ligne commencent à 1.")
        Me.Colonne = colonne
        Me.Ligne = ligne
    End Sub

    ''' <summary>Lettres de colonne pour un index (1 -> A, 26 -> Z, 27 -> AA).</summary>
    Public Shared Function ColonneVersLettres(ByVal index As Integer) As String
        If index < 1 Then Throw New ArgumentOutOfRangeException(NameOf(index))
        Dim sb As New StringBuilder()
        Dim n As Integer = index
        While n > 0
            Dim reste As Integer = (n - 1) Mod 26
            sb.Insert(0, Chr(Asc("A"c) + reste))
            n = (n - 1) \ 26
        End While
        Return sb.ToString()
    End Function

    ''' <summary>Index de colonne pour des lettres (A -> 1, AA -> 27).</summary>
    Public Shared Function LettresVersColonne(ByVal lettres As String) As Integer
        If String.IsNullOrEmpty(lettres) Then Throw New ArgumentException("Lettres de colonne vides.")
        Dim valeur As Integer = 0
        For Each c As Char In lettres.ToUpperInvariant()
            If c < "A"c OrElse c > "Z"c Then Throw New FormatException("Lettre de colonne invalide : " & c)
            valeur = valeur * 26 + (Asc(c) - Asc("A"c) + 1)
        Next
        Return valeur
    End Function

    ''' <summary>Analyse une référence « A1 ».</summary>
    Public Shared Function Analyser(ByVal refA1 As String) As ReferenceCellule
        If String.IsNullOrWhiteSpace(refA1) Then Throw New FormatException("Référence vide.")
        Dim texte As String = refA1.Trim().ToUpperInvariant()
        Dim i As Integer = 0
        While i < texte.Length AndAlso texte(i) >= "A"c AndAlso texte(i) <= "Z"c
            i += 1
        End While
        If i = 0 OrElse i = texte.Length Then Throw New FormatException("Référence invalide : " & refA1)
        Dim lettres As String = texte.Substring(0, i)
        Dim ligne As Integer
        If Not Integer.TryParse(texte.Substring(i), ligne) OrElse ligne < 1 Then Throw New FormatException("Référence invalide : " & refA1)
        Return New ReferenceCellule(LettresVersColonne(lettres), ligne)
    End Function

    ''' <summary>Référence au format A1 (ex. « B3 »).</summary>
    Public ReadOnly Property A1 As String
        Get
            Return ColonneVersLettres(Colonne) & Ligne.ToString()
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return A1
    End Function

End Structure

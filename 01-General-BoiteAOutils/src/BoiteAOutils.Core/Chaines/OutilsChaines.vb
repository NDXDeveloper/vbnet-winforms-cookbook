Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' Outils de traitement des chaines de caracteres.
''' </summary>
''' <remarks>
''' Nettoyage et normalisation de chaines : suppression d'accents, retrait des
''' caracteres speciaux, validation de filtres et detection de GUID.
''' </remarks>
Public Module OutilsChaines

    ' Expression compilee reutilisable : ne conserve que lettres, chiffres,
    ' soulignes et points.
    Private ReadOnly _regexCaracteresAutorises As New Regex("[^a-zA-Z0-9_.]+", RegexOptions.Compiled)

    ' Motif d'un GUID canonique (8-4-4-4-12 chiffres hexadecimaux).
    Private ReadOnly _regexGuid As New Regex(
        "\b[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}\b",
        RegexOptions.Compiled)

    ''' <summary>
    ''' Supprime tous les caracteres autres que les lettres, chiffres, "_" et ".".
    ''' </summary>
    Public Function RetirerCaracteresSpeciaux(ByVal texte As String) As String
        If texte Is Nothing Then Return String.Empty
        Return _regexCaracteresAutorises.Replace(texte, "")
    End Function

    ''' <summary>
    ''' Nettoie une chaine destinee a servir de filtre, en retirant d'abord les
    ''' caracteres interdits dans un nom de fichier, puis les caracteres speciaux.
    ''' </summary>
    Public Function RendreFiltreValide(ByVal nom As String) As String
        If nom Is Nothing Then Return String.Empty
        Dim caracteresInterdits As String = Regex.Escape(New String(IO.Path.GetInvalidFileNameChars()))
        Dim motif As String = String.Format("([{0}]*\.+$)|([{0}]+)", caracteresInterdits)
        Return RetirerCaracteresSpeciaux(Regex.Replace(nom, motif, ""))
    End Function

    ''' <summary>
    ''' Supprime les accents (diacritiques) d'une chaine et la convertit en
    ''' minuscules. S'appuie sur la normalisation Unicode (forme D), qui separe
    ''' les caracteres de base de leurs signes diacritiques.
    ''' </summary>
    Public Function RetirerAccents(ByVal texte As String) As String
        If texte Is Nothing Then Return String.Empty
        Dim chaineNormalisee As String = texte.Normalize(NormalizationForm.FormD)
        Dim sb As New StringBuilder(chaineNormalisee.Length)
        For Each c As Char In chaineNormalisee
            ' On rejette les "marques sans chasse" (accents, cedilles...).
            If CharUnicodeInfo.GetUnicodeCategory(c) <> UnicodeCategory.NonSpacingMark Then
                sb.Append(c)
            End If
        Next
        Return sb.ToString().ToLowerInvariant()
    End Function

    ''' <summary>Met la premiere lettre d'une chaine en majuscule.</summary>
    Public Function PremiereLettreEnMajuscule(ByVal texte As String) As String
        If String.IsNullOrEmpty(texte) Then Return String.Empty
        If texte.Length = 1 Then Return Char.ToUpper(texte(0)).ToString()
        Return Char.ToUpper(texte(0)) & texte.Substring(1)
    End Function

    ''' <summary>Indique si la chaine contient un GUID au format canonique.</summary>
    Public Function ContientGuid(ByVal texte As String) As Boolean
        If String.IsNullOrEmpty(texte) Then Return False
        Return _regexGuid.IsMatch(texte)
    End Function

End Module

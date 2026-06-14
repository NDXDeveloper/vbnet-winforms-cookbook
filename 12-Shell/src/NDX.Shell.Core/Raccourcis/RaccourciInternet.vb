' ============================================================================
'  RaccourciInternet.vb  -  Raccourci Web ".url" (fichier INI).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.IO
Imports System.Text

''' <summary>
''' Lit et écrit les raccourcis Internet « .url ». Ce sont de simples fichiers
''' INI : une section <c>[InternetShortcut]</c> et une clé <c>URL=</c>. La
''' génération et l'analyse sont du pur traitement de texte, donc testables sans
''' rien écrire sur le disque.
''' </summary>
Public NotInheritable Class RaccourciInternet

    Private Sub New()
    End Sub

    Private Const SECTION As String = "[InternetShortcut]"
    Private Shared ReadOnly _utf8 As New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)

    ''' <summary>Construit le contenu d'un fichier .url pour l'URL donnée.</summary>
    Public Shared Function GenererContenu(ByVal url As String) As String
        If String.IsNullOrWhiteSpace(url) Then Throw New ArgumentException("URL obligatoire.", NameOf(url))
        Dim sb As New StringBuilder()
        sb.Append(SECTION).Append(vbCrLf)
        sb.Append("URL=").Append(url.Trim()).Append(vbCrLf)
        Return sb.ToString()
    End Function

    ''' <summary>Extrait l'URL du contenu d'un fichier .url ; lève si elle est absente.</summary>
    Public Shared Function LireUrl(ByVal contenu As String) As String
        If contenu IsNot Nothing Then
            For Each ligne As String In contenu.Replace(vbCrLf, vbLf).Split(vbLf(0))
                Dim net As String = ligne.Trim()
                Dim p As Integer = net.IndexOf("="c)
                If p > 0 AndAlso net.Substring(0, p).Trim().Equals("URL", StringComparison.OrdinalIgnoreCase) Then
                    Dim valeur As String = net.Substring(p + 1).Trim()
                    If valeur.Length > 0 Then Return valeur
                End If
            Next
        End If
        Throw New FormatException("Aucune URL trouvée dans le raccourci Internet.")
    End Function

    ''' <summary>Écrit un fichier .url pointant vers <paramref name="url"/>.</summary>
    Public Shared Sub Ecrire(ByVal chemin As String, ByVal url As String)
        File.WriteAllText(chemin, GenererContenu(url), _utf8)
    End Sub

    ''' <summary>Lit l'URL d'un fichier .url existant.</summary>
    Public Shared Function LireFichier(ByVal chemin As String) As String
        Return LireUrl(File.ReadAllText(chemin, _utf8))
    End Function

End Class

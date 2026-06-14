' ============================================================================
'  ServiceMiseAJour.vb  -  Logique de detection et de controle des mises a jour.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text

''' <summary>
''' Décide s'il existe une mise à jour, repère les mises à jour obligatoires et
''' vérifie l'intégrité d'un paquet téléchargé (SHA-256). Logique pure : aucune
''' dépendance à la base ni au réseau, donc entièrement testable.
''' </summary>
Public NotInheritable Class ServiceMiseAJour

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Renvoie la publication la plus récente strictement supérieure à
    ''' <paramref name="versionActuelle"/>, ou <c>Nothing</c> si l'on est à jour.
    ''' </summary>
    Public Shared Function RechercherDerniere(ByVal versionActuelle As VersionSemantique,
                                              ByVal publications As IEnumerable(Of Publication)) As Publication
        If publications Is Nothing Then Return Nothing
        Return publications.
            Where(Function(p) p IsNot Nothing AndAlso p.Version > versionActuelle).
            OrderByDescending(Function(p) p.Version).
            FirstOrDefault()
    End Function

    ''' <summary>Indique si une mise à jour est disponible.</summary>
    Public Shared Function MiseAJourDisponible(ByVal versionActuelle As VersionSemantique,
                                               ByVal publications As IEnumerable(Of Publication)) As Boolean
        Return RechercherDerniere(versionActuelle, publications) IsNot Nothing
    End Function

    ''' <summary>
    ''' Vrai si au moins une version <b>obligatoire</b> est plus récente que
    ''' la version actuelle (l'utilisateur ne peut pas la sauter).
    ''' </summary>
    Public Shared Function MiseAJourObligatoireEnAttente(ByVal versionActuelle As VersionSemantique,
                                                         ByVal publications As IEnumerable(Of Publication)) As Boolean
        If publications Is Nothing Then Return False
        Return publications.Any(Function(p) p IsNot Nothing AndAlso p.Obligatoire AndAlso p.Version > versionActuelle)
    End Function

    ''' <summary>Calcule l'empreinte SHA-256 (hex minuscules) d'un paquet.</summary>
    Public Shared Function CalculerEmpreinte(ByVal paquet As Byte()) As String
        If paquet Is Nothing Then Throw New ArgumentNullException(NameOf(paquet))
        Using algo As SHA256 = SHA256.Create()
            Dim emp As Byte() = algo.ComputeHash(paquet)
            Dim sb As New StringBuilder(emp.Length * 2)
            For Each o As Byte In emp
                sb.Append(o.ToString("x2"))
            Next
            Return sb.ToString()
        End Using
    End Function

    ''' <summary>Vérifie qu'un paquet correspond à l'empreinte attendue (insensible à la casse).</summary>
    Public Shared Function VerifierIntegrite(ByVal paquet As Byte(), ByVal empreinteAttendue As String) As Boolean
        If paquet Is Nothing OrElse String.IsNullOrWhiteSpace(empreinteAttendue) Then Return False
        Return String.Equals(CalculerEmpreinte(paquet), empreinteAttendue.Trim(), StringComparison.OrdinalIgnoreCase)
    End Function

End Class

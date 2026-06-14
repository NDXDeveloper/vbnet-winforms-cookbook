' ============================================================================
'  Publication.vb  -  Description d'une version publiee (manifeste).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Une version mise à disposition (entrée du manifeste de déploiement).</summary>
Public NotInheritable Class Publication

    ''' <summary>Numéro de version de cette publication.</summary>
    Public Property Version As VersionSemantique

    ''' <summary>Notes de version (nouveautés, corrections).</summary>
    Public Property Notes As String

    ''' <summary>Adresse de téléchargement du paquet.</summary>
    Public Property UrlPaquet As String

    ''' <summary>Empreinte SHA-256 (hex) attendue du paquet téléchargé.</summary>
    Public Property EmpreinteSha256 As String

    ''' <summary>Vrai si la mise à jour est obligatoire (saut impossible).</summary>
    Public Property Obligatoire As Boolean

    ''' <summary>Date de publication.</summary>
    Public Property PublieeLe As DateTime

    Public Overrides Function ToString() As String
        Return Version.ToString() & If(Obligatoire, " (obligatoire)", "")
    End Function

End Class

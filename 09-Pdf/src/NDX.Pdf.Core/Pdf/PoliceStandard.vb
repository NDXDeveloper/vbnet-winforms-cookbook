' ============================================================================
'  PoliceStandard.vb  -  Polices de base d'un PDF (les 14 polices standard).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>
''' Sous-ensemble des « 14 polices standard » PDF : toujours disponibles, jamais
''' à incorporer dans le fichier. Courier est à chasse fixe (utile pour le calcul
''' exact de largeur, donc pour le retour à la ligne).
''' </summary>
Public Enum PoliceStandard
    ''' <summary>Helvetica (sans empattement).</summary>
    Helvetica = 0
    ''' <summary>Helvetica gras.</summary>
    HelveticaGras = 1
    ''' <summary>Times (avec empattements).</summary>
    Times = 2
    ''' <summary>Times gras.</summary>
    TimesGras = 3
    ''' <summary>Courier (chasse fixe / monospace).</summary>
    Courier = 4
End Enum

''' <summary>Correspondances entre <see cref="PoliceStandard"/> et le format PDF.</summary>
Public NotInheritable Class PolicesPdf

    Private Sub New()
    End Sub

    ''' <summary>Nom PDF « BaseFont » de la police.</summary>
    Public Shared Function NomBaseFont(ByVal police As PoliceStandard) As String
        Select Case police
            Case PoliceStandard.Helvetica : Return "Helvetica"
            Case PoliceStandard.HelveticaGras : Return "Helvetica-Bold"
            Case PoliceStandard.Times : Return "Times-Roman"
            Case PoliceStandard.TimesGras : Return "Times-Bold"
            Case PoliceStandard.Courier : Return "Courier"
            Case Else : Return "Helvetica"
        End Select
    End Function

    ''' <summary>Nom de ressource interne (ex. « F1 ») référencé dans le flux de contenu.</summary>
    Public Shared Function Reference(ByVal police As PoliceStandard) As String
        Return "F" & (CInt(police) + 1).ToString()
    End Function

    ''' <summary>Nombre total de polices déclarées (pour générer les objets PDF).</summary>
    Public Shared ReadOnly Property Nombre As Integer
        Get
            Return [Enum].GetValues(GetType(PoliceStandard)).Length
        End Get
    End Property

End Class

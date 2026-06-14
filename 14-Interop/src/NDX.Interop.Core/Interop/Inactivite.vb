' ============================================================================
'  Inactivite.vb  -  Duree d'inactivite utilisateur (GetLastInputInfo).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Runtime.InteropServices

''' <summary>
''' Mesure le temps écoulé depuis la dernière interaction clavier/souris, via
''' l'API <c>GetLastInputInfo</c>. Le calcul du delta gère le <b>repli</b>
''' (wraparound) du compteur de ticks 32 bits (qui revient à zéro toutes les ~49,7
''' jours) ; ce calcul est isolé pour être testable sans appel système.
''' </summary>
Public NotInheritable Class Inactivite

    Private Sub New()
    End Sub

    Private Const MASQUE_32 As Long = &HFFFFFFFFL

    ''' <summary>
    ''' Différence (en ms) entre deux ticks 32 bits non signés, en tenant compte
    ''' du repli. Pur : aucune dépendance système.
    ''' </summary>
    Public Shared Function MillisecondesInactif(ByVal tickActuel As Long, ByVal tickDerniereEntree As Long) As Long
        Return (tickActuel - tickDerniereEntree) And MASQUE_32
    End Function

    ''' <summary>Durée d'inactivité courante (TimeSpan.Zero si l'API échoue).</summary>
    Public Shared Function Duree() As TimeSpan
        Dim info As New LASTINPUTINFO()
        info.cbSize = CUInt(Marshal.SizeOf(info))
        If Not GetLastInputInfo(info) Then Return TimeSpan.Zero
        Dim ms As Long = MillisecondesInactif(Environment.TickCount And MASQUE_32, CLng(info.dwTime))
        Return TimeSpan.FromMilliseconds(ms)
    End Function

End Class

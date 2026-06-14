' ============================================================================
'  RessourcesProcessus.vb  -  Comptage des objets GDI / USER (GetGuiResources).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>
''' Compte les handles GDI et USER détenus par le processus courant. Très utile
''' pour diagnostiquer les <b>fuites d'objets graphiques</b> (Pen/Brush/Font non
''' libérés) qui finissent par faire échouer le rendu WinForms.
''' </summary>
Public NotInheritable Class RessourcesProcessus

    Private Sub New()
    End Sub

    ''' <summary>Nombre d'objets GDI du processus courant.</summary>
    Public Shared Function ObjetsGdi() As Integer
        Return CInt(GetGuiResources(GetCurrentProcess(), GR_GDIOBJECTS))
    End Function

    ''' <summary>Nombre d'objets USER du processus courant.</summary>
    Public Shared Function ObjetsUser() As Integer
        Return CInt(GetGuiResources(GetCurrentProcess(), GR_USEROBJECTS))
    End Function

End Class

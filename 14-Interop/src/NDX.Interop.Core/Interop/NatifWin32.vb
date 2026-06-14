' ============================================================================
'  NatifWin32.vb  -  Declarations P/Invoke vers les API Windows (user32/kernel32).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Runtime.InteropServices

''' <summary>
''' Déclarations d'interopérabilité avec les fonctions natives Windows. Chaque
''' <see cref="DllImportAttribute"/> relie une fonction managée à son équivalent
''' non managé (DLL système). Réservé à l'usage interne de la bibliothèque.
''' </summary>
Friend Module NatifWin32

    ''' <summary>Structure attendue par <c>GetLastInputInfo</c> (taille + tick de la dernière entrée).</summary>
    <StructLayout(LayoutKind.Sequential)>
    Public Structure LASTINPUTINFO
        Public cbSize As UInteger
        Public dwTime As UInteger
    End Structure

    ''' <summary>Récupère l'instant (tick) de la dernière entrée clavier/souris.</summary>
    <DllImport("user32.dll")>
    Public Function GetLastInputInfo(ByRef plii As LASTINPUTINFO) As Boolean
    End Function

    ''' <summary>Compte les objets GDI (uiFlags=0) ou USER (uiFlags=1) d'un processus.</summary>
    <DllImport("user32.dll")>
    Public Function GetGuiResources(ByVal hProcess As IntPtr, ByVal uiFlags As Integer) As UInteger
    End Function

    ''' <summary>Pseudo-handle du processus courant.</summary>
    <DllImport("kernel32.dll")>
    Public Function GetCurrentProcess() As IntPtr
    End Function

    ''' <summary>Modifie la position/ordre Z d'une fenêtre (utilisé pour « toujours au premier plan »).</summary>
    <DllImport("user32.dll")>
    Public Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr,
                                 ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer,
                                 ByVal uFlags As UInteger) As Boolean
    End Function

    ' Indicateurs / constantes Win32.
    Public Const GR_GDIOBJECTS As Integer = 0
    Public Const GR_USEROBJECTS As Integer = 1

    Public ReadOnly HWND_TOPMOST As New IntPtr(-1)
    Public ReadOnly HWND_NOTOPMOST As New IntPtr(-2)
    Public Const SWP_NOSIZE As UInteger = &H1UI
    Public Const SWP_NOMOVE As UInteger = &H2UI

End Module

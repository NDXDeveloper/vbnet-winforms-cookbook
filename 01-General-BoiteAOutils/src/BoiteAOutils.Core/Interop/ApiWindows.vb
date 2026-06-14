Imports System.Runtime.InteropServices

''' <summary>
''' Declarations d'interoperabilite avec l'API Win32 (P/Invoke).
''' </summary>
''' <remarks>
''' Les appels passent par <see cref="DllImportAttribute"/>, qui permet
''' d'invoquer des fonctions exportees par des DLL systeme (ici <c>user32.dll</c>).
''' </remarks>
Friend Module ApiWindows

    ' --- Constantes SystemParametersInfo --------------------------------------
    ''' <summary>Active/desactive le bip systeme.</summary>
    Public Const SPI_SETBEEP As Integer = 2
    ''' <summary>Active/desactive l'economiseur d'ecran.</summary>
    Public Const SPI_SETSCREENSAVEACTIVE As Integer = 17
    ''' <summary>Ecrit le changement dans le profil utilisateur.</summary>
    Public Const SPIF_UPDATEINIFILE As Integer = &H1
    ''' <summary>Diffuse le message de changement aux applications.</summary>
    Public Const SPIF_SENDWININICHANGE As Integer = &H2

    ''' <summary>
    ''' Recupere l'identifiant du thread et du processus proprietaires d'une fenetre.
    ''' </summary>
    <DllImport("user32.dll", SetLastError:=True)>
    Public Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer
    End Function

    ''' <summary>
    ''' Lit ou modifie un parametre systeme (bip, economiseur d'ecran, etc.).
    ''' </summary>
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Function SystemParametersInfo(ByVal uAction As Integer,
                                         ByVal uParam As Integer,
                                         ByVal lpvParam As Integer,
                                         ByVal fuWinIni As Integer) As Integer
    End Function

End Module

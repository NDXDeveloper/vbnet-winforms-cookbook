' ============================================================================
'  VerrouInstance.vb  -  Instance unique d'application via un Mutex nomme.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Threading

''' <summary>
''' Garantit qu'une seule instance « primaire » s'exécute, grâce à un <see cref="Mutex"/>
''' nommé : la première instance le crée (<see cref="EstPremiere"/> = vrai) ; les
''' suivantes constatent qu'il existe déjà. À libérer (<see cref="Dispose"/>) en fin
''' de vie.
''' </summary>
Public NotInheritable Class VerrouInstance
    Implements IDisposable

    Private ReadOnly _mutex As Mutex
    Private _possede As Boolean

    Public Sub New(ByVal nom As String)
        Dim cree As Boolean
        _mutex = New Mutex(initiallyOwned:=True, name:=nom, createdNew:=cree)
        EstPremiere = cree
        _possede = cree
    End Sub

    ''' <summary>Vrai si CETTE instance est la première (primaire).</summary>
    Public ReadOnly Property EstPremiere As Boolean

    Public Sub Dispose() Implements IDisposable.Dispose
        Try
            If _possede Then
                _mutex.ReleaseMutex()
                _possede = False
            End If
        Catch
            ' Ignorer : le mutex peut déjà être libéré.
        End Try
        _mutex.Dispose()
    End Sub

End Class

' ============================================================================
'  PuitsConsole.vb
'  Puits ecrivant vers la sortie de debogage (et la console si presente).
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Diagnostics

''' <summary>Puits ecrivant chaque entree formatee dans la sortie de debogage.</summary>
Public NotInheritable Class PuitsConsole
    Implements IPuits

    Public Sub Ecrire(ByVal entree As EntreeJournal) Implements IPuits.Ecrire
        Debug.WriteLine(entree.Formater())
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Rien a liberer.
    End Sub

End Class

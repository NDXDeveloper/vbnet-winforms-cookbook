' ============================================================================
'  DepotTacheTests.vb  -  Test d'integration (base "traitements").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Asynchrone

<TestClass>
Public Class DepotTacheTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotTache.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Empiler_Puis_Traiter_ChangeLEtat()
        ExigerBase()
        Dim libelle As String = "Test " & DateTime.UtcNow.Ticks.ToString()
        Dim id As Integer = DepotTache.Empiler(libelle)
        Assert.IsTrue(id > 0)

        Assert.IsTrue(DepotTache.ListerEnAttente().Any(Function(t) t.Id = id), "La tâche doit être en attente.")
        DepotTache.MarquerTraitee(id)
        Assert.IsFalse(DepotTache.ListerEnAttente().Any(Function(t) t.Id = id), "La tâche ne doit plus être en attente.")
    End Sub

End Class

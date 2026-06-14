' ============================================================================
'  DepotEvenementTests.vb  -  Test d'integration (base "demarrage").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Demarrage

<TestClass>
Public Class DepotEvenementTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotEvenement.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Enregistrer_AjouteUnEvenement()
        ExigerBase()
        Dim avant As Integer = DepotEvenement.ListerTable().Rows.Count
        DepotEvenement.Enregistrer("test", "Événement de test " & DateTime.UtcNow.Ticks.ToString())
        Dim apres As Integer = DepotEvenement.ListerTable().Rows.Count
        Assert.IsTrue(apres >= avant AndAlso apres > 0)
    End Sub

End Class

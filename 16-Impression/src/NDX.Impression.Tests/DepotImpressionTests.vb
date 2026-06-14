' ============================================================================
'  DepotImpressionTests.vb  -  Test d'integration (base "impressions").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Impression

<TestClass>
Public Class DepotImpressionTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotImpression.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Enregistrer_Puis_Recharger_RestitueLeContenu()
        ExigerBase()
        Dim contenu As String = "Première ligne" & vbCrLf & "Deuxième ligne"
        Dim id As Integer = DepotImpression.Enregistrer("Test " & DateTime.UtcNow.Ticks.ToString(), contenu, 1)
        Assert.IsTrue(id > 0)
        Assert.AreEqual(contenu, DepotImpression.Recharger(id))
    End Sub

End Class

' ============================================================================
'  DepotCommandeTests.vb  -  Test d'integration (base "commandes").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.InstanceUnique

<TestClass>
Public Class DepotCommandeTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotCommande.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Enregistrer_AjouteUneLigne()
        ExigerBase()
        Dim avant As Integer = DepotCommande.ListerTable().Rows.Count
        DepotCommande.Enregistrer("instance-2", "--ouvrir rapport.txt")
        Dim apres As Integer = DepotCommande.ListerTable().Rows.Count
        Assert.IsTrue(apres >= avant AndAlso apres > 0)
    End Sub

End Class

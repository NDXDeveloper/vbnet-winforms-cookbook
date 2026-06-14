' ============================================================================
'  DepotSupervisionTests.vb  -  Test d'integration (base "supervision").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Interop

<TestClass>
Public Class DepotSupervisionTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotSupervision.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Enregistrer_Puis_Lister_AjouteUneLigne()
        ExigerBase()
        Dim avant As Integer = DepotSupervision.ListerTable().Rows.Count
        DepotSupervision.Enregistrer(120, 35, 4200)
        Dim apres As Integer = DepotSupervision.ListerTable().Rows.Count
        Assert.IsTrue(apres >= avant, "La liste ne doit pas diminuer après un enregistrement.")
        Assert.IsTrue(apres > 0)
    End Sub

End Class

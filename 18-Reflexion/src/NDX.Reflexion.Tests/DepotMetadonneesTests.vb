' ============================================================================
'  DepotMetadonneesTests.vb  -  Test d'integration (base "metadonnees").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Reflexion

<TestClass>
Public Class DepotMetadonneesTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotMetadonnees.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub EnregistrerDescription_InsereLesMembres()
        ExigerBase()
        Dim n As Integer = DepotMetadonnees.EnregistrerDescription(GetType(Article))
        Assert.IsTrue(n >= 6, "Article a 5 propriétés + 1 événement au minimum.")
        Assert.IsTrue(DepotMetadonnees.ListerTable().Rows.Count > 0)
    End Sub

End Class

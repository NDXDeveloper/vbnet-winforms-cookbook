' ============================================================================
'  DepotArticleTests.vb  -  Test d'integration (base "inventaire").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.ControlesII

<TestClass>
Public Class DepotArticleTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotArticle.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Ajouter_Puis_Lister_ContientLesArticles()
        ExigerBase()
        DepotArticle.Ajouter("ART-" & DateTime.UtcNow.Ticks.ToString(), "Article de test", 12.5D, 4)
        Dim table = DepotArticle.ListerTable()
        Assert.IsTrue(table.Rows.Count > 0)
        Assert.IsTrue(table.Columns.Contains("Référence"))
    End Sub

End Class

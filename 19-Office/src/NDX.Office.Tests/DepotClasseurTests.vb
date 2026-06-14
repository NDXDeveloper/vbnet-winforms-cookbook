' ============================================================================
'  DepotClasseurTests.vb  -  Test d'integration (base "classeur").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Office

<TestClass>
Public Class DepotClasseurTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotClasseur.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Ajouter_Puis_ListerLignes_ContientEntete()
        ExigerBase()
        DepotClasseur.Ajouter("v1-" & DateTime.UtcNow.Ticks.ToString(), "v2", "v3")
        Dim lignes = DepotClasseur.ListerLignes()
        Assert.IsTrue(lignes.Count >= 2, "En-tête + au moins une ligne de données.")
        Assert.AreEqual("Valeur 1", lignes(0)(0), "La première ligne est l'en-tête.")
    End Sub

End Class

' ============================================================================
'  DepotMesureTests.vb  -  Test d'integration (base "mesures").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Graphiques

<TestClass>
Public Class DepotMesureTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotMesure.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Ajouter_Puis_ChargerSerie_ContientLaMesure()
        ExigerBase()
        Dim libelle As String = "T" & (DateTime.UtcNow.Ticks Mod 100000).ToString()
        DepotMesure.Ajouter(libelle, 42.0)
        Dim serie = DepotMesure.ChargerSerie()
        Assert.IsTrue(serie.Nombre > 0, "La série ne doit pas être vide après ajout.")
        Assert.IsTrue(serie.Libelles.Contains(libelle), "Le libellé ajouté doit être présent.")
    End Sub

End Class

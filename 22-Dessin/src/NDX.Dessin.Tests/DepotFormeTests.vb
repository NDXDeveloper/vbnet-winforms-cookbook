' ============================================================================
'  DepotFormeTests.vb  -  Test d'integration (base "atelier").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Dessin

<TestClass>
Public Class DepotFormeTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotForme.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub EnregistrerScene_Puis_ChargerScene_RestitueLesFormes()
        ExigerBase()
        Dim scene As New Scene()
        scene.Ajouter(New Forme() With {.Type = TypeForme.Rectangle, .X = 5, .Y = 6, .Largeur = 40, .Hauteur = 20, .CouleurHex = "#1565C0"})
        scene.Ajouter(New Forme() With {.Type = TypeForme.Ellipse, .X = 50, .Y = 60, .Largeur = 30, .Hauteur = 30, .CouleurHex = "#AD1457"})

        DepotForme.EnregistrerScene(scene)
        Dim relue = DepotForme.ChargerScene()

        Assert.AreEqual(2, relue.Formes.Count)
        Assert.AreEqual(TypeForme.Rectangle, relue.Formes(0).Type)
        Assert.AreEqual(40, relue.Formes(0).Largeur)
    End Sub

End Class

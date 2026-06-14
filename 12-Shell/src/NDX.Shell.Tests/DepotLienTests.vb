' ============================================================================
'  DepotLienTests.vb  -  Test d'integration (base "catalogue_liens").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Shell

<TestClass>
Public Class DepotLienTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotLien.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Ajouter_Puis_Lister_ContientLeLien()
        ExigerBase()
        Dim nom As String = "Test " & DateTime.UtcNow.Ticks.ToString()
        Dim id As Integer = DepotLien.Ajouter(New Lien() With {
            .Categorie = "web", .Nom = nom, .Cible = "https://exemple.test", .Description = "test"})
        Assert.IsTrue(id > 0)
        Assert.IsTrue(DepotLien.ListerObjets().Any(Function(l) l.Nom = nom))
        DepotLien.Supprimer(id)
        Assert.IsFalse(DepotLien.ListerObjets().Any(Function(l) l.Nom = nom), "Le lien doit avoir été supprimé.")
    End Sub

    <TestMethod>
    Public Sub Lister_SeedNonVide()
        ExigerBase()
        Assert.IsTrue(DepotLien.ListerObjets().Count > 0, "Le jeu d'amorçage doit contenir des liens.")
    End Sub

End Class

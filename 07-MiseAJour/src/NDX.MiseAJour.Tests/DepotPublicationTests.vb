' ============================================================================
'  DepotPublicationTests.vb  -  Test d'integration (base "deploiement").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.MiseAJour

<TestClass>
Public Class DepotPublicationTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotPublication.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Publier_Puis_Lister_ContientLaVersion()
        ExigerBase()
        Dim version As String = "9.9." & (DateTime.UtcNow.Millisecond).ToString()
        DepotPublication.Publier(New Publication() With {
            .Version = VersionSemantique.Analyser(version),
            .Notes = "Publication de test.",
            .Obligatoire = False,
            .PublieeLe = DateTime.Now})

        Dim liste = DepotPublication.Lister()
        Assert.IsTrue(liste.Any(Function(p) p.Version.ToString() = version),
                      "La version publiée doit apparaître dans le manifeste.")
    End Sub

    <TestMethod>
    Public Sub Manifeste_SeedContientUneMiseAJourPour_1_0_0()
        ExigerBase()
        Dim liste = DepotPublication.Lister()
        Dim p = ServiceMiseAJour.RechercherDerniere(VersionSemantique.Analyser("1.0.0"), liste)
        Assert.IsNotNull(p, "Le jeu d'amorçage doit proposer une version plus récente que 1.0.0.")
    End Sub

End Class

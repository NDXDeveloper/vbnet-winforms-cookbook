' ============================================================================
'  DepotRaccourciTests.vb  -  Test d'integration (base "raccourcis").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Raccourcis

<TestClass>
Public Class DepotRaccourciTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotRaccourci.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Definir_NormaliseEtRelit()
        ExigerBase()
        ' Saisie « libre » : doit être normalisée à l'enregistrement.
        ' (Accord choisi de façon à ne pas entrer en conflit avec le jeu d'amorçage.)
        DepotRaccourci.Definir("Test aller-retour", "ctrl+alt+k  ctrl+alt+s")
        Dim liaison = DepotRaccourci.Charger().FirstOrDefault(Function(p) p.Key = "Test aller-retour")
        Assert.AreEqual("Ctrl+Alt+K, Ctrl+Alt+S", liaison.Value)
    End Sub

    <TestMethod>
    Public Sub ConstruireGestionnaire_ChargeLesLiaisonsAmorcees()
        ExigerBase()
        Dim g = DepotRaccourci.ConstruireGestionnaire()
        Assert.IsTrue(g.Nombre > 0, "Le jeu d'amorçage doit fournir des raccourcis.")
    End Sub

End Class

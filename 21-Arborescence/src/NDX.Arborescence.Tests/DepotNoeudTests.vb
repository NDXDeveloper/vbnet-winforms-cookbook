' ============================================================================
'  DepotNoeudTests.vb  -  Test d'integration (base "arborescence").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Arborescence

<TestClass>
Public Class DepotNoeudTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotNoeud.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Ajouter_ParentEnfant_Puis_Descendants_Recursif()
        ExigerBase()
        Dim racine As Integer = DepotNoeud.Ajouter(Nothing, "Racine de test " & DateTime.UtcNow.Ticks.ToString())
        Dim enfant As Integer = DepotNoeud.Ajouter(racine, "Enfant")
        DepotNoeud.Ajouter(enfant, "Petit-enfant")

        Dim descendants = DepotNoeud.Descendants(racine)
        Assert.AreEqual(2, descendants.Rows.Count, "La racine doit avoir 2 descendants (enfant + petit-enfant).")

        ' Nettoyage : la suppression de la racine retire la sous-arborescence (cascade).
        DepotNoeud.Supprimer(racine)
        Assert.IsFalse(DepotNoeud.Lister().Any(Function(n) n.Id = racine))
    End Sub

End Class

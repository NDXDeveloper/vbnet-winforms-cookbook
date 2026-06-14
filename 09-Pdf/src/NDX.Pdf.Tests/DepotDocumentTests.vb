' ============================================================================
'  DepotDocumentTests.vb  -  Test d'integration (base "bibliotheque").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Pdf

<TestClass>
Public Class DepotDocumentTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotDocument.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Archiver_Puis_Recharger_RestitueLeBinaire()
        ExigerBase()
        Dim doc As New DocumentPdf() With {.Titre = "Test intégration", .Auteur = "Tests"}
        doc.AjouterPage().EcrireTexte(50, 60, "Contenu de test.")
        Dim pdf As Byte() = doc.Construire()

        Dim id As Integer = DepotDocument.Enregistrer(doc.Titre, doc.Auteur, doc.NombrePages, pdf)
        Assert.IsTrue(id > 0)

        Dim relu As Byte() = DepotDocument.Recharger(id)
        CollectionAssert.AreEqual(pdf, relu, "Le PDF rechargé doit être identique à l'original.")
    End Sub

    <TestMethod>
    Public Sub Lister_ContientLaColonneTitre()
        ExigerBase()
        Dim doc As New DocumentPdf()
        doc.AjouterPage().EcrireTexte(50, 60, "x")
        DepotDocument.Enregistrer("pour-liste", "Tests", 1, doc.Construire())
        Dim table = DepotDocument.Lister()
        Assert.IsTrue(table.Rows.Count > 0)
        Assert.IsTrue(table.Columns.Contains("Titre"))
    End Sub

End Class

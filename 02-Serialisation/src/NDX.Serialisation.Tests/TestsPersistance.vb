' ============================================================================
'  TestsPersistance.vb
'  Tests d'integration de la persistance en base (coffre de documents).
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Data
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Serialisation

''' <summary>
''' Tests d'integration : necessitent le conteneur MariaDB demarre
''' (docker compose up -d). En son absence, les tests se terminent en
''' <c>Inconclusive</c> plutot qu'en echec.
''' </summary>
<TestClass>
Public Class TestsPersistance

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotDocuments.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez 'docker compose up -d') : " & message)
        End If
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub TesterConnexion_renvoie_la_version()
        ExigerBase()
        Dim message As String = ""
        Assert.IsTrue(DepotDocuments.TesterConnexion(message))
        StringAssert.Contains(message, "OK")
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub Enregistrer_puis_recharger_round_trip()
        ExigerBase()
        Dim modele As Catalogue = Catalogue.Exemple()
        Dim id As Integer = DepotDocuments.Enregistrer("Test d'intégration", modele, FormatSerialisation.Binaire)
        Assert.IsTrue(id > 0, "L'identifiant inséré doit être positif.")

        Dim copie As Catalogue = DepotDocuments.Recharger(Of Catalogue)(id)
        Assert.IsNotNull(copie)
        Assert.AreEqual(modele.Nom, copie.Nom)
        Assert.AreEqual(modele.Produits.Count, copie.Produits.Count)

        Assert.IsTrue(DepotDocuments.VerifierIntegrite(id), "L'empreinte SHA-256 doit correspondre.")
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub Lister_retourne_une_table()
        ExigerBase()
        DepotDocuments.Enregistrer("Document de liste", Catalogue.Exemple(), FormatSerialisation.Json)
        Dim table As DataTable = DepotDocuments.Lister()
        Assert.IsNotNull(table)
        Assert.IsTrue(table.Rows.Count > 0)
        Assert.IsTrue(table.Columns.Contains("Id"))
        Assert.IsTrue(table.Columns.Contains("Format"))
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub ListerCategories_contient_les_categories_amorcees()
        ExigerBase()
        Dim table As DataTable = DepotDocuments.ListerCategories()
        Assert.IsNotNull(table)
        Assert.IsTrue(table.Rows.Count >= 4, "Les 4 catégories de démonstration doivent être présentes.")
    End Sub

End Class

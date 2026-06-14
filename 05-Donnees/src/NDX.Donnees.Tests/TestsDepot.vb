' ============================================================================
'  TestsDepot.vb  -  Tests d'integration du depot et de l'unite de travail.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Donnees

''' <summary>Necessitent le conteneur MariaDB demarre ; sinon Inconclusive.</summary>
<TestClass>
Public Class TestsDepot

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotProduit.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (docker compose up -d) : " & message)
        End If
    End Sub

    Private Shared Function Ref() As String
        Return "IT-" & Guid.NewGuid().ToString("N").Substring(0, 10)
    End Function

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub Crud_complet()
        ExigerBase()
        Dim depot As New DepotProduit()
        Dim p As New Produit() With {.Reference = Ref(), .Designation = "Produit de test", .PrixHt = 1.23D, .Stock = 5}

        Dim id As Integer = depot.Inserer(p)
        Assert.IsTrue(id > 0)

        Dim lu As Produit = depot.ParId(id)
        Assert.IsNotNull(lu)
        Assert.AreEqual(p.Reference, lu.Reference)
        Assert.AreEqual(5, lu.Stock)
        Assert.AreEqual(1.23D, lu.PrixHt)

        lu.Stock = 9
        Assert.IsTrue(depot.MettreAJour(lu))
        Assert.AreEqual(9, depot.ParId(id).Stock)

        Assert.IsTrue(depot.Supprimer(id))
        Assert.IsNull(depot.ParId(id))
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub Lister_et_compter()
        ExigerBase()
        Dim depot As New DepotProduit()
        Assert.IsTrue(depot.Compter() > 0)
        Dim page = depot.Lister(1, 5)
        Assert.IsTrue(page.Count <= 5)
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub UnitOfWork_annulation_ne_persiste_rien()
        ExigerBase()
        Dim avant As Long = New DepotProduit().Compter()
        Using uow As New UniteDeTravail()
            uow.Commencer()
            Dim depot As New DepotProduit(uow)
            depot.Inserer(New Produit() With {.Reference = Ref(), .Designation = "Transitoire", .PrixHt = 1D, .Stock = 1})
            uow.Annuler()
        End Using
        Assert.AreEqual(avant, New DepotProduit().Compter(), "Apres annulation, le total doit etre inchange.")
    End Sub

End Class

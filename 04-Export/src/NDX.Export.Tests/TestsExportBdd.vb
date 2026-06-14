' ============================================================================
'  TestsExportBdd.vb  -  Tests d'integration de la source de donnees.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Export

''' <summary>Necessitent le conteneur MariaDB demarre ; sinon Inconclusive.</summary>
<TestClass>
Public Class TestsExportBdd

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not SourceDonnees.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (docker compose up -d) : " & message)
        End If
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub ToutesLesVentes_retourne_des_lignes()
        ExigerBase()
        Dim t As DataTable = SourceDonnees.ToutesLesVentes()
        Assert.IsTrue(t.Rows.Count > 0)
        Assert.IsTrue(t.Columns.Contains("Produit"))
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub VentesParCategorie_agrege()
        ExigerBase()
        Dim t As DataTable = SourceDonnees.VentesParCategorie()
        Assert.IsTrue(t.Rows.Count > 0)
        Assert.IsTrue(t.Columns.Contains("Montant total"))
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub Export_excel_depuis_la_base()
        ExigerBase()
        Dim octets As Byte() = Exportateurs.Exporter(SourceDonnees.ToutesLesVentes(), FormatExport.Excel)
        Assert.IsTrue(octets.Length > 0)
    End Sub

End Class

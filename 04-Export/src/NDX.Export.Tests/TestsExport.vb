' ============================================================================
'  TestsExport.vb  -  Tests unitaires des exportateurs (CSV, Excel, PDF).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Export

<TestClass>
Public Class TestsExport

    Private Shared Function TableDemo() As DataTable
        Dim t As New DataTable()
        t.Columns.Add("Nom", GetType(String))
        t.Columns.Add("Quantite", GetType(Integer))
        t.Columns.Add("Prix", GetType(Decimal))
        t.Rows.Add("Vis; speciale", 200, 1.5D)
        t.Rows.Add("Ecrou ""M8""", 50, 0.2D)
        Return t
    End Function

    <TestMethod>
    Public Sub Csv_entetes_et_echappement()
        Dim octets As Byte() = New ExportateurCsv().Exporter(TableDemo())
        Dim texte As String = Encoding.UTF8.GetString(octets)
        StringAssert.Contains(texte, "Nom;Quantite;Prix")
        ' Le champ contenant ';' est entoure de guillemets.
        StringAssert.Contains(texte, """Vis; speciale""")
        ' Les guillemets internes sont doubles.
        StringAssert.Contains(texte, """Ecrou """"M8""""""")
    End Sub

    <TestMethod>
    Public Sub Excel_produit_un_zip_openxml_valide()
        Dim octets As Byte() = New ExportateurExcel().Exporter(TableDemo())
        Using ms As New MemoryStream(octets)
            Using zip As New ZipArchive(ms, ZipArchiveMode.Read)
                Assert.IsNotNull(zip.GetEntry("[Content_Types].xml"), "Partie [Content_Types].xml requise.")
                Assert.IsNotNull(zip.GetEntry("xl/workbook.xml"), "Partie workbook requise.")
                Dim feuille As ZipArchiveEntry = zip.GetEntry("xl/worksheets/sheet1.xml")
                Assert.IsNotNull(feuille, "La feuille doit exister.")
                Using r As New StreamReader(feuille.Open())
                    Dim xml As String = r.ReadToEnd()
                    StringAssert.Contains(xml, "Nom")
                    StringAssert.Contains(xml, "<v>200</v>")   ' cellule numerique
                End Using
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub Pdf_a_une_structure_minimale()
        Dim octets As Byte() = New ExportateurPdf() With {.Titre = "Test"}.Exporter(TableDemo())
        Dim s As String = Encoding.ASCII.GetString(octets)
        Assert.IsTrue(s.StartsWith("%PDF-1.4"), "Doit commencer par l'en-tete PDF.")
        StringAssert.Contains(s, "/BaseFont /Courier")
        StringAssert.Contains(s, "startxref")
        StringAssert.Contains(s, "%%EOF")
    End Sub

    <TestMethod>
    Public Sub Fabrique_renvoie_le_bon_exportateur()
        Assert.AreEqual(FormatExport.Csv, Exportateurs.Creer(FormatExport.Csv).Format)
        Assert.AreEqual(".xlsx", Exportateurs.Creer(FormatExport.Excel).Extension)
        Assert.AreEqual("application/pdf", Exportateurs.Creer(FormatExport.Pdf).TypeMime)
    End Sub

End Class

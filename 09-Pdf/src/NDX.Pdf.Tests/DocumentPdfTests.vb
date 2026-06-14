' ============================================================================
'  DocumentPdfTests.vb  -  Tests de structure du PDF genere.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Pdf

<TestClass>
Public Class DocumentPdfTests

    Private Shared Function Texte(ByVal pdf As Byte()) As String
        Return Encoding.GetEncoding(1252).GetString(pdf)
    End Function

    <TestMethod>
    Public Sub Construire_CommenceParEnTetePdf()
        Dim pdf = New DocumentPdf().Construire()
        Dim debut As String = Encoding.ASCII.GetString(pdf, 0, 8)
        Assert.AreEqual("%PDF-1.4", debut)
    End Sub

    <TestMethod>
    Public Sub Construire_SeTermineParEof()
        Dim pdf = New DocumentPdf().Construire()
        Assert.IsTrue(Texte(pdf).TrimEnd().EndsWith("%%EOF"), "Le PDF doit se terminer par %%EOF.")
    End Sub

    <TestMethod>
    Public Sub Construire_ContientLesElementsStructurels()
        Dim doc As New DocumentPdf()
        doc.AjouterPage().EcrireTexte(50, 50, "Bonjour")
        Dim t As String = Texte(doc.Construire())
        Assert.IsTrue(t.Contains("/Type /Catalog"), "catalogue manquant")
        Assert.IsTrue(t.Contains("/Type /Pages"), "arbre de pages manquant")
        Assert.IsTrue(t.Contains("/WinAnsiEncoding"), "encodage WinAnsi manquant")
        Assert.IsTrue(t.Contains("xref"), "table xref manquante")
        Assert.IsTrue(t.Contains("trailer"), "trailer manquant")
        Assert.IsTrue(t.Contains("startxref"), "startxref manquant")
    End Sub

    <TestMethod>
    Public Sub Construire_CompteLesPages()
        Dim doc As New DocumentPdf()
        doc.AjouterPage()
        doc.AjouterPage()
        doc.AjouterPage()
        Assert.AreEqual(3, doc.NombrePages)
        Assert.IsTrue(Texte(doc.Construire()).Contains("/Count 3"), "le nombre de pages doit apparaître dans l'arbre.")
    End Sub

    <TestMethod>
    Public Sub Construire_DocumentVide_ProduitUnePageParDefaut()
        Dim doc As New DocumentPdf()
        Dim t As String = Texte(doc.Construire())
        Assert.IsTrue(t.Contains("/Count 1"), "un document vide doit produire une page par défaut.")
    End Sub

    <TestMethod>
    Public Sub Metadonnees_SontPresentes()
        Dim doc As New DocumentPdf() With {.Titre = "Mon Titre", .Auteur = "Moi"}
        Dim t As String = Texte(doc.Construire())
        Assert.IsTrue(t.Contains("(Mon Titre)"), "titre manquant dans les métadonnées")
        Assert.IsTrue(t.Contains("(Moi)"), "auteur manquant dans les métadonnées")
    End Sub

    <TestMethod>
    Public Sub EcrireParagraphe_RenvoieUnYPlusBas()
        Dim page = New DocumentPdf().AjouterPage()
        Dim yApres As Double = page.EcrireParagraphe(50, 100, 400, "Du texte sur plusieurs mots à mettre en page.", 11)
        Assert.IsTrue(yApres > 100, "Y doit progresser vers le bas après le paragraphe.")
    End Sub

End Class

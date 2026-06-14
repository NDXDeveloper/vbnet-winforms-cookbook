' ============================================================================
'  TestsSerialisation.vb
'  Tests unitaires de la serialisation (aller-retour, fichier, stockage isole).
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Serialisation

''' <summary>Tests des conversions de serialisation (sans dependance externe).</summary>
<TestClass>
Public Class TestsSerialisation

    Private Shared ReadOnly TousFormats As FormatSerialisation() = {
        FormatSerialisation.Xml, FormatSerialisation.ContratXml,
        FormatSerialisation.Binaire, FormatSerialisation.Json}

    Private Shared Sub AssertCatalogueEgal(ByVal attendu As Catalogue, ByVal obtenu As Catalogue)
        Assert.IsNotNull(obtenu)
        Assert.AreEqual(attendu.Nom, obtenu.Nom)
        Assert.AreEqual(attendu.Produits.Count, obtenu.Produits.Count)
        For i As Integer = 0 To attendu.Produits.Count - 1
            Assert.AreEqual(attendu.Produits(i).Reference, obtenu.Produits(i).Reference)
            Assert.AreEqual(attendu.Produits(i).Designation, obtenu.Produits(i).Designation)
            Assert.AreEqual(attendu.Produits(i).PrixHt, obtenu.Produits(i).PrixHt)
            Assert.AreEqual(attendu.Produits(i).Quantite, obtenu.Produits(i).Quantite)
        Next
    End Sub

    <TestMethod>
    Public Sub AllerRetour_octets_pour_chaque_format()
        Dim modele As Catalogue = Catalogue.Exemple()
        For Each format As FormatSerialisation In TousFormats
            Dim octets As Byte() = Serialiseur.VersOctets(modele, format)
            Assert.IsTrue(octets.Length > 0, "Le payload ne doit pas être vide (" & format.ToString() & ").")
            Dim copie As Catalogue = Serialiseur.DepuisOctets(Of Catalogue)(octets, format)
            AssertCatalogueEgal(modele, copie)
        Next
    End Sub

    <TestMethod>
    Public Sub AllerRetour_chaine_pour_les_formats_texte()
        Dim modele As Catalogue = Catalogue.Exemple()
        For Each format As FormatSerialisation In {FormatSerialisation.Xml, FormatSerialisation.ContratXml, FormatSerialisation.Json}
            Dim texte As String = Serialiseur.VersChaine(modele, format)
            Assert.IsFalse(String.IsNullOrEmpty(texte))
            Dim copie As Catalogue = Serialiseur.DepuisChaine(Of Catalogue)(texte, format)
            AssertCatalogueEgal(modele, copie)
        Next
    End Sub

    <TestMethod>
    Public Sub VersChaine_rejette_le_format_binaire()
        Assert.ThrowsException(Of InvalidOperationException)(
            Sub() Serialiseur.VersChaine(Catalogue.Exemple(), FormatSerialisation.Binaire))
    End Sub

    <TestMethod>
    Public Sub EstFormatTexte_distingue_binaire_et_texte()
        Assert.IsTrue(Serialiseur.EstFormatTexte(FormatSerialisation.Xml))
        Assert.IsTrue(Serialiseur.EstFormatTexte(FormatSerialisation.Json))
        Assert.IsFalse(Serialiseur.EstFormatTexte(FormatSerialisation.Binaire))
    End Sub

    <TestMethod>
    Public Sub AllerRetour_via_fichier()
        Dim modele As Catalogue = Catalogue.Exemple()
        Dim chemin As String = Path.GetTempFileName()
        Try
            Serialiseur.SauverFichier(modele, chemin, FormatSerialisation.Xml)
            Dim copie As Catalogue = Serialiseur.ChargerFichier(Of Catalogue)(chemin, FormatSerialisation.Xml)
            AssertCatalogueEgal(modele, copie)
        Finally
            If File.Exists(chemin) Then File.Delete(chemin)
        End Try
    End Sub

    <TestMethod>
    Public Sub AllerRetour_via_stockage_isole()
        Dim modele As Catalogue = Catalogue.Exemple()
        Dim nom As String = "test_" & Guid.NewGuid().ToString("N") & ".dat"
        Try
            StockageIsole.Sauver(modele, nom, FormatSerialisation.Binaire)
            Assert.IsTrue(StockageIsole.Existe(nom))
            Dim copie As Catalogue = StockageIsole.Charger(Of Catalogue)(nom, FormatSerialisation.Binaire)
            AssertCatalogueEgal(modele, copie)
        Finally
            StockageIsole.Supprimer(nom)
        End Try
        Assert.IsFalse(StockageIsole.Existe(nom))
    End Sub

    <TestMethod>
    Public Sub Le_binaire_est_plus_compact_que_le_xml()
        Dim modele As Catalogue = Catalogue.Exemple()
        Dim tailleXml As Integer = Serialiseur.VersOctets(modele, FormatSerialisation.Xml).Length
        Dim tailleBinaire As Integer = Serialiseur.VersOctets(modele, FormatSerialisation.Binaire).Length
        Assert.IsTrue(tailleBinaire < tailleXml,
            String.Format("Binaire ({0}) devrait être plus compact que XML ({1}).", tailleBinaire, tailleXml))
    End Sub

End Class

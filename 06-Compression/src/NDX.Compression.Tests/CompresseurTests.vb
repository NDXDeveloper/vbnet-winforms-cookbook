' ============================================================================
'  CompresseurTests.vb  -  Tests unitaires de la compression (sans base).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Compression

<TestClass>
Public Class CompresseurTests

    <DataTestMethod>
    <DataRow(AlgorithmeCompression.GZip)>
    <DataRow(AlgorithmeCompression.Deflate)>
    Public Sub AllerRetour_Octets_Identique(ByVal algo As AlgorithmeCompression)
        Dim source As Byte() = Encoding.UTF8.GetBytes(String.Concat(Enumerable.Repeat("Données à compresser. ", 200)))
        Dim compresse As Byte() = Compresseur.Compresser(source, algo)
        Dim restitue As Byte() = Compresseur.Decompresser(compresse, algo)
        CollectionAssert.AreEqual(source, restitue)
    End Sub

    <DataTestMethod>
    <DataRow(AlgorithmeCompression.GZip)>
    <DataRow(AlgorithmeCompression.Deflate)>
    Public Sub AllerRetour_Texte_Identique(ByVal algo As AlgorithmeCompression)
        Dim source As String = "Texte accentué : éàçùœ — répété. " & New String("x"c, 500)
        Dim compresse As Byte() = Compresseur.CompresserTexte(source, algo)
        Assert.AreEqual(source, Compresseur.DecompresserTexte(compresse, algo))
    End Sub

    <TestMethod>
    Public Sub TexteRepetitif_EstReellementCompresse()
        Dim source As String = New String("A"c, 5000)
        Dim compresse As Byte() = Compresseur.CompresserTexte(source, AlgorithmeCompression.GZip)
        Assert.IsTrue(compresse.Length < Encoding.UTF8.GetByteCount(source),
                      "La taille compressée doit être inférieure à l'originale pour un texte répétitif.")
    End Sub

    <TestMethod>
    Public Sub ChaineVide_AllerRetour_RendChaineVide()
        Dim compresse As Byte() = Compresseur.CompresserTexte("", AlgorithmeCompression.GZip)
        Assert.AreEqual("", Compresseur.DecompresserTexte(compresse, AlgorithmeCompression.GZip))
    End Sub

    <TestMethod>
    Public Sub Ratio_Et_Gain_SontCoherents()
        Assert.AreEqual(0.25, Compresseur.Ratio(400, 100), 0.0001)
        Assert.AreEqual(75.0, Compresseur.PourcentageGain(400, 100), 0.0001)
    End Sub

    <TestMethod>
    Public Sub Ratio_TailleOrigineNulle_NeDivisePasParZero()
        Assert.AreEqual(0.0, Compresseur.Ratio(0, 100), 0.0001)
    End Sub

    <TestMethod>
    Public Sub Compresser_Null_LeveArgumentNull()
        Assert.ThrowsException(Of ArgumentNullException)(Sub() Compresseur.Compresser(Nothing))
    End Sub

    <TestMethod>
    Public Sub GZip_Et_Deflate_ProduisentDesOctetsDifferents()
        Dim source As Byte() = Encoding.UTF8.GetBytes(New String("Z"c, 1000))
        Dim g As Byte() = Compresseur.Compresser(source, AlgorithmeCompression.GZip)
        Dim d As Byte() = Compresseur.Compresser(source, AlgorithmeCompression.Deflate)
        ' GZIP ajoute un en-tete + CRC : les flux ne sont pas identiques.
        Assert.IsFalse(g.Length = d.Length AndAlso g.SequenceEqual(d))
    End Sub

End Class

Imports System.Drawing
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BoiteAOutils

''' <summary>
''' Tests unitaires des methodes de logique pure (sans base de donnees).
''' </summary>
''' <remarks>
''' Ces tests valident le comportement des outils de la bibliotheque. Ils
''' s'executent sans dependance externe (ni base, ni reseau) : ideal pour une
''' integration continue.
''' </remarks>
<TestClass>
Public Class TestsChaines

    <TestMethod>
    Public Sub RetirerAccents_supprime_diacritiques_et_minuscule()
        Assert.AreEqual("evenement", OutilsChaines.RetirerAccents("Évènement"))
        Assert.AreEqual("ca va", OutilsChaines.RetirerAccents("Ça Va"))
    End Sub

    <TestMethod>
    Public Sub RetirerCaracteresSpeciaux_ne_garde_que_alnum_point_souligne()
        Assert.AreEqual("abc1.2_3", OutilsChaines.RetirerCaracteresSpeciaux("a b@c#1.2_3"))
    End Sub

    <TestMethod>
    Public Sub PremiereLettreEnMajuscule_cas_courants()
        Assert.AreEqual("Bonjour", OutilsChaines.PremiereLettreEnMajuscule("bonjour"))
        Assert.AreEqual("A", OutilsChaines.PremiereLettreEnMajuscule("a"))
        Assert.AreEqual("", OutilsChaines.PremiereLettreEnMajuscule(""))
    End Sub

    <TestMethod>
    Public Sub ContientGuid_detecte_un_guid_canonique()
        Assert.IsTrue(OutilsChaines.ContientGuid("ref 3F2504E0-4F89-41D3-9A0C-0305E82C3301 fin"))
        Assert.IsFalse(OutilsChaines.ContientGuid("aucun identifiant ici"))
    End Sub

End Class

<TestClass>
Public Class TestsConversions

    <TestMethod>
    Public Sub ConvertirEnEntier_valeurs_valides_et_invalides()
        Assert.AreEqual(42, OutilsConversions.ConvertirEnEntier("42"))
        Assert.AreEqual(0, OutilsConversions.ConvertirEnEntier(""))
        Assert.AreEqual(-1, OutilsConversions.ConvertirEnEntier("abc"))
    End Sub

    <TestMethod>
    Public Sub Formater_supprime_les_decimales_inutiles()
        Assert.AreEqual("12", OutilsConversions.Formater(12.0))
        Assert.AreEqual("12.50", OutilsConversions.Formater(12.5))
    End Sub

End Class

<TestClass>
Public Class TestsGeometrie

    <TestMethod>
    Public Sub PointMilieu_calcule_le_centre()
        Assert.AreEqual(New Point(5, 10), OutilsGeometrie.PointMilieu(New Point(0, 0), New Point(10, 20)))
    End Sub

    <TestMethod>
    Public Sub DistanceAuCarre_triangle_3_4_5()
        Assert.AreEqual(25, OutilsGeometrie.DistanceAuCarre(New Point(0, 0), New Point(3, 4)))
    End Sub

    <TestMethod>
    Public Sub PivoterPoint_90_degres_autour_origine()
        Dim resultat As Point = OutilsGeometrie.PivoterPoint(New Point(10, 0), New Point(0, 0), 90)
        Assert.AreEqual(New Point(0, 10), resultat)
    End Sub

End Class

<TestClass>
Public Class TestsImages

    <TestMethod>
    Public Sub DetecterExtension_reconnait_png_pdf_jpg()
        Dim png As Byte() = {&H89, &H50, &H4E, &H47, &HD, &HA, &H1A, &HA}
        Dim pdf As Byte() = {&H25, &H50, &H44, &H46, &H2D, &H31, &H2E, &H34}
        Dim jpg As Byte() = {&HFF, &HD8, &HFF, &HE0, &H0, &H10, &H4A, &H46}
        Assert.AreEqual(".png", OutilsImages.DetecterExtension(png))
        Assert.AreEqual(".pdf", OutilsImages.DetecterExtension(pdf))
        Assert.AreEqual(".jpg", OutilsImages.DetecterExtension(jpg))
    End Sub

    <TestMethod>
    Public Sub DetecterExtension_trop_court_renvoie_vide()
        Assert.AreEqual("", OutilsImages.DetecterExtension(New Byte() {1, 2, 3}))
    End Sub

End Class

<TestClass>
Public Class TestsFichiers

    <TestMethod>
    Public Sub AjouterSeparateurFinal_ajoute_le_separateur()
        Dim resultat As String = OutilsFichiers.AjouterSeparateurFinal("C:\temp")
        Assert.IsTrue(resultat.EndsWith(IO.Path.DirectorySeparatorChar))
    End Sub

    <TestMethod>
    Public Sub Combiner_concatene_les_octets()
        Dim resultat As Byte() = OutilsFichiers.Combiner(New Byte() {1, 2}, New Byte() {3, 4})
        CollectionAssert.AreEqual(New Byte() {1, 2, 3, 4}, resultat)
    End Sub

    <TestMethod>
    Public Sub RendreNomFichierValide_retire_les_caracteres_interdits()
        Dim resultat As String = OutilsFichiers.RendreNomFichierValide("a<b>c.txt")
        Assert.IsFalse(resultat.Contains("<"))
        Assert.IsFalse(resultat.Contains(">"))
    End Sub

End Class

<TestClass>
Public Class TestsReseau

    <TestMethod>
    Public Sub EstAdresseIPv4Valide_valide_et_rejette()
        Assert.IsTrue(OutilsReseau.EstAdresseIPv4Valide("192.168.1.10"))
        Assert.IsFalse(OutilsReseau.EstAdresseIPv4Valide("999.1.1.1"))
        Assert.IsFalse(OutilsReseau.EstAdresseIPv4Valide("1.2.3"))
        Assert.IsFalse(OutilsReseau.EstAdresseIPv4Valide("abc"))
    End Sub

End Class

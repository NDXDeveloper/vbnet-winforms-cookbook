' ============================================================================
'  GestionnaireRaccourcisTests.vb  -  Tests de la reconnaissance des accords.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Raccourcis

<TestClass>
Public Class GestionnaireRaccourcisTests

    <TestMethod>
    Public Sub Simple_Declenche()
        Dim g As New GestionnaireRaccourcis()
        g.Inscrire("Enregistrer", "Ctrl+S")
        Dim r = g.Appuyer("Ctrl+S")
        Assert.AreEqual(EtatTouche.Declenchee, r.Etat)
        Assert.AreEqual("Enregistrer", r.Action)
    End Sub

    <TestMethod>
    Public Sub Accord_EnAttentePuisDeclenche()
        Dim g As New GestionnaireRaccourcis()
        g.Inscrire("Tout enregistrer", "Ctrl+K, Ctrl+S")
        Assert.AreEqual(EtatTouche.EnAttente, g.Appuyer("Ctrl+K").Etat)
        Dim r = g.Appuyer("Ctrl+S")
        Assert.AreEqual(EtatTouche.Declenchee, r.Etat)
        Assert.AreEqual("Tout enregistrer", r.Action)
    End Sub

    <TestMethod>
    Public Sub Accord_FrappeInattendue_Reinitialise()
        Dim g As New GestionnaireRaccourcis()
        g.Inscrire("Tout enregistrer", "Ctrl+K, Ctrl+S")
        Assert.AreEqual(EtatTouche.EnAttente, g.Appuyer("Ctrl+K").Etat)
        Assert.AreEqual(EtatTouche.Aucun, g.Appuyer("Ctrl+X").Etat)
        Assert.IsFalse(g.AccordEnCours)
    End Sub

    <TestMethod>
    Public Sub ApresEchec_DetecteLeRaccourciSuivant()
        Dim g As New GestionnaireRaccourcis()
        g.Inscrire("Enregistrer", "Ctrl+S")
        Assert.AreEqual(EtatTouche.Aucun, g.Appuyer("Ctrl+F").Etat)   ' inconnu
        Assert.AreEqual(EtatTouche.Declenchee, g.Appuyer("Ctrl+S").Etat)
    End Sub

    <TestMethod>
    Public Sub DeuxAccords_MemePrefixe_SeDistinguent()
        Dim g As New GestionnaireRaccourcis()
        g.Inscrire("Tout enregistrer", "Ctrl+K, Ctrl+S")
        g.Inscrire("Commenter", "Ctrl+K, Ctrl+C")
        Assert.AreEqual(EtatTouche.EnAttente, g.Appuyer("Ctrl+K").Etat)
        Dim r = g.Appuyer("Ctrl+C")
        Assert.AreEqual("Commenter", r.Action)
    End Sub

    <TestMethod>
    Public Sub Inscrire_Doublon_Leve()
        Dim g As New GestionnaireRaccourcis()
        g.Inscrire("A", "Ctrl+S")
        Assert.ThrowsException(Of InvalidOperationException)(Sub() g.Inscrire("B", "Ctrl+S"))
    End Sub

    <TestMethod>
    Public Sub Inscrire_ConflitDePrefixe_Leve()
        Dim g As New GestionnaireRaccourcis()
        g.Inscrire("Court", "Ctrl+K")
        ' « Ctrl+K » est un préfixe de « Ctrl+K, Ctrl+S » -> conflit.
        Assert.ThrowsException(Of InvalidOperationException)(Sub() g.Inscrire("Long", "Ctrl+K, Ctrl+S"))
    End Sub

    <TestMethod>
    Public Sub Reinitialiser_VideLaSequence()
        Dim g As New GestionnaireRaccourcis()
        g.Inscrire("Tout enregistrer", "Ctrl+K, Ctrl+S")
        g.Appuyer("Ctrl+K")
        Assert.IsTrue(g.AccordEnCours)
        g.Reinitialiser()
        Assert.IsFalse(g.AccordEnCours)
    End Sub

End Class

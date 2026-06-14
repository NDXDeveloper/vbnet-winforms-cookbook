' ============================================================================
'  InspecteurTypeTests.vb  -  Tests de l'inspection de type (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Reflexion

<TestClass>
Public Class InspecteurTypeTests

    <TestMethod>
    Public Sub ListerProprietes_TrouveLesProprietesDArticle()
        Dim noms = InspecteurType.ListerProprietes(GetType(Article)).Select(Function(d) d.Nom).ToList()
        Assert.AreEqual(5, noms.Count)
        CollectionAssert.Contains(noms, "Reference")
        CollectionAssert.Contains(noms, "PrixHT")
        CollectionAssert.Contains(noms, "CreeLe")
    End Sub

    <TestMethod>
    Public Sub ListerEvenements_TrouveLEvenement()
        Dim evenements = InspecteurType.ListerEvenements(GetType(Article)).Select(Function(d) d.Nom).ToList()
        CollectionAssert.Contains(evenements, "PrixModifie")
    End Sub

    <TestMethod>
    Public Sub ListerProprietes_EstTriee()
        Dim noms = InspecteurType.ListerProprietes(GetType(Personne)).Select(Function(d) d.Nom).ToList()
        Dim triee = noms.OrderBy(Function(n) n).ToList()
        CollectionAssert.AreEqual(triee, noms)
    End Sub

    <TestMethod>
    Public Sub ListerTout_RegroupeLesTroisGenres()
        Dim genres = InspecteurType.ListerTout(GetType(Article)).Select(Function(d) d.Genre).Distinct().ToList()
        CollectionAssert.Contains(genres, "Propriété")
        CollectionAssert.Contains(genres, "Événement")
    End Sub

End Class

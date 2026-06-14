' ============================================================================
'  RaccourciTests.vb  -  Tests des accords (suites de combinaisons).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Raccourcis

<TestClass>
Public Class RaccourciTests

    <TestMethod>
    Public Sub Analyser_AccordVirgule()
        Dim r = Raccourci.Analyser("Ctrl+K, Ctrl+S")
        Assert.AreEqual(2, r.NombreFrappes)
        Assert.AreEqual("Ctrl+K, Ctrl+S", r.Texte)
    End Sub

    <TestMethod>
    Public Sub Analyser_AccordEspace_MemeResultat()
        Assert.AreEqual(Raccourci.Analyser("Ctrl+K, Ctrl+S"), Raccourci.Analyser("ctrl+k ctrl+s"))
    End Sub

    <TestMethod>
    Public Sub Analyser_SimpleFrappe()
        Dim r = Raccourci.Analyser("Ctrl+S")
        Assert.AreEqual(1, r.NombreFrappes)
        Assert.AreEqual("Ctrl+S", r.Texte)
    End Sub

    <TestMethod>
    Public Sub EstPrefixeDe_DetecteLePrefixe()
        Dim court = Raccourci.Analyser("Ctrl+K")
        Dim long_ = Raccourci.Analyser("Ctrl+K, Ctrl+S")
        Assert.IsTrue(court.EstPrefixeDe(long_))
        Assert.IsFalse(long_.EstPrefixeDe(court))
    End Sub

    <TestMethod>
    Public Sub EstPrefixeDe_AccordsDivergents_Faux()
        Dim a = Raccourci.Analyser("Ctrl+K, Ctrl+S")
        Dim b = Raccourci.Analyser("Ctrl+K, Ctrl+C")
        Assert.IsFalse(a.EstPrefixeDe(b))
        Assert.IsFalse(b.EstPrefixeDe(a))
    End Sub

    <TestMethod>
    Public Sub TryAnalyser_Invalide_RenvoieFalse()
        Dim r As Raccourci = Nothing
        Assert.IsFalse(Raccourci.TryAnalyser("Ctrl+, , ", r))
    End Sub

End Class

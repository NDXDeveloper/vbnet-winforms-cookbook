' ============================================================================
'  StatistiquesTexteTests.vb  -  Tests du comptage (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Editeur

<TestClass>
Public Class StatistiquesTexteTests

    <TestMethod>
    Public Sub PhraseSimple()
        Dim s = StatistiquesTexte.Analyser("Bonjour le monde")
        Assert.AreEqual(3, s.Mots)
        Assert.AreEqual(16, s.Caracteres)
        Assert.AreEqual(1, s.Lignes)
    End Sub

    <TestMethod>
    Public Sub PlusieursLignes()
        Dim s = StatistiquesTexte.Analyser("a" & vbCrLf & "b" & vbCrLf & "c")
        Assert.AreEqual(3, s.Lignes)
        Assert.AreEqual(3, s.Mots)
    End Sub

    <TestMethod>
    Public Sub EspacesMultiples_NeComptentPasDeMotsVides()
        Dim s = StatistiquesTexte.Analyser("  deux   mots  ")
        Assert.AreEqual(2, s.Mots)
        Assert.AreEqual(8, s.CaracteresSansEspaces)
    End Sub

    <TestMethod>
    Public Sub TexteVide()
        Dim s = StatistiquesTexte.Analyser("")
        Assert.AreEqual(0, s.Mots)
        Assert.AreEqual(0, s.Caracteres)
        Assert.AreEqual(0, s.Lignes)
    End Sub

End Class

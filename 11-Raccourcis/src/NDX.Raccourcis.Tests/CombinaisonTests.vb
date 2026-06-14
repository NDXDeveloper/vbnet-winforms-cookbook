' ============================================================================
'  CombinaisonTests.vb  -  Tests de l'analyse / normalisation d'une combinaison.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Raccourcis

<TestClass>
Public Class CombinaisonTests

    <TestMethod>
    Public Sub Analyser_NormaliseLaCasse()
        Assert.AreEqual("Ctrl+K", Combinaison.Analyser("ctrl+k").ToString())
    End Sub

    <TestMethod>
    Public Sub Analyser_OrdonneLesModificateurs()
        Assert.AreEqual("Ctrl+Maj+P", Combinaison.Analyser("Maj+Ctrl+P").ToString())
        Assert.AreEqual("Ctrl+Alt+Maj+A", Combinaison.Analyser("Maj+Alt+Ctrl+a").ToString())
    End Sub

    <TestMethod>
    Public Sub Analyser_ChiffresEtFonctions()
        Assert.AreEqual("Ctrl+1", Combinaison.Analyser("ctrl+1").ToString())
        Assert.AreEqual("F11", Combinaison.Analyser("F11").ToString())
    End Sub

    <TestMethod>
    Public Sub Analyser_AliasFrancais_EquivautAuNomAnglais()
        Assert.AreEqual(Combinaison.Analyser("Enter"), Combinaison.Analyser("Entrée"))
        Assert.AreEqual(Combinaison.Analyser("Escape"), Combinaison.Analyser("Échap"))
        Assert.AreEqual(Combinaison.Analyser("Shift+Delete"), Combinaison.Analyser("Maj+Suppr"))
    End Sub

    <DataTestMethod>
    <DataRow("")>
    <DataRow("Ctrl+")>
    <DataRow("Ctrl+Maj")>
    <DataRow("Ctrl+A+B")>
    <DataRow("Ctrl+ZZZ")>
    Public Sub TryAnalyser_Invalides_RenvoieFalse(ByVal texte As String)
        Dim c As Combinaison
        Assert.IsFalse(Combinaison.TryAnalyser(texte, c))
    End Sub

    <TestMethod>
    Public Sub Egalite_IndependanteDeLaSaisie()
        Assert.AreEqual(Combinaison.Analyser("Ctrl+S"), Combinaison.Analyser("ctrl+s"))
        Assert.AreEqual(Combinaison.Analyser("Ctrl+Maj+P"), Combinaison.Analyser("Maj+Ctrl+p"))
    End Sub

End Class

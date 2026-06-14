' ============================================================================
'  BoutonBasculeTests.vb  -  Tests du comportement (non visuel) de BoutonBascule.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Controles

<TestClass>
Public Class BoutonBasculeTests

    <TestMethod>
    Public Sub EtatParDefaut_EstInactif()
        Using b As New BoutonBascule()
            Assert.IsFalse(b.Actif)
        End Using
    End Sub

    <TestMethod>
    Public Sub Basculer_InverseLEtat()
        Using b As New BoutonBascule()
            b.Basculer()
            Assert.IsTrue(b.Actif)
            b.Basculer()
            Assert.IsFalse(b.Actif)
        End Using
    End Sub

    <TestMethod>
    Public Sub Changer_DeclencheLEvenement_UneFois()
        Using b As New BoutonBascule()
            Dim compte As Integer = 0
            AddHandler b.BasculeModifiee, Sub(s, e) compte += 1
            b.Actif = True
            Assert.AreEqual(1, compte)
        End Using
    End Sub

    <TestMethod>
    Public Sub AffecterMemeValeur_NeDeclenchePasLEvenement()
        Using b As New BoutonBascule()
            Dim compte As Integer = 0
            AddHandler b.BasculeModifiee, Sub(s, e) compte += 1
            b.Actif = False   ' déjà False : aucun changement
            Assert.AreEqual(0, compte)
        End Using
    End Sub

End Class

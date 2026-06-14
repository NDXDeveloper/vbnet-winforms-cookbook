' ============================================================================
'  CalculEtatTests.vb  -  Tests de la machine a etats du bouton (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.ControlesII

<TestClass>
Public Class CalculEtatTests

    <TestMethod>
    Public Sub Desactive_Prioritaire()
        Assert.AreEqual(EtatBouton.Desactive, CalculEtat.Determiner(active:=False, survol:=True, enfonce:=True))
    End Sub

    <TestMethod>
    Public Sub Enfonce_AvantSurvol()
        Assert.AreEqual(EtatBouton.Enfonce, CalculEtat.Determiner(active:=True, survol:=True, enfonce:=True))
    End Sub

    <TestMethod>
    Public Sub Survol_SiActifEtSurvole()
        Assert.AreEqual(EtatBouton.Survol, CalculEtat.Determiner(active:=True, survol:=True, enfonce:=False))
    End Sub

    <TestMethod>
    Public Sub Normal_ParDefaut()
        Assert.AreEqual(EtatBouton.Normal, CalculEtat.Determiner(active:=True, survol:=False, enfonce:=False))
    End Sub

End Class

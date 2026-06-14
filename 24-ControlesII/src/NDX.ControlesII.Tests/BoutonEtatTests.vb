' ============================================================================
'  BoutonEtatTests.vb  -  Tests (non visuels) du BoutonEtat.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.ControlesII

<TestClass>
Public Class BoutonEtatTests

    <TestMethod>
    Public Sub EtatParDefaut_EstNormal()
        Using b As New BoutonEtat()
            Assert.AreEqual(EtatBouton.Normal, b.EtatCourant)
        End Using
    End Sub

    <TestMethod>
    Public Sub Desactive_EtatEstDesactive()
        Using b As New BoutonEtat()
            b.Enabled = False
            Assert.AreEqual(EtatBouton.Desactive, b.EtatCourant)
        End Using
    End Sub

End Class

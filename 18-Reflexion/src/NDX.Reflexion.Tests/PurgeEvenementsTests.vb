' ============================================================================
'  PurgeEvenementsTests.vb  -  Tests de la purge d'abonnes (reflexion).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Reflexion

<TestClass>
Public Class PurgeEvenementsTests

    Private _compte As Integer

    Private Sub Gestionnaire(ByVal sender As Object, ByVal e As EventArgs)
        _compte += 1
    End Sub

    <TestMethod>
    Public Sub RetirerTous_SupprimeLesAbonnes()
        _compte = 0
        Dim a As New Article()
        AddHandler a.PrixModifie, AddressOf Gestionnaire
        AddHandler a.PrixModifie, AddressOf Gestionnaire

        a.DeclencherPrixModifie()
        Assert.AreEqual(2, _compte, "Deux abonnés : le compteur monte de 2.")

        Assert.IsTrue(PurgeEvenements.RetirerTous(a, "PrixModifie"))
        a.DeclencherPrixModifie()
        Assert.AreEqual(2, _compte, "Après purge, plus aucun abonné : le compteur ne bouge plus.")
    End Sub

    <TestMethod>
    Public Sub RetirerTous_EvenementInconnu_RendFaux()
        Assert.IsFalse(PurgeEvenements.RetirerTous(New Article(), "EvenementQuiNExistePas"))
    End Sub

End Class

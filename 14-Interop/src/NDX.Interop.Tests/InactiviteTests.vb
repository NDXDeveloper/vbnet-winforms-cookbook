' ============================================================================
'  InactiviteTests.vb  -  Tests du calcul d'inactivite (pur) + appel reel.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Interop

<TestClass>
Public Class InactiviteTests

    <TestMethod>
    Public Sub MillisecondesInactif_CasNominal()
        Assert.AreEqual(800L, Inactivite.MillisecondesInactif(1000, 200))
    End Sub

    <TestMethod>
    Public Sub MillisecondesInactif_Egal_RendZero()
        Assert.AreEqual(0L, Inactivite.MillisecondesInactif(12345, 12345))
    End Sub

    <TestMethod>
    Public Sub MillisecondesInactif_GereLeRepli32Bits()
        ' Le compteur vient de repasser par zéro : tick actuel = 50,
        ' dernière entrée = 6 ms avant le repli (2^32 - 6 = 4294967290).
        Assert.AreEqual(56L, Inactivite.MillisecondesInactif(50, 4294967290L))
    End Sub

    <TestMethod>
    Public Sub Duree_AppelReel_RendValeurNonNegative()
        ' Appel système réel (Windows) : on vérifie surtout que le P/Invoke aboutit.
        Assert.IsTrue(Inactivite.Duree() >= TimeSpan.Zero)
    End Sub

End Class

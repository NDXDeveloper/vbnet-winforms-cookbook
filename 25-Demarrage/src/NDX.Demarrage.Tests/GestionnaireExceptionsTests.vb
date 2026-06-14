' ============================================================================
'  GestionnaireExceptionsTests.vb  -  Tests de la description d'exception (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Demarrage

<TestClass>
Public Class GestionnaireExceptionsTests

    <TestMethod>
    Public Sub Decrire_TypeEtMessage()
        Dim texte = GestionnaireExceptions.Decrire(New InvalidOperationException("oups"))
        Assert.AreEqual("InvalidOperationException : oups", texte)
    End Sub

    <TestMethod>
    Public Sub Decrire_Null_RendTexteNeutre()
        Assert.AreEqual("(aucune exception)", GestionnaireExceptions.Decrire(Nothing))
    End Sub

End Class

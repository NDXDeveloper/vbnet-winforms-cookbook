' ============================================================================
'  CopieurProprietesTests.vb  -  Tests de la copie de proprietes (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Reflexion

<TestClass>
Public Class CopieurProprietesTests

    <TestMethod>
    Public Sub Copier_MemeType_RecopieToutesLesValeurs()
        Dim source As New Personne() With {.Nom = "Martin", .Prenom = "Alice", .Age = 30, .Courriel = "a@exemple.test"}
        Dim destination As New Personne()
        Dim n As Integer = CopieurProprietes.Copier(source, destination)
        Assert.AreEqual(4, n)
        Assert.AreEqual("Martin", destination.Nom)
        Assert.AreEqual("Alice", destination.Prenom)
        Assert.AreEqual(30, destination.Age)
        Assert.AreEqual("a@exemple.test", destination.Courriel)
    End Sub

    <TestMethod>
    Public Sub Copier_TypesSansProprietesCommunes_NeCopieRien()
        Dim source As New Article() With {.Reference = "R1", .Designation = "Vis"}
        Dim destination As New Personne()
        Assert.AreEqual(0, CopieurProprietes.Copier(source, destination))
    End Sub

    <TestMethod>
    Public Sub Copier_Null_Leve()
        Assert.ThrowsException(Of ArgumentNullException)(Function() CopieurProprietes.Copier(Nothing, New Personne()))
    End Sub

End Class

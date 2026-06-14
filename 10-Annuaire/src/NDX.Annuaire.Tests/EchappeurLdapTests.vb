' ============================================================================
'  EchappeurLdapTests.vb  -  Tests de l'echappement de filtre LDAP (RFC 4515).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Annuaire

<TestClass>
Public Class EchappeurLdapTests

    <TestMethod>
    Public Sub TexteSimple_InchangeSiPasDeCaractereSpecial()
        Assert.AreEqual("alice", EchappeurLdap.EchapperFiltre("alice"))
    End Sub

    <DataTestMethod>
    <DataRow("*", "\2a")>
    <DataRow("(", "\28")>
    <DataRow(")", "\29")>
    <DataRow("\", "\5c")>
    Public Sub CaracteresSpeciaux_SontEchappes(ByVal entree As String, ByVal attendu As String)
        Assert.AreEqual(attendu, EchappeurLdap.EchapperFiltre(entree))
    End Sub

    <TestMethod>
    Public Sub TentativeInjection_EstNeutralisee()
        ' Sans échappement, "*)(uid=*" ouvrirait un filtre arbitraire.
        Dim resultat As String = EchappeurLdap.EchapperFiltre("*)(uid=*")
        Assert.AreEqual("\2a\29\28uid=\2a", resultat)
        Assert.IsFalse(resultat.Contains("*"), "Aucune étoile ne doit subsister.")
    End Sub

    <TestMethod>
    Public Sub OctetNul_EstEchappe()
        Assert.AreEqual("a\00b", EchappeurLdap.EchapperFiltre("a" & ChrW(0) & "b"))
    End Sub

    <TestMethod>
    Public Sub Null_RendChaineVide()
        Assert.AreEqual("", EchappeurLdap.EchapperFiltre(Nothing))
    End Sub

End Class

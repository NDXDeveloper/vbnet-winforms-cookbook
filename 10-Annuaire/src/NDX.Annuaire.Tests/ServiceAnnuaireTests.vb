' ============================================================================
'  ServiceAnnuaireTests.vb  -  Test d'integration (annuaire OpenLDAP).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur OpenLDAP demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Annuaire

<TestClass>
Public Class ServiceAnnuaireTests

    Private Shared Sub ExigerAnnuaire()
        Dim message As String = ""
        If Not ServiceAnnuaire.TesterConnexion(message) Then
            Assert.Inconclusive("Annuaire indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Authentifier_BonsIdentifiants_Reussit()
        ExigerAnnuaire()
        Dim r = ServiceAnnuaire.Authentifier("alice", "alice_pwd")
        Assert.IsTrue(r.Reussite, r.Message)
        Assert.AreEqual("alice", r.Compte.Identifiant)
    End Sub

    <TestMethod>
    Public Sub Authentifier_MauvaisMotDePasse_Echoue()
        ExigerAnnuaire()
        Dim r = ServiceAnnuaire.Authentifier("alice", "mauvais_mot_de_passe")
        Assert.IsFalse(r.Reussite)
    End Sub

    <TestMethod>
    Public Sub Authentifier_ChampsVides_Echoue()
        ' Pas besoin de l'annuaire : validation locale.
        Assert.IsFalse(ServiceAnnuaire.Authentifier("", "").Reussite)
    End Sub

    <TestMethod>
    Public Sub ListerUtilisateurs_ContientLesComptesAmorces()
        ExigerAnnuaire()
        Dim comptes = ServiceAnnuaire.ListerUtilisateurs()
        Assert.IsTrue(comptes.Any(Function(u) u.Identifiant = "alice"), "alice doit être présente.")
        Assert.IsTrue(comptes.Any(Function(u) u.Identifiant = "bob"), "bob doit être présent.")
    End Sub

End Class

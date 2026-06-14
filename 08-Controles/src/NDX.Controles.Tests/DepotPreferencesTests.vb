' ============================================================================
'  DepotPreferencesTests.vb  -  Test d'integration (base "preferences").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Controles

<TestClass>
Public Class DepotPreferencesTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotPreferences.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Ecrire_Puis_Lire_RestitueLaValeur()
        ExigerBase()
        Dim cle As String = "test.aller_retour"
        DepotPreferences.Ecrire(cle, "valeur-A")
        Assert.AreEqual("valeur-A", DepotPreferences.Lire(cle))
    End Sub

    <TestMethod>
    Public Sub Ecrire_DeuxFois_MetAJour_SansDoublon()
        ExigerBase()
        Dim cle As String = "test.upsert"
        DepotPreferences.Ecrire(cle, "premiere")
        DepotPreferences.Ecrire(cle, "seconde")
        Assert.AreEqual("seconde", DepotPreferences.Lire(cle))
    End Sub

    <TestMethod>
    Public Sub Lire_CleInconnue_RenvoieLeDefaut()
        ExigerBase()
        Assert.AreEqual("(absente)", DepotPreferences.Lire("test.cle_qui_n_existe_pas", "(absente)"))
    End Sub

End Class

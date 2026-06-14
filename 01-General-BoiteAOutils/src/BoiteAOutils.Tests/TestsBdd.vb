Imports System.Data
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BoiteAOutils

''' <summary>
''' Initialisation globale de l'assembly de tests.
''' </summary>
<TestClass>
Public Class InitialisationTests
    ''' <summary>
    ''' Desactive la journalisation des exceptions en base pendant les tests :
    ''' les tests de logique pure ne doivent ni dependre de la base, ni subir le
    ''' delai d'attente d'une connexion absente.
    ''' </summary>
    <AssemblyInitialize>
    Public Shared Sub Initialiser(ByVal contexte As TestContext)
        GestionExceptions.JournaliserEnBase = False
    End Sub
End Class

''' <summary>
''' Tests d'integration de la couche d'acces aux donnees.
''' </summary>
''' <remarks>
''' Ils necessitent le conteneur MariaDB demarre (docker compose up -d). En son
''' absence, chaque test se termine en <c>Inconclusive</c> plutot qu'en echec.
''' </remarks>
<TestClass>
Public Class TestsBdd

    ''' <summary>Verifie la connexion ; ignore le test (Inconclusive) si la base est absente.</summary>
    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not AccesDonnees.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (demarrez 'docker compose up -d') : " & message)
        End If
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub TesterConnexion_renvoie_la_version_du_serveur()
        ExigerBase()
        Dim message As String = ""
        Assert.IsTrue(AccesDonnees.TesterConnexion(message))
        StringAssert.Contains(message, "OK")
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub IndicateursFiche_retourne_une_ligne_avec_les_colonnes_calculees()
        ExigerBase()
        Dim dt As DataTable = AccesDonnees.IndicateursFiche(1)
        Assert.IsNotNull(dt)
        Assert.AreEqual(1, dt.Rows.Count)
        Assert.IsTrue(dt.Columns.Contains("Poids (kg)"))
        Assert.IsTrue(dt.Columns.Contains("% Chutes"))
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub StatistiquesClients_retourne_des_lignes()
        ExigerBase()
        Dim dt As DataTable = AccesDonnees.StatistiquesClients(New DateTime(2025, 1, 1))
        Assert.IsNotNull(dt)
        Assert.IsTrue(dt.Rows.Count > 0)
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub VerifierArticles_signale_les_identifiants_absents()
        ExigerBase()
        Dim problemes As New List(Of Integer)()
        Dim tousPresents As Boolean = AccesDonnees.VerifierArticles(New List(Of Integer) From {1, 2, 999}, problemes)
        Assert.IsFalse(tousPresents)
        Assert.IsTrue(problemes.Contains(999))
        Assert.IsFalse(problemes.Contains(1))
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub LogErreur_insere_et_renvoie_un_identifiant()
        ExigerBase()
        Dim id As Integer = AccesDonnees.LogErreur("Test d'integration.", "tests@etabli")
        Assert.IsTrue(id > 0, "LogErreur doit renvoyer l'identifiant insere (> 0).")
    End Sub

End Class

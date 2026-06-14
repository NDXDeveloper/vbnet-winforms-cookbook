' ============================================================================
'  TestsPersistance.vb  -  Tests d'integration du puits base de donnees.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Journalisation

''' <summary>Necessitent le conteneur MariaDB demarre ; sinon Inconclusive.</summary>
<TestClass>
Public Class TestsPersistance

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not PuitsBase.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (docker compose up -d) : " & message)
        End If
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub TesterConnexion_renvoie_la_version()
        ExigerBase()
        Dim m As String = ""
        Assert.IsTrue(PuitsBase.TesterConnexion(m))
        StringAssert.Contains(m, "OK")
    End Sub

    <TestMethod>
    <TestCategory("Integration")>
    Public Sub Puits_base_ecrit_puis_relit()
        ExigerBase()
        Dim cible As String = "ITg-" & Guid.NewGuid().ToString("N").Substring(0, 8)
        Using puits As New PuitsBase()
            puits.Ecrire(New EntreeJournal(Niveau.Erreur, cible, "Entree de test d'integration."))
        End Using
        Dim table As DataTable = PuitsBase.Lire(Niveau.Debogage, 500)
        Assert.IsNotNull(table)
        Dim trouve As Boolean = False
        For Each ligne As DataRow In table.Rows
            If String.Equals(Convert.ToString(ligne("Categorie")), cible) Then trouve = True : Exit For
        Next
        Assert.IsTrue(trouve, "L'entree ecrite doit etre relue depuis la base.")
    End Sub

End Class

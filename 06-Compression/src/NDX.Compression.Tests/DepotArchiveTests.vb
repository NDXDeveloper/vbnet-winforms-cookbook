' ============================================================================
'  DepotArchiveTests.vb  -  Test d'integration (base "archive" via Docker).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Ce test requiert le conteneur MariaDB demarre (docker compose up -d).
'  S'il est indisponible, le test est marque "Inconclusive" plutot qu'en echec.
' ============================================================================

Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Compression

<TestClass>
Public Class DepotArchiveTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotArchive.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Enregistrer_Puis_Recharger_RestitueLesDonnees()
        ExigerBase()
        Dim source As String = "Contenu de test — " & New String("k"c, 2000)
        Dim donnees As Byte() = Encoding.UTF8.GetBytes(source)

        Dim id As Integer = DepotArchive.Enregistrer("test-integration.txt", donnees, AlgorithmeCompression.GZip)
        Assert.IsTrue(id > 0, "L'identifiant inséré doit être positif.")

        Dim restitue As Byte() = DepotArchive.Recharger(id)
        Assert.AreEqual(source, Encoding.UTF8.GetString(restitue))
    End Sub

    <TestMethod>
    Public Sub Lister_RenvoieLesColonnesAttendues()
        ExigerBase()
        DepotArchive.Enregistrer("pour-liste.txt", Encoding.UTF8.GetBytes("abc"), AlgorithmeCompression.Deflate)
        Dim table = DepotArchive.Lister()
        Assert.IsTrue(table.Rows.Count > 0, "La liste ne devrait pas être vide après insertion.")
        Assert.IsTrue(table.Columns.Contains("Gain %"), "La colonne de gain doit être présente.")
    End Sub

End Class

' ============================================================================
'  DepotDocumentTests.vb  -  Test d'integration (base "redaction").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Editeur

<TestClass>
Public Class DepotDocumentTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotDocument.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Enregistrer_Puis_Recharger_RestitueLeRtf()
        ExigerBase()
        Dim rtf As String = "{\rtf1\ansi Bonjour {\b en gras} et normal.}"
        Dim id As Integer = DepotDocument.Enregistrer("Test " & DateTime.UtcNow.Ticks.ToString(), rtf)
        Assert.IsTrue(id > 0)
        Assert.AreEqual(rtf, DepotDocument.Recharger(id))
    End Sub

End Class

' ============================================================================
'  CanalCommandeTests.vb  -  Tests de l'IPC par tube nomme (local).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.InstanceUnique

<TestClass>
Public Class CanalCommandeTests

    <TestMethod>
    Public Sub Envoyer_Puis_Recevoir_TransmetLeMessage()
        Dim nom As String = "ndx-canal-test-" & Guid.NewGuid().ToString("N")
        Dim reception As Task(Of String) = Task.Run(Function() CanalCommande.RecevoirUn(nom, 5000))
        Thread.Sleep(200)   ' laisser le serveur se mettre en écoute

        Assert.IsTrue(CanalCommande.Envoyer(nom, "bonjour le monde"), "L'envoi doit réussir.")
        Assert.IsTrue(reception.Wait(6000), "La réception doit aboutir avant le délai.")
        Assert.AreEqual("bonjour le monde", reception.Result)
    End Sub

    <TestMethod>
    Public Sub BoutABout_CommandeEncodee_EstRestituee()
        Dim nom As String = "ndx-canal-test-" & Guid.NewGuid().ToString("N")
        Dim args As New List(Of String) From {"--ouvrir", "a|b", "c\d"}
        Dim reception As Task(Of String) = Task.Run(Function() CanalCommande.RecevoirUn(nom, 5000))
        Thread.Sleep(200)

        Assert.IsTrue(CanalCommande.Envoyer(nom, Commande.Encoder(args)))
        Assert.IsTrue(reception.Wait(6000))
        CollectionAssert.AreEqual(args, Commande.Decoder(reception.Result))
    End Sub

    <TestMethod>
    Public Sub Envoyer_PersonneNEcoute_RendFaux()
        Dim nom As String = "ndx-canal-absent-" & Guid.NewGuid().ToString("N")
        Assert.IsFalse(CanalCommande.Envoyer(nom, "test", timeoutMs:=400))
    End Sub

End Class

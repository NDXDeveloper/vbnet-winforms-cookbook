' ============================================================================
'  CommandeTests.vb  -  Tests de l'encodage / decodage (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.InstanceUnique

<TestClass>
Public Class CommandeTests

    <TestMethod>
    Public Sub AllerRetour_PreserveLesArguments()
        Dim args As New List(Of String) From {"--ouvrir", "fichier avec espaces.txt", "a|b", "c\d", "ligne1" & vbCrLf & "ligne2", "éàç"}
        Dim restitue As List(Of String) = Commande.Decoder(Commande.Encoder(args))
        CollectionAssert.AreEqual(args, restitue)
    End Sub

    <TestMethod>
    Public Sub Encoder_SeparateurEchappe_RestitueDeuxArguments()
        Dim ligne As String = Commande.Encoder(New String() {"x|y", "z"})
        Dim args As List(Of String) = Commande.Decoder(ligne)
        Assert.AreEqual(2, args.Count)
        Assert.AreEqual("x|y", args(0))
        Assert.AreEqual("z", args(1))
    End Sub

    <TestMethod>
    Public Sub Encoder_Null_RendChaineVide()
        Assert.AreEqual("", Commande.Encoder(Nothing))
    End Sub

    <TestMethod>
    Public Sub Decoder_ChaineVide_RendListeVide()
        Assert.AreEqual(0, Commande.Decoder("").Count)
    End Sub

End Class

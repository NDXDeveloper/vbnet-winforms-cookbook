' ============================================================================
'  SequenceDemarrageTests.vb  -  Tests de la sequence d'etapes (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Demarrage

<TestClass>
Public Class SequenceDemarrageTests

    Private Shared Function Etapes(ByVal ParamArray succes As Boolean()) As List(Of EtapeDemarrage)
        Dim liste As New List(Of EtapeDemarrage)()
        For i As Integer = 0 To succes.Length - 1
            Dim ok As Boolean = succes(i)
            liste.Add(New EtapeDemarrage("Étape " & (i + 1).ToString(), Function() ok))
        Next
        Return liste
    End Function

    <TestMethod>
    Public Sub ToutesReussies()
        Dim r = SequenceDemarrage.Executer(Etapes(True, True, True))
        Assert.AreEqual(3, r.Count)
        Assert.IsTrue(SequenceDemarrage.ToutEstReussi(r))
    End Sub

    <TestMethod>
    Public Sub Echec_ArreteLaSequence()
        Dim r = SequenceDemarrage.Executer(Etapes(True, False, True), arreterAuPremierEchec:=True)
        Assert.AreEqual(2, r.Count, "La séquence s'arrête après l'échec.")
        Assert.IsFalse(r(1).Reussi)
    End Sub

    <TestMethod>
    Public Sub Echec_PoursuiteSiDemande()
        Dim r = SequenceDemarrage.Executer(Etapes(True, False, True), arreterAuPremierEchec:=False)
        Assert.AreEqual(3, r.Count, "Toutes les étapes sont exécutées malgré l'échec.")
        Assert.IsFalse(SequenceDemarrage.ToutEstReussi(r))
    End Sub

    <TestMethod>
    Public Sub Exception_DansUneEtape_EstCapturee()
        Dim etapes As New List(Of EtapeDemarrage) From {
            New EtapeDemarrage("Qui échoue", Function() As Boolean
                                                 Throw New InvalidOperationException("boum")
                                             End Function)}
        Dim r = SequenceDemarrage.Executer(etapes)
        Assert.AreEqual(1, r.Count)
        Assert.IsFalse(r(0).Reussi)
        Assert.AreEqual("boum", r(0).Message)
    End Sub

End Class

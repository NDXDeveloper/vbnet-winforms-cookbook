' ============================================================================
'  ConstructeurArbreTests.vb  -  Tests de la reconstruction d'arbre (pur).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Arborescence

<TestClass>
Public Class ConstructeurArbreTests

    Private Shared Function N(ByVal id As Integer, ByVal parent As Integer?, ByVal libelle As String) As Noeud
        Return New Noeud() With {.Id = id, .ParentId = parent, .Libelle = libelle}
    End Function

    Private Shared Function ArbreExemple() As List(Of Noeud)
        Return New List(Of Noeud) From {
            N(1, Nothing, "Racine"),
            N(2, 1, "A"),
            N(3, 1, "B"),
            N(4, 2, "A1")}
    End Function

    <TestMethod>
    Public Sub Construire_RattacheLesEnfants()
        Dim racines = ConstructeurArbre.Construire(ArbreExemple())
        Assert.AreEqual(1, racines.Count, "Une seule racine.")
        Dim racine = racines(0)
        Assert.AreEqual(2, racine.Enfants.Count, "La racine a deux enfants (A, B).")
        Dim a = racine.Enfants.First(Function(x) x.Libelle = "A")
        Assert.AreEqual(1, a.Enfants.Count, "A a un enfant (A1).")
    End Sub

    <TestMethod>
    Public Sub NombreDescendants_CompteToutLArbre()
        Dim racine = ConstructeurArbre.Construire(ArbreExemple())(0)
        Assert.AreEqual(3, racine.NombreDescendants, "A, B et A1 = 3 descendants.")
    End Sub

    <TestMethod>
    Public Sub Construire_OrphelinDevientRacine()
        Dim plats As New List(Of Noeud) From {N(1, Nothing, "Racine"), N(2, 99, "Orphelin")}
        Dim racines = ConstructeurArbre.Construire(plats)
        Assert.AreEqual(2, racines.Count, "Un nœud dont le parent est absent devient une racine.")
    End Sub

    <TestMethod>
    Public Sub Construire_ListeVide_RendForetVide()
        Assert.AreEqual(0, ConstructeurArbre.Construire(New List(Of Noeud)()).Count)
    End Sub

End Class

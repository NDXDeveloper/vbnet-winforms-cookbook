' ============================================================================
'  ClasseurXlsxTests.vb  -  Tests d'aller-retour .xlsx (sans Office).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Office

<TestClass>
Public Class ClasseurXlsxTests

    <TestMethod>
    Public Sub AllerRetour_ConserveLesValeurs()
        Dim grille As New List(Of String())()
        grille.Add(New String() {"Produit", "Quantité"})
        grille.Add(New String() {"Café", "12"})
        grille.Add(New String() {"a&b", "<x>"})   ' caractères à échapper en XML

        Dim relu As List(Of String())
        Using ms As New MemoryStream()
            ClasseurXlsx.Ecrire(ms, grille)
            ms.Position = 0
            relu = ClasseurXlsx.Lire(ms)
        End Using

        Assert.AreEqual(3, relu.Count)
        Assert.AreEqual("Produit", relu(0)(0))
        Assert.AreEqual("Café", relu(1)(0))
        Assert.AreEqual("12", relu(1)(1))
        Assert.AreEqual("a&b", relu(2)(0))
        Assert.AreEqual("<x>", relu(2)(1))
    End Sub

    <TestMethod>
    Public Sub Ecrire_ProduitUneArchiveZip()
        Dim grille As New List(Of String()) From {New String() {"x"}}
        Using ms As New MemoryStream()
            ClasseurXlsx.Ecrire(ms, grille)
            Dim octets = ms.ToArray()
            ' Signature ZIP : 'P' 'K' (0x50 0x4B).
            Assert.AreEqual(CByte(&H50), octets(0))
            Assert.AreEqual(CByte(&H4B), octets(1))
        End Using
    End Sub

End Class

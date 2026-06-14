' ============================================================================
'  SerieDonneesTests.vb  -  Tests de la serie de donnees.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Graphiques

<TestClass>
Public Class SerieDonneesTests

    <TestMethod>
    Public Sub Ajouter_RemplitLibellesEtValeurs()
        Dim serie As New SerieDonnees()
        serie.Ajouter("Jan", 12)
        serie.Ajouter("Fév", 19)
        Assert.AreEqual(2, serie.Nombre)
        Assert.AreEqual("Jan", serie.Libelles(0))
        Assert.AreEqual(19.0, serie.Valeurs(1), 0.0001)
    End Sub

    <TestMethod>
    Public Sub SerieVide_NombreZero()
        Assert.AreEqual(0, New SerieDonnees().Nombre)
    End Sub

End Class

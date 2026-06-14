' ============================================================================
'  ServiceMiseAJourTests.vb  -  Tests de la logique de detection (sans base).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.MiseAJour

<TestClass>
Public Class ServiceMiseAJourTests

    Private Shared Function Manifeste() As List(Of Publication)
        Return New List(Of Publication) From {
            New Publication() With {.Version = VersionSemantique.Analyser("1.2.0"), .Obligatoire = False},
            New Publication() With {.Version = VersionSemantique.Analyser("1.3.0"), .Obligatoire = True},
            New Publication() With {.Version = VersionSemantique.Analyser("1.10.0"), .Obligatoire = False}}
    End Function

    <TestMethod>
    Public Sub RechercherDerniere_RenvoieLaPlusRecenteSuperieure()
        Dim p = ServiceMiseAJour.RechercherDerniere(VersionSemantique.Analyser("1.2.0"), Manifeste())
        Assert.IsNotNull(p)
        Assert.AreEqual("1.10.0", p.Version.ToString())
    End Sub

    <TestMethod>
    Public Sub RechercherDerniere_AJour_RenvoieNothing()
        Dim p = ServiceMiseAJour.RechercherDerniere(VersionSemantique.Analyser("1.10.0"), Manifeste())
        Assert.IsNull(p)
    End Sub

    <TestMethod>
    Public Sub MiseAJourObligatoire_DetecteVersionImposee()
        ' Depuis 1.2.0, la 1.3.0 obligatoire est en attente.
        Assert.IsTrue(ServiceMiseAJour.MiseAJourObligatoireEnAttente(VersionSemantique.Analyser("1.2.0"), Manifeste()))
        ' Depuis 1.5.0, plus aucune obligatoire au-dessus.
        Assert.IsFalse(ServiceMiseAJour.MiseAJourObligatoireEnAttente(VersionSemantique.Analyser("1.5.0"), Manifeste()))
    End Sub

    <TestMethod>
    Public Sub VerifierIntegrite_BonneEmpreinte_Vrai()
        Dim paquet As Byte() = Encoding.UTF8.GetBytes("contenu du paquet")
        Dim emp As String = ServiceMiseAJour.CalculerEmpreinte(paquet)
        Assert.IsTrue(ServiceMiseAJour.VerifierIntegrite(paquet, emp))
        Assert.IsTrue(ServiceMiseAJour.VerifierIntegrite(paquet, emp.ToUpperInvariant()), "La comparaison doit être insensible à la casse.")
    End Sub

    <TestMethod>
    Public Sub VerifierIntegrite_MauvaiseEmpreinte_Faux()
        Dim paquet As Byte() = Encoding.UTF8.GetBytes("contenu du paquet")
        Assert.IsFalse(ServiceMiseAJour.VerifierIntegrite(paquet, "00ff00ff"))
    End Sub

    <TestMethod>
    Public Sub CalculerEmpreinte_TableauVide_ValeurConnue()
        ' SHA-256 du message vide (valeur de référence).
        Assert.AreEqual("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                        ServiceMiseAJour.CalculerEmpreinte(New Byte() {}))
    End Sub

End Class

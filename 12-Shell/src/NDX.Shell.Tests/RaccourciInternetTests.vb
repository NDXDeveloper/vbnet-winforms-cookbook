' ============================================================================
'  RaccourciInternetTests.vb  -  Tests du raccourci .url (pur, sans disque).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Shell

<TestClass>
Public Class RaccourciInternetTests

    <TestMethod>
    Public Sub GenererContenu_ContientSectionEtUrl()
        Dim contenu = RaccourciInternet.GenererContenu("https://exemple.test/page")
        Assert.IsTrue(contenu.Contains("[InternetShortcut]"), "section manquante")
        Assert.IsTrue(contenu.Contains("URL=https://exemple.test/page"), "clé URL manquante")
    End Sub

    <TestMethod>
    Public Sub LireUrl_ExtraitLAdresse()
        Dim contenu = "[InternetShortcut]" & vbCrLf & "URL=https://exemple.test" & vbCrLf
        Assert.AreEqual("https://exemple.test", RaccourciInternet.LireUrl(contenu))
    End Sub

    <TestMethod>
    Public Sub AllerRetour_ConserveLUrl()
        Dim url = "https://exemple.test/chemin?x=1&y=2"
        Assert.AreEqual(url, RaccourciInternet.LireUrl(RaccourciInternet.GenererContenu(url)))
    End Sub

    <TestMethod>
    Public Sub LireUrl_CleInsensibleALaCasse()
        Dim contenu = "[InternetShortcut]" & vbCrLf & "url=https://exemple.test" & vbCrLf
        Assert.AreEqual("https://exemple.test", RaccourciInternet.LireUrl(contenu))
    End Sub

    <TestMethod>
    Public Sub LireUrl_IgnoreLesAutresCles()
        Dim contenu = "[InternetShortcut]" & vbCrLf & "IconIndex=0" & vbCrLf & "URL=https://exemple.test" & vbCrLf
        Assert.AreEqual("https://exemple.test", RaccourciInternet.LireUrl(contenu))
    End Sub

    <TestMethod>
    Public Sub LireUrl_SansUrl_Leve()
        Assert.ThrowsException(Of FormatException)(Function() RaccourciInternet.LireUrl("[InternetShortcut]" & vbCrLf))
    End Sub

    <TestMethod>
    Public Sub GenererContenu_UrlVide_Leve()
        Assert.ThrowsException(Of ArgumentException)(Function() RaccourciInternet.GenererContenu("   "))
    End Sub

End Class

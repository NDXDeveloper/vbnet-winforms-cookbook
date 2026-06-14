' ============================================================================
'  DepotThemeTests.vb  -  Test d'integration (base "themes").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports System.Drawing
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.FormBase

<TestClass>
Public Class DepotThemeTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotTheme.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Enregistrer_Puis_Lister_RestitueLesCouleurs()
        ExigerBase()
        Dim nom As String = "Test-" & DateTime.UtcNow.Ticks.ToString()
        Dim original As New Theme(nom, Color.FromArgb(18, 52, 86), Color.White, Color.FromArgb(124, 77, 255))
        DepotTheme.Enregistrer(original)

        Dim relu = DepotTheme.Lister().FirstOrDefault(Function(t) t.Nom = nom)
        Assert.IsNotNull(relu, "Le thème enregistré doit être relu.")
        Assert.AreEqual(original.Fond.ToArgb(), relu.Fond.ToArgb(), "La couleur de fond doit être conservée (via hexadécimal).")
        Assert.AreEqual(original.Accent.ToArgb(), relu.Accent.ToArgb())
    End Sub

End Class

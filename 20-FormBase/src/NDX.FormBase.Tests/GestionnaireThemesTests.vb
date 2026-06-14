' ============================================================================
'  GestionnaireThemesTests.vb  -  Tests de l'application recursive d'un theme.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Windows.Forms
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.FormBase

<TestClass>
Public Class GestionnaireThemesTests

    <TestMethod>
    Public Sub AppliquerSur_PropageAuxEnfants()
        Using parent As New Panel()
            Dim enfant As New Label()
            parent.Controls.Add(enfant)

            GestionnaireThemes.AppliquerSur(parent, GestionnaireThemes.Sombre)

            Assert.AreEqual(GestionnaireThemes.Sombre.Fond, parent.BackColor)
            Assert.AreEqual(GestionnaireThemes.Sombre.Fond, enfant.BackColor, "Le thème doit être propagé à l'enfant.")
            Assert.AreEqual(GestionnaireThemes.Sombre.Texte, enfant.ForeColor)
        End Using
    End Sub

    <TestMethod>
    Public Sub Predefinis_ContientClairEtSombre()
        Dim noms = GestionnaireThemes.Predefinis().ConvertAll(Function(t) t.Nom)
        Assert.AreEqual(2, noms.Count)
        CollectionAssert.Contains(noms, "Clair")
        CollectionAssert.Contains(noms, "Sombre")
    End Sub

End Class

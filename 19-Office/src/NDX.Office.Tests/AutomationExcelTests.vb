' ============================================================================
'  AutomationExcelTests.vb  -  Tests de l'automation Excel (COM, optionnelle).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Excel n'etant pas forcement installe, l'export est marque "Inconclusive"
'  lorsqu'Excel est absent (la voie « sans Office » reste, elle, testee).
' ============================================================================

Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Office

<TestClass>
Public Class AutomationExcelTests

    <TestMethod>
    Public Sub EstDisponible_NeLevePas()
        Dim dispo As Boolean = AutomationExcel.EstDisponible()
        Assert.IsTrue(dispo OrElse Not dispo, "EstDisponible doit répondre sans exception.")
    End Sub

    <TestMethod>
    Public Sub Exporter_SiExcelDisponible_CreeLeFichier()
        If Not AutomationExcel.EstDisponible() Then
            Assert.Inconclusive("Microsoft Excel n'est pas installé : export COM non testé.")
        End If
        Dim chemin As String = Path.Combine(Path.GetTempPath(), "ndx-office-test-" & Guid.NewGuid().ToString("N") & ".xlsx")
        Dim grille As New List(Of String()) From {New String() {"A", "B"}, New String() {"1", "2"}}
        Try
            AutomationExcel.Exporter(chemin, grille)
            Assert.IsTrue(File.Exists(chemin), "Le fichier .xlsx doit avoir été créé par Excel.")
        Finally
            Try
                If File.Exists(chemin) Then File.Delete(chemin)
            Catch
            End Try
        End Try
    End Sub

End Class

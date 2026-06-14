' ============================================================================
'  TestsJournalisation.vb  -  Tests unitaires du journal et des puits (hors base).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Journalisation

<TestClass>
Public Class TestsJournalisation

    <TestMethod>
    Public Sub Memoire_capture_les_entrees()
        Dim memoire As New PuitsMemoire()
        Dim journal As New Journal()
        journal.AjouterPuits(memoire)
        journal.Information("Test", "un")
        journal.Information("Test", "deux")
        Assert.AreEqual(2, memoire.Instantane().Length)
    End Sub

    <TestMethod>
    Public Sub NiveauMinimal_filtre_les_entrees_moins_graves()
        Dim memoire As New PuitsMemoire()
        Dim journal As New Journal() With {.NiveauMinimal = Niveau.Avertissement}
        journal.AjouterPuits(memoire)
        journal.Debogage("T", "ignore")
        journal.Information("T", "ignore")
        journal.Avertissement("T", "garde")
        journal.Erreur("T", "garde")
        Assert.AreEqual(2, memoire.Instantane().Length)
    End Sub

    <TestMethod>
    Public Sub Memoire_respecte_sa_capacite()
        Dim memoire As New PuitsMemoire() With {.Capacite = 3}
        Dim journal As New Journal()
        journal.AjouterPuits(memoire)
        For i As Integer = 1 To 5
            journal.Information("T", "ligne " & i.ToString())
        Next
        Assert.AreEqual(3, memoire.Instantane().Length)
    End Sub

    <TestMethod>
    Public Sub Formater_contient_niveau_categorie_et_message()
        Dim e As New EntreeJournal(Niveau.Erreur, "BDD", "Echec")
        Dim s As String = e.Formater()
        StringAssert.Contains(s, "ERREUR")
        StringAssert.Contains(s, "BDD")
        StringAssert.Contains(s, "Echec")
    End Sub

    <TestMethod>
    Public Sub Fichier_ecrit_et_rote_par_taille()
        Dim chemin As String = Path.Combine(Path.GetTempPath(), "ndxtest_" & Guid.NewGuid().ToString("N") & ".log")
        Dim archive As String = Path.Combine(Path.GetDirectoryName(chemin),
            Path.GetFileNameWithoutExtension(chemin) & ".1" & Path.GetExtension(chemin))
        Try
            Dim puits As New PuitsFichier(chemin, tailleMaxOctets:=256, nbArchives:=2)
            For i As Integer = 1 To 60
                puits.Ecrire(New EntreeJournal(Niveau.Information, "Test", "ligne de journal numero " & i.ToString()))
            Next
            Assert.IsTrue(File.Exists(chemin), "Le fichier courant doit exister.")
            Assert.IsTrue(File.Exists(archive), "Une archive .1 doit avoir ete creee par rotation.")
        Finally
            For Each f As String In {chemin, archive,
                Path.Combine(Path.GetDirectoryName(chemin), Path.GetFileNameWithoutExtension(chemin) & ".2" & Path.GetExtension(chemin))}
                If File.Exists(f) Then File.Delete(f)
            Next
        End Try
    End Sub

End Class

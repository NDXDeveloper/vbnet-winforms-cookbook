' ============================================================================
'  RaccourciWindowsTests.vb  -  Test du raccourci .lnk (COM WScript.Shell).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  L'objet COM WScript.Shell exige un appartement STA : on execute donc les
'  operations .lnk sur un thread STA dedie. Si l'objet est indisponible, le test
'  est marque "Inconclusive".
' ============================================================================

Imports System.IO
Imports System.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Shell

<TestClass>
Public Class RaccourciWindowsTests

    ''' <summary>Exécute une action sur un thread STA et relaie l'éventuelle exception.</summary>
    Private Shared Sub SurSTA(ByVal action As Action)
        Dim capture As Exception = Nothing
        Dim t As New Thread(Sub()
                                Try
                                    action()
                                Catch ex As Exception
                                    capture = ex
                                End Try
                            End Sub)
        t.SetApartmentState(ApartmentState.STA)
        t.Start()
        t.Join()
        If capture IsNot Nothing Then Throw capture
    End Sub

    <TestMethod>
    Public Sub Lnk_Creation_Puis_LectureCible()
        Dim dossier As String = Path.Combine(Path.GetTempPath(), "ndx-shell-tests")
        Directory.CreateDirectory(dossier)
        Dim lnk As String = Path.Combine(dossier, "test-ndx.lnk")
        Dim cible As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "notepad.exe")
        If File.Exists(lnk) Then File.Delete(lnk)

        Dim disponible As Boolean = False
        Dim existe As Boolean = False
        Dim lu As String = ""
        SurSTA(Sub()
                   disponible = RaccourciWindows.EstDisponible()
                   If Not disponible Then Return
                   RaccourciWindows.Creer(lnk, cible, description:="Test NDX")
                   existe = File.Exists(lnk)
                   lu = RaccourciWindows.LireCible(lnk)
               End Sub)

        If Not disponible Then Assert.Inconclusive("WScript.Shell indisponible sur ce poste.")
        Assert.IsTrue(existe, "Le fichier .lnk doit avoir été créé.")
        Assert.IsTrue(lu.ToLowerInvariant().EndsWith("notepad.exe"),
                      "La cible relue doit pointer vers notepad.exe (lu : " & lu & ").")

        Try
            File.Delete(lnk)
        Catch
        End Try
    End Sub

End Class

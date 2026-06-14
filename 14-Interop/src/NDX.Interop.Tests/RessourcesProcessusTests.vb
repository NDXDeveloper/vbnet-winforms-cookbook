' ============================================================================
'  RessourcesProcessusTests.vb  -  Tests du comptage GDI/USER (appel reel Win32).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Interop

<TestClass>
Public Class RessourcesProcessusTests

    <TestMethod>
    Public Sub ObjetsGdi_AppelAboutit()
        Dim n As Integer = RessourcesProcessus.ObjetsGdi()
        Assert.IsTrue(n >= 0 AndAlso n < 1000000, "Le comptage GDI doit renvoyer une valeur plausible (P/Invoke OK).")
    End Sub

    <TestMethod>
    Public Sub ObjetsUser_AppelAboutit()
        Dim n As Integer = RessourcesProcessus.ObjetsUser()
        Assert.IsTrue(n >= 0 AndAlso n < 1000000, "Le comptage USER doit renvoyer une valeur plausible (P/Invoke OK).")
    End Sub

End Class

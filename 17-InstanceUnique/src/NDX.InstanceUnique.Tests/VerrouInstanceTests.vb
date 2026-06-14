' ============================================================================
'  VerrouInstanceTests.vb  -  Tests du verrou d'instance (Mutex, en memoire).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.InstanceUnique

<TestClass>
Public Class VerrouInstanceTests

    <TestMethod>
    Public Sub Premiere_Vrai_Suivante_Faux_Puis_Reutilisable()
        Dim nom As String = "ndx-verrou-test-" & Guid.NewGuid().ToString("N")

        Dim v1 As New VerrouInstance(nom)
        Assert.IsTrue(v1.EstPremiere, "La première instance doit être primaire.")

        Dim v2 As New VerrouInstance(nom)
        Assert.IsFalse(v2.EstPremiere, "Tant que v1 vit, v2 n'est pas primaire.")
        v2.Dispose()
        v1.Dispose()

        ' Une fois v1 libérée, le nom redevient disponible.
        Dim v3 As New VerrouInstance(nom)
        Assert.IsTrue(v3.EstPremiere, "Après libération, une nouvelle instance redevient primaire.")
        v3.Dispose()
    End Sub

End Class

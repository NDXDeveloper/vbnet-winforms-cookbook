' ============================================================================
'  DepotImageTests.vb  -  Test d'integration (base "mediatheque").
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  Requiert le conteneur MariaDB demarre (docker compose up -d).
'  Indisponible -> "Inconclusive" plutot qu'echec.
' ============================================================================

Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Images

<TestClass>
Public Class DepotImageTests

    Private Shared Sub ExigerBase()
        Dim message As String = ""
        If Not DepotImage.TesterConnexion(message) Then
            Assert.Inconclusive("Base indisponible (démarrez le conteneur Docker). Détail : " & message)
        End If
    End Sub

    <TestMethod>
    Public Sub Enregistrer_Puis_ChargerVignette_RendUneImageValide()
        ExigerBase()
        Dim png As Byte()
        Using bmp As New Bitmap(300, 150)
            Using g = Graphics.FromImage(bmp)
                g.Clear(Color.SteelBlue)
            End Using
            png = Vignette.VersPng(bmp)
        End Using

        Dim id As Integer = DepotImage.Enregistrer("test-" & DateTime.UtcNow.Ticks.ToString() & ".png", png)
        Assert.IsTrue(id > 0)

        Dim octets As Byte() = DepotImage.ChargerVignette(id)
        Using flux As New MemoryStream(octets)
            Using v As Image = Image.FromStream(flux)
                Assert.IsTrue(v.Width > 0 AndAlso v.Height > 0, "La vignette relue doit être une image valide.")
                Assert.IsTrue(v.Width <= 160 AndAlso v.Height <= 160, "La vignette doit tenir dans la boîte 160×160.")
            End Using
        End Using
    End Sub

End Class

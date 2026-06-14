' ============================================================================
'  SceneTests.vb  -  Tests de la selection (forme la plus haute sous un point).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports NDX.Dessin

<TestClass>
Public Class SceneTests

    <TestMethod>
    Public Sub TrouverA_RenvoieLaFormeLaPlusHaute()
        Dim scene As New Scene()
        scene.Ajouter(New Forme() With {.Type = TypeForme.Rectangle, .X = 0, .Y = 0, .Largeur = 100, .Hauteur = 100})
        scene.Ajouter(New Forme() With {.Type = TypeForme.Ellipse, .X = 0, .Y = 0, .Largeur = 100, .Hauteur = 100})

        Dim trouvee = scene.TrouverA(50, 50)
        Assert.IsNotNull(trouvee)
        Assert.AreEqual(TypeForme.Ellipse, trouvee.Type, "La dernière ajoutée est au-dessus.")
    End Sub

    <TestMethod>
    Public Sub TrouverA_AucuneForme_RendNothing()
        Dim scene As New Scene()
        scene.Ajouter(New Forme() With {.Type = TypeForme.Rectangle, .X = 0, .Y = 0, .Largeur = 10, .Hauteur = 10})
        Assert.IsNull(scene.TrouverA(500, 500))
    End Sub

End Class

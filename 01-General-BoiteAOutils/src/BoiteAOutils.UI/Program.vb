Imports System.Windows.Forms

''' <summary>
''' Point d'entree de la galerie WinForms.
''' </summary>
''' <remarks>
''' On fournit explicitement <c>Sub Main</c> (cf. propriete de projet
''' <c>MyType = WindowsFormsWithCustomSubMain</c>) afin de maitriser la sequence
''' d'initialisation : culture, styles visuels, puis ouverture de la fenetre
''' principale.
''' </remarks>
Module Program

    <STAThread>
    Public Sub Main()
        ' Culture fr-FR avec separateur decimal ".".
        OutilsSysteme.DefinirCulture()

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New FrmGalerie())
    End Sub

End Module

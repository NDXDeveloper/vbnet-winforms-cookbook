' ============================================================================
'  Program.vb
'  Point d'entree de la galerie de demonstration (WinForms).
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Windows.Forms

''' <summary>Point d'entree de l'application.</summary>
Module Program

    <STAThread>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New FrmPrincipale())
    End Sub

End Module

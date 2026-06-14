' ============================================================================
'  Program.vb  -  Demarrage : capture globale d'exceptions + splash + fenetre.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Threading
Imports System.Windows.Forms
Imports NDX.Demarrage

Module Program
    <STAThread>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        ' Capture globale AVANT toute fenêtre (SetUnhandledExceptionMode l'exige).
        GestionnaireExceptions.Installer(AddressOf SurExceptionGlobale)

        ' Écran de démarrage (splash) le temps de l'initialisation.
        Using splash As New FrmChargement()
            splash.Show()
            splash.Refresh()
            Thread.Sleep(1200)
            splash.Close()
        End Using

        Application.Run(New FrmPrincipale())
    End Sub

    ''' <summary>Appelé pour toute exception non gérée : on journalise et on informe, sans planter.</summary>
    Private Sub SurExceptionGlobale(ByVal ex As Exception)
        Try
            DepotEvenement.Enregistrer("exception", GestionnaireExceptions.Decrire(ex))
        Catch
            ' base indisponible : on n'empêche pas l'affichage du message.
        End Try
        MessageBox.Show("Une erreur a été interceptée :" & vbCrLf & vbCrLf & GestionnaireExceptions.Decrire(ex),
                        "Erreur gérée", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub
End Module

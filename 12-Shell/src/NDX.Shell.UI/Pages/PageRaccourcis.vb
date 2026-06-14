' ============================================================================
'  PageRaccourcis.vb  -  Creer et lire des raccourcis .lnk et .url.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports NDX.Shell

''' <summary>Crée des raccourcis .lnk / .url sur le disque et relit leur cible.</summary>
Public NotInheritable Class PageRaccourcis
    Inherits PageBase

    Private ReadOnly _txtCible As New TextBox() With {.Width = 460}
    Private ReadOnly _sortie As TextBox

    Public Sub New()
        MyBase.New("Créer / lire un raccourci", "Saisissez une cible (exécutable pour un .lnk, ou URL pour un .url), puis créez le raccourci ou relisez-en un.")
        _sortie = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 80, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        haut.Controls.Add(New Label() With {.Text = "Cible (chemin .exe ou URL) :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        _txtCible.Text = "C:\Windows\System32\notepad.exe"
        haut.Controls.Add(_txtCible)
        haut.Controls.Add(Bouton("Créer un .lnk…", AddressOf SurCreerLnk))
        haut.Controls.Add(Bouton("Créer un .url…", AddressOf SurCreerUrl))
        haut.Controls.Add(Bouton("Lire un raccourci…", AddressOf SurLire))

        Contenu.Controls.Add(_sortie)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub SurCreerLnk(ByVal sender As Object, ByVal e As EventArgs)
        If Not RaccourciWindows.EstDisponible() Then
            Journaliser("WScript.Shell indisponible sur ce poste.") : Return
        End If
        Using dlg As New SaveFileDialog() With {.Filter = "Raccourci Windows (*.lnk)|*.lnk", .FileName = "Bloc-notes.lnk"}
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Try
                RaccourciWindows.Creer(dlg.FileName, _txtCible.Text, description:="Raccourci créé par NDX.Shell")
                Journaliser("Raccourci .lnk créé : " & dlg.FileName & "  →  " & _txtCible.Text)
            Catch ex As Exception
                Journaliser("Erreur .lnk : " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub SurCreerUrl(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New SaveFileDialog() With {.Filter = "Raccourci Internet (*.url)|*.url", .FileName = "Lien.url"}
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Try
                RaccourciInternet.Ecrire(dlg.FileName, _txtCible.Text)
                Journaliser("Raccourci .url créé : " & dlg.FileName & "  →  " & _txtCible.Text)
            Catch ex As Exception
                Journaliser("Erreur .url : " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub SurLire(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New OpenFileDialog() With {.Filter = "Raccourcis (*.lnk;*.url)|*.lnk;*.url"}
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Try
                Dim cible As String
                If String.Equals(Path.GetExtension(dlg.FileName), ".url", StringComparison.OrdinalIgnoreCase) Then
                    cible = RaccourciInternet.LireFichier(dlg.FileName)
                Else
                    cible = RaccourciWindows.LireCible(dlg.FileName)
                End If
                Journaliser("Cible de « " & Path.GetFileName(dlg.FileName) & " » : " & cible)
            Catch ex As Exception
                Journaliser("Erreur de lecture : " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub Journaliser(ByVal ligne As String)
        _sortie.AppendText(ligne & vbCrLf)
    End Sub

End Class

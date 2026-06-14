' ============================================================================
'  PageJournal.vb  -  Emettre des entrees et observer la diffusion (multi-puits).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports NDX.Journalisation

Public NotInheritable Class PageJournal
    Inherits PageBase

    Private ReadOnly _memoire As New PuitsMemoire()
    Private ReadOnly _console As New PuitsConsole()
    Private ReadOnly _fichier As PuitsFichier
    Private ReadOnly _vue As TextBox
    Private ReadOnly _cboNiveauMin As New ComboBox()
    Private ReadOnly _txtCategorie As New TextBox()
    Private ReadOnly _txtMessage As New TextBox()
    Private ReadOnly _chkFichier As New CheckBox()
    Private ReadOnly _chkBase As New CheckBox()

    Public Sub New()
        MyBase.New("Journalisation en direct",
                   "Émettez des entrées : elles sont diffusées vers la mémoire (vue ci-dessous), la console, et — au choix — le fichier et la base.")
        _fichier = New PuitsFichier(Path.Combine(Path.GetTempPath(), "ndx-journal-demo.log"), tailleMaxOctets:=8192, nbArchives:=3)
        _vue = Console()
        Construire()
        AddHandler _memoire.EntreeAjoutee, AddressOf SurEntree
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True, .WrapContents = True}

        _cboNiveauMin.DropDownStyle = ComboBoxStyle.DropDownList
        _cboNiveauMin.Width = 120
        _cboNiveauMin.DataSource = [Enum].GetValues(GetType(Niveau))
        _txtCategorie.Width = 110
        _txtCategorie.Text = "Demo"
        _txtMessage.Width = 240
        _txtMessage.Text = "Message d'exemple"
        _chkFichier.Text = "Vers fichier (rotation 8 Ko)"
        _chkFichier.AutoSize = True
        _chkFichier.Margin = New Padding(6, 8, 0, 0)
        _chkBase.Text = "Vers base"
        _chkBase.AutoSize = True
        _chkBase.Margin = New Padding(6, 8, 0, 0)

        barre.Controls.Add(New Label() With {.Text = "Niveau min :", .AutoSize = True, .Margin = New Padding(4, 9, 0, 0)})
        barre.Controls.Add(_cboNiveauMin)
        barre.Controls.Add(New Label() With {.Text = "Catégorie :", .AutoSize = True, .Margin = New Padding(6, 9, 0, 0)})
        barre.Controls.Add(_txtCategorie)
        barre.Controls.Add(New Label() With {.Text = "Message :", .AutoSize = True, .Margin = New Padding(6, 9, 0, 0)})
        barre.Controls.Add(_txtMessage)
        barre.Controls.Add(_chkFichier)
        barre.Controls.Add(_chkBase)

        Dim barre2 As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True, .WrapContents = True}
        barre2.Controls.Add(Bouton("Debug", AddressOf SurDebug))
        barre2.Controls.Add(Bouton("Info", AddressOf SurInfo))
        barre2.Controls.Add(Bouton("Avertissement", AddressOf SurAvert))
        barre2.Controls.Add(Bouton("Erreur", AddressOf SurErreur))
        barre2.Controls.Add(Bouton("Critique", AddressOf SurCritique))
        barre2.Controls.Add(Bouton("Ouvrir le fichier", AddressOf SurOuvrirFichier))
        barre2.Controls.Add(Bouton("Vider la vue", AddressOf SurVider))

        Contenu.Controls.Add(_vue)
        Contenu.Controls.Add(barre2)
        Contenu.Controls.Add(barre)
    End Sub

    Private Function NiveauMin() As Niveau
        Return CType(_cboNiveauMin.SelectedItem, Niveau)
    End Function

    Private Sub Emettre(ByVal niveau As Niveau)
        ' Journal ephemere : memoire + console toujours, fichier/base au choix.
        Dim journal As New Journal() With {.NiveauMinimal = NiveauMin()}
        journal.AjouterPuits(_memoire)
        journal.AjouterPuits(_console)
        If _chkFichier.Checked Then journal.AjouterPuits(_fichier)
        If _chkBase.Checked Then journal.AjouterPuits(New PuitsBase())

        Dim exemple As Exception = Nothing
        If niveau >= Niveau.Erreur Then
            exemple = New InvalidOperationException("Exemple d'exception associée à l'entrée.")
        End If
        journal.Journaliser(niveau, _txtCategorie.Text, _txtMessage.Text, exemple)
    End Sub

    Private Sub SurDebug(ByVal s As Object, ByVal e As EventArgs)
        Emettre(Niveau.Debogage)
    End Sub
    Private Sub SurInfo(ByVal s As Object, ByVal e As EventArgs)
        Emettre(Niveau.Information)
    End Sub
    Private Sub SurAvert(ByVal s As Object, ByVal e As EventArgs)
        Emettre(Niveau.Avertissement)
    End Sub
    Private Sub SurErreur(ByVal s As Object, ByVal e As EventArgs)
        Emettre(Niveau.Erreur)
    End Sub
    Private Sub SurCritique(ByVal s As Object, ByVal e As EventArgs)
        Emettre(Niveau.Critique)
    End Sub

    Private Sub SurOuvrirFichier(ByVal s As Object, ByVal e As EventArgs)
        If File.Exists(_fichier.Chemin) Then
            Process.Start(_fichier.Chemin)
        Else
            _vue.AppendText("(le fichier n'existe pas encore : cochez « Vers fichier » et émettez des entrées)" & Environment.NewLine)
        End If
    End Sub

    Private Sub SurVider(ByVal s As Object, ByVal e As EventArgs)
        _memoire.Vider()
        _vue.Clear()
    End Sub

    ' Recu potentiellement depuis un autre thread -> marshalage vers l'UI.
    Private Sub SurEntree(ByVal entree As EntreeJournal)
        If _vue.IsDisposed Then Return
        If _vue.InvokeRequired Then
            Try
                _vue.BeginInvoke(New Action(Of EntreeJournal)(AddressOf SurEntree), entree)
            Catch
            End Try
            Return
        End If
        _vue.AppendText(entree.Formater() & Environment.NewLine)
    End Sub

End Class

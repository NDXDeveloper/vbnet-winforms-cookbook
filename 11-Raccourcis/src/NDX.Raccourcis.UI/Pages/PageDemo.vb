' ============================================================================
'  PageDemo.vb  -  Capture clavier en direct : reconnaissance des accords.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports NDX.Raccourcis

''' <summary>
''' Capture les frappes via <see cref="ProcessCmdKey"/> (qui intercepte aussi
''' Ctrl+S, Tab, etc.) et les transmet au gestionnaire, en affichant l'état.
''' </summary>
Public NotInheritable Class PageDemo
    Inherits PageBase

    Private ReadOnly _gestionnaire As New GestionnaireRaccourcis()
    Private ReadOnly _zone As New Label()
    Private ReadOnly _console As TextBox
    Private ReadOnly _lblSequence As New Label() With {.Dock = DockStyle.Bottom, .Height = 30, .Font = New Font("Segoe UI Semibold", 11.0F),
                                                       .TextAlign = ContentAlignment.MiddleLeft, .ForeColor = Color.FromArgb(84, 110, 122)}

    Public Sub New()
        MyBase.New("Démonstration (accords)", "Cliquez dans la zone puis tapez. Essayez : Ctrl+S, Ctrl+Maj+P, et l'accord Ctrl+K puis Ctrl+S.")
        _console = Console()
        InscrireRaccourcis()
        Construire()
    End Sub

    Private Sub InscrireRaccourcis()
        _gestionnaire.Inscrire("Enregistrer", "Ctrl+S")
        _gestionnaire.Inscrire("Tout enregistrer", "Ctrl+K, Ctrl+S")
        _gestionnaire.Inscrire("Palette de commandes", "Ctrl+Maj+P")
        _gestionnaire.Inscrire("Rechercher", "Ctrl+F")
        _gestionnaire.Inscrire("Commenter la ligne", "Ctrl+K, Ctrl+C")
        _gestionnaire.Inscrire("Plein écran", "F11")
    End Sub

    Private Sub Construire()
        _zone.Text = "▶  Cliquez ici, puis tapez un raccourci  ◀"
        _zone.Dock = DockStyle.Top
        _zone.Height = 70
        _zone.TextAlign = ContentAlignment.MiddleCenter
        _zone.Font = New Font("Segoe UI", 12.0F)
        _zone.BackColor = Color.FromArgb(236, 239, 241)
        _zone.ForeColor = Color.FromArgb(55, 71, 79)
        AddHandler _zone.Click, Sub() Me.Focus()

        Dim aide As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .Dock = DockStyle.Top, .Height = 130, .BorderStyle = BorderStyle.None,
            .BackColor = Color.White, .Font = New Font("Segoe UI", 9.5F)}
        Dim sb As New StringBuilder()
        sb.AppendLine("Raccourcis inscrits :")
        For Each liaison In _gestionnaire.Lister()
            sb.AppendLine("   • " & liaison.Value.PadRight(18) & " → " & liaison.Key)
        Next
        sb.AppendLine()
        sb.AppendLine("Échap réinitialise une séquence d'accord en cours.")
        aide.Text = sb.ToString()

        Contenu.Controls.Add(_console)
        Contenu.Controls.Add(_lblSequence)
        Contenu.Controls.Add(aide)
        Contenu.Controls.Add(_zone)
    End Sub

    ' On rend la page sélectionnable pour recevoir le focus clavier.
    Protected Overrides Sub OnGotFocus(ByVal e As EventArgs)
        MyBase.OnGotFocus(e)
        _zone.BackColor = Color.FromArgb(197, 225, 165)   ' actif : la capture écoute
    End Sub

    Protected Overrides Sub OnLostFocus(ByVal e As EventArgs)
        MyBase.OnLostFocus(e)
        _zone.BackColor = Color.FromArgb(236, 239, 241)
    End Sub

    ''' <summary>Intercepte toutes les frappes (y compris Ctrl+S, Tab…) tant que la page a le focus.</summary>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        Dim code As Keys = keyData And Keys.KeyCode
        ' On ignore les frappes de modificateurs seuls.
        If code = Keys.ControlKey OrElse code = Keys.ShiftKey OrElse code = Keys.Menu OrElse code = Keys.None Then
            Return MyBase.ProcessCmdKey(msg, keyData)
        End If
        If code = Keys.Escape Then
            _gestionnaire.Reinitialiser()
            _lblSequence.Text = ""
            Journaliser("(séquence réinitialisée)")
            Return True
        End If

        Dim r As ResultatTouche = _gestionnaire.Appuyer(New Combinaison(keyData))
        Select Case r.Etat
            Case EtatTouche.Declenchee
                _lblSequence.Text = ""
                Journaliser("✓ " & r.SequenceEnCours & "  →  ACTION : " & r.Action)
            Case EtatTouche.EnAttente
                _lblSequence.Text = "Accord en cours : " & r.SequenceEnCours & " …"
            Case Else
                _lblSequence.Text = ""
                Journaliser("✗ " & New Combinaison(keyData).ToString() & "  (aucun raccourci)")
        End Select
        Return True
    End Function

    Private Sub Journaliser(ByVal ligne As String)
        _console.AppendText(ligne & vbCrLf)
    End Sub

End Class

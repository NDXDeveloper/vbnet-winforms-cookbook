Imports System.Data
Imports System.Drawing
Imports System.Reflection
Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

''' <summary>
''' Page "Base de donnees" : requetes parametrees, affichage en grille stylee,
''' transaction (commit/rollback) et journalisation d'une erreur.
''' </summary>
Public NotInheritable Class PageBaseDeDonnees
    Inherits PageBase

    Private ReadOnly _dgv As New DataGridView()
    Private ReadOnly _nudFiche As New NumericUpDown()
    Private ReadOnly _dtpDepuis As New DateTimePicker()
    Private ReadOnly _console As TextBox

    Public Sub New()
        MyBase.New("Base de donnees (MariaDB)",
                   "ExecuteNonQuery / ExecuteScalar / GetDTfromCommand, requetes parametrees, transactions, journal d'erreurs.")
        _console = Console()
        Construire()
    End Sub

    Private Sub Construire()
        ' --- Grille de resultats (style + double buffer) ---------------------
        _dgv.Dock = DockStyle.Fill
        _dgv.ReadOnly = True
        _dgv.AllowUserToAddRows = False
        _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        OutilsControles.AppliquerStyleGrille(_dgv)
        OutilsControles.ActiverDoubleBuffer(_dgv)

        ' --- Console de messages ---------------------------------------------
        Dim panneauConsole As New Panel() With {.Dock = DockStyle.Bottom, .Height = 96, .Padding = New Padding(0, 4, 0, 0)}
        panneauConsole.Controls.Add(_console)

        ' --- Barre d'outils ---------------------------------------------------
        Dim barre As New FlowLayoutPanel() With {
            .Dock = DockStyle.Top, .AutoSize = True, .WrapContents = True, .Padding = New Padding(0, 0, 0, 6)}

        _nudFiche.Minimum = 1
        _nudFiche.Maximum = 15
        _nudFiche.Value = 1
        _nudFiche.Width = 50

        _dtpDepuis.Format = DateTimePickerFormat.Short
        _dtpDepuis.Width = 110
        _dtpDepuis.Value = New DateTime(2026, 1, 1)

        barre.Controls.Add(New Label() With {.Text = "Fiche n :", .AutoSize = True, .Anchor = AnchorStyles.Left, .Margin = New Padding(4, 9, 0, 0)})
        barre.Controls.Add(_nudFiche)
        barre.Controls.Add(Bouton("Indicateurs fiche", AddressOf SurIndicateurs))
        barre.Controls.Add(Bouton("Lignes", AddressOf SurLignes))
        barre.Controls.Add(Bouton("Montant global", AddressOf SurMontant))
        barre.Controls.Add(New Label() With {.Text = "  Stats depuis :", .AutoSize = True, .Anchor = AnchorStyles.Left, .Margin = New Padding(4, 9, 0, 0)})
        barre.Controls.Add(_dtpDepuis)
        barre.Controls.Add(Bouton("Stats clients", AddressOf SurStats))
        barre.Controls.Add(Bouton("Verifier articles", AddressOf SurVerifier))
        barre.Controls.Add(Bouton("Demo transaction", AddressOf SurTransaction))
        barre.Controls.Add(Bouton("Journaliser une erreur", AddressOf SurJournaliser))

        Contenu.Controls.Add(_dgv)
        Contenu.Controls.Add(panneauConsole)
        Contenu.Controls.Add(barre)
    End Sub

    Private Sub Afficher(ByVal dt As DataTable)
        _dgv.DataSource = dt
    End Sub

    Private Sub Tracer(ByVal texte As String)
        _console.AppendText(texte & Environment.NewLine)
    End Sub

    Private Sub SurIndicateurs(ByVal sender As Object, ByVal e As EventArgs)
        Afficher(AccesDonnees.IndicateursFiche(CInt(_nudFiche.Value)))
    End Sub

    Private Sub SurLignes(ByVal sender As Object, ByVal e As EventArgs)
        Afficher(AccesDonnees.LignesParFiche(CInt(_nudFiche.Value)))
    End Sub

    Private Sub SurMontant(ByVal sender As Object, ByVal e As EventArgs)
        Afficher(AccesDonnees.MontantGlobalParFiche(CInt(_nudFiche.Value)))
    End Sub

    Private Sub SurStats(ByVal sender As Object, ByVal e As EventArgs)
        Afficher(AccesDonnees.StatistiquesClients(_dtpDepuis.Value))
    End Sub

    Private Sub SurVerifier(ByVal sender As Object, ByVal e As EventArgs)
        ' On melange volontairement des identifiants existants (1, 2) et un absent (999).
        Dim aVerifier As New List(Of Integer) From {1, 2, 999}
        Dim problemes As New List(Of Integer)()
        Dim tousPresents As Boolean = AccesDonnees.VerifierArticles(aVerifier, problemes)
        If tousPresents Then
            Tracer("Verification : tous les articles existent.")
        Else
            Tracer("Verification : identifiants introuvables -> " & String.Join(", ", problemes))
        End If
    End Sub

    ''' <summary>
    ''' Demonstration de transaction : insertion d'une ligne puis ROLLBACK ;
    ''' on prouve, en comptant avant/apres, que rien n'a ete persiste.
    ''' </summary>
    Private Sub SurTransaction(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Using oMy As New ConnexionMySql(False, MethodBase.GetCurrentMethod())
                oMy.Open(MethodBase.GetCurrentMethod())
                Dim avant As Long = CompterJournal(oMy)

                oMy.BeginTransaction(MethodBase.GetCurrentMethod())
                Using cmd As New MySqlCommand(
                    "INSERT INTO journal_erreur(message, survenu_le, par_qui) VALUES(@m, @d, @q);",
                    oMy.Conn, oMy.Tr)
                    cmd.Parameters.AddWithValue("@m", "Ligne inseree dans une transaction de demonstration.")
                    cmd.Parameters.AddWithValue("@d", DateTime.Now)
                    cmd.Parameters.AddWithValue("@q", OutilsSysteme.NomUtilisateur())
                    AccesDonnees.ExecuteNonQuery(cmd, oMy, MethodBase.GetCurrentMethod(), Environment.StackTrace)
                End Using
                Dim pendant As Long = CompterJournal(oMy)

                oMy.Rollback(MethodBase.GetCurrentMethod())
                Dim apres As Long = CompterJournal(oMy)

                Tracer(String.Format("Transaction : avant={0}, pendant (non valide)={1}, apres ROLLBACK={2}.", avant, pendant, apres))
            End Using
        Catch ex As Exception
            Tracer("Erreur transaction : " & ex.Message)
        End Try
    End Sub

    Private Function CompterJournal(ByVal oMy As ConnexionMySql) As Long
        Using cmd As New MySqlCommand("SELECT COUNT(*) FROM journal_erreur;", oMy.Conn, oMy.Tr)
            Return Convert.ToInt64(AccesDonnees.ExecuteScalar(cmd, oMy, MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Using
    End Function

    Private Sub SurJournaliser(ByVal sender As Object, ByVal e As EventArgs)
        Dim id As Integer = AccesDonnees.LogErreur("Erreur de demonstration declenchee depuis la galerie.", OutilsSysteme.NomUtilisateur())
        Tracer("LogErreur : nouvelle entree id = " & id.ToString() & " (technique INSERT + LAST_INSERT_ID).")
    End Sub

End Class

Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Page "Systeme et OS" : memoire, utilisateur, machine, culture, GC, serialisation XML.</summary>
Public NotInheritable Class PageSysteme
    Inherits PageBase

    Private ReadOnly _console As TextBox

    Public Sub New()
        MyBase.New("Systeme et OS",
                   "MemoirePhysiqueDisponible, NomUtilisateur/Machine/Session, DefinirCulture, ForcerRamasseMiettes, DesactiverBips, SerialiserEnXml.")
        _console = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True, .WrapContents = True}
        barre.Controls.Add(Bouton("Memoire dispo.", AddressOf SurMemoire))
        barre.Controls.Add(Bouton("Utilisateur", AddressOf SurUtilisateur))
        barre.Controls.Add(Bouton("Machine", AddressOf SurMachine))
        barre.Controls.Add(Bouton("Session", AddressOf SurSession))
        barre.Controls.Add(Bouton("Definir culture fr-FR", AddressOf SurCulture))
        barre.Controls.Add(Bouton("Ramasse-miettes", AddressOf SurGc))
        barre.Controls.Add(Bouton("Desactiver les bips", AddressOf SurBips))
        barre.Controls.Add(Bouton("Serialiser un objet (XML)", AddressOf SurSerialiser))

        Contenu.Controls.Add(_console)
        Contenu.Controls.Add(barre)
    End Sub

    Private Sub Tracer(ByVal texte As String)
        _console.AppendText(texte & Environment.NewLine)
    End Sub

    Private Sub SurMemoire(ByVal sender As Object, ByVal e As EventArgs)
        Tracer(OutilsSysteme.MemoirePhysiqueDisponible())
    End Sub

    Private Sub SurUtilisateur(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("NomUtilisateur = " & OutilsSysteme.NomUtilisateur())
    End Sub

    Private Sub SurMachine(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("NomMachine = " & OutilsSysteme.NomMachine())
    End Sub

    Private Sub SurSession(ByVal sender As Object, ByVal e As EventArgs)
        Dim s As String = OutilsSysteme.NomSession()
        Tracer("NomSession = " & If(String.IsNullOrEmpty(s), "(session console locale)", s))
    End Sub

    Private Sub SurCulture(ByVal sender As Object, ByVal e As EventArgs)
        OutilsSysteme.DefinirCulture()
        Tracer("Culture courante = " & System.Threading.Thread.CurrentThread.CurrentCulture.Name &
               " (separateur decimal '" & System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator & "').")
    End Sub

    Private Sub SurGc(ByVal sender As Object, ByVal e As EventArgs)
        Dim avant As Long = GC.GetTotalMemory(False)
        OutilsSysteme.ForcerRamasseMiettes()
        Dim apres As Long = GC.GetTotalMemory(True)
        Tracer(String.Format("Ramasse-miettes : memoire geree {0:N0} -> {1:N0} octets.", avant, apres))
    End Sub

    Private Sub SurBips(ByVal sender As Object, ByVal e As EventArgs)
        OutilsSysteme.DesactiverBips()
        Tracer("Bips systeme desactives (SystemParametersInfo / SPI_SETBEEP).")
    End Sub

    Private Sub SurSerialiser(ByVal sender As Object, ByVal e As EventArgs)
        Dim exemple As New ArticleDemo() With {.Id = 1, .Designation = "Poutre IPE 200", .Prix = 145.0D}
        Tracer(OutilsSysteme.SerialiserEnXml(exemple))
    End Sub

    ''' <summary>Objet d'exemple serialisable (membres publics + constructeur sans parametre).</summary>
    Public Class ArticleDemo
        Public Property Id As Integer
        Public Property Designation As String
        Public Property Prix As Decimal
    End Class

End Class

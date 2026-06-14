' ============================================================================
'  PageGenerer.vb  -  Composer un PDF, l'enregistrer, l'ouvrir, l'archiver.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports NDX.Pdf

''' <summary>Compose un PDF à partir d'un titre, d'un auteur et d'un corps de texte.</summary>
Public NotInheritable Class PageGenerer
    Inherits PageBase

    Private ReadOnly _txtTitre As New TextBox() With {.Width = 300, .Text = "Rapport de démonstration"}
    Private ReadOnly _txtAuteur As New TextBox() With {.Width = 220, .Text = "Nicolas DEOUX"}
    Private ReadOnly _txtCorps As New TextBox()
    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Font = New Font("Segoe UI", 9.5F), .Padding = New Padding(6, 9, 0, 0)}

    Private _dernierPdf As Byte()

    Public Sub New()
        MyBase.New("Composer un PDF", "Saisissez un titre, un auteur et un corps : le document est généré octet par octet, puis enregistrable, ouvrable et archivable.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New TableLayoutPanel() With {.Dock = DockStyle.Top, .Height = 70, .ColumnCount = 4, .RowCount = 1}
        haut.Controls.Add(New Label() With {.Text = "Titre :", .AutoSize = True, .Margin = New Padding(4, 9, 2, 0)})
        haut.Controls.Add(_txtTitre)
        haut.Controls.Add(New Label() With {.Text = "Auteur :", .AutoSize = True, .Margin = New Padding(12, 9, 2, 0)})
        haut.Controls.Add(_txtAuteur)

        _txtCorps.Multiline = True
        _txtCorps.ScrollBars = ScrollBars.Vertical
        _txtCorps.Dock = DockStyle.Fill
        _txtCorps.Font = New Font("Consolas", 9.5F)
        _txtCorps.Text = CorpsExemple()

        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Bottom, .Height = 44}
        barre.Controls.Add(Bouton("Générer", AddressOf SurGenerer))
        barre.Controls.Add(Bouton("Enregistrer sous…", AddressOf SurEnregistrer))
        barre.Controls.Add(Bouton("Ouvrir", AddressOf SurOuvrir))
        barre.Controls.Add(Bouton("Archiver en base", AddressOf SurArchiver))
        barre.Controls.Add(_lblEtat)

        Contenu.Controls.Add(_txtCorps)
        Contenu.Controls.Add(barre)
        Contenu.Controls.Add(haut)
    End Sub

    ''' <summary>Compose le document PDF à partir des champs saisis.</summary>
    Private Function Composer() As DocumentPdf
        Dim doc As New DocumentPdf() With {.Titre = _txtTitre.Text, .Auteur = _txtAuteur.Text}
        Dim page As PagePdf = doc.AjouterPage()
        Dim marge As Double = 56

        page.EcrireTexte(marge, 70, _txtTitre.Text, PoliceStandard.HelveticaGras, 20)
        page.EcrireTexte(marge, 92, "par " & _txtAuteur.Text, PoliceStandard.Times, 11)
        page.TracerLigne(marge, 104, DocumentPdf.A4Largeur - marge, 104, 1)

        ' Corps : retour à la ligne automatique dans la largeur utile.
        Dim largeurUtile As Double = DocumentPdf.A4Largeur - 2 * marge
        page.EcrireParagraphe(marge, 130, largeurUtile, _txtCorps.Text, 11)

        page.TracerRectangle(marge, 760, largeurUtile, 30, False, 1)
        page.EcrireTexte(marge + 8, 780, "Document généré par NDX.Pdf — composition 100 % VB.NET.", PoliceStandard.Helvetica, 9)
        Return doc
    End Function

    Private Sub SurGenerer(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim doc As DocumentPdf = Composer()
            _dernierPdf = doc.Construire()
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "PDF généré : " & _dernierPdf.Length.ToString("N0") & " octets, " & doc.NombrePages.ToString() & " page(s)."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurEnregistrer(ByVal sender As Object, ByVal e As EventArgs)
        If Not Genere() Then Return
        Using dlg As New SaveFileDialog() With {.Filter = "Document PDF (*.pdf)|*.pdf", .FileName = "document.pdf"}
            If dlg.ShowDialog() = DialogResult.OK Then
                File.WriteAllBytes(dlg.FileName, _dernierPdf)
                _lblEtat.ForeColor = Color.Green
                _lblEtat.Text = "Enregistré : " & dlg.FileName
            End If
        End Using
    End Sub

    Private Sub SurOuvrir(ByVal sender As Object, ByVal e As EventArgs)
        If Not Genere() Then Return
        Try
            Dim chemin As String = Path.Combine(Path.GetTempPath(), "ndx-pdf-apercu.pdf")
            File.WriteAllBytes(chemin, _dernierPdf)
            Process.Start(New ProcessStartInfo(chemin) With {.UseShellExecute = True})
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Ouvert dans la visionneuse par défaut."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    Private Sub SurArchiver(ByVal sender As Object, ByVal e As EventArgs)
        If Not Genere() Then Return
        Try
            Dim id As Integer = DepotDocument.Enregistrer(_txtTitre.Text, _txtAuteur.Text, 1, _dernierPdf)
            _lblEtat.ForeColor = Color.Green
            _lblEtat.Text = "Archivé en base (id = " & id.ToString() & ")."
        Catch ex As Exception
            _lblEtat.ForeColor = Color.Firebrick
            _lblEtat.Text = "Erreur : " & ex.Message
        End Try
    End Sub

    ''' <summary>Génère le PDF si nécessaire ; renvoie False en cas d'échec.</summary>
    Private Function Genere() As Boolean
        If _dernierPdf Is Nothing Then SurGenerer(Me, EventArgs.Empty)
        Return _dernierPdf IsNot Nothing
    End Function

    Private Shared Function CorpsExemple() As String
        Return "Ce document illustre la composition de PDF en VB.NET pur." & vbCrLf &
               "Le moteur écrit l'en-tête, les objets, la table de références croisées et le trailer." & vbCrLf & vbCrLf &
               "Le corps que vous lisez est mis en page automatiquement : chaque ligne est coupée " &
               "à la largeur utile de la page, en respectant les mots. Modifiez ce texte puis cliquez " &
               "sur Générer pour voir le résultat. Les accents (é, è, à, ç, ù, œ) sont gérés via " &
               "l'encodage WinAnsi des polices standard."
    End Function

End Class

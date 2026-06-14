' ============================================================================
'  PageFichiers.vb
'  Demonstration : serialiser vers un fichier / depuis un fichier, et vers le
'  stockage isole de l'utilisateur.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Serialisation

''' <summary>Page de demonstration de la persistance fichier et stockage isole.</summary>
Public NotInheritable Class PageFichiers
    Inherits PageBase

    Private Const FICHIER_ISOLE As String = "catalogue_demo.dat"

    Private ReadOnly _cboFormat As New ComboBox()
    Private ReadOnly _console As TextBox

    Public Sub New()
        MyBase.New("Fichiers et stockage isolé",
                   "Enregistrer / recharger un objet vers un fichier ou le stockage isolé. Choisissez le même format au chargement.")
        _console = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True, .WrapContents = True}

        _cboFormat.DropDownStyle = ComboBoxStyle.DropDownList
        _cboFormat.Width = 130
        _cboFormat.DataSource = [Enum].GetValues(GetType(FormatSerialisation))

        barre.Controls.Add(New Label() With {.Text = "Format :", .AutoSize = True, .Margin = New Padding(4, 9, 0, 0)})
        barre.Controls.Add(_cboFormat)
        barre.Controls.Add(Bouton("Enregistrer dans un fichier...", AddressOf SurEnregistrerFichier))
        barre.Controls.Add(Bouton("Charger depuis un fichier...", AddressOf SurChargerFichier))
        barre.Controls.Add(New Label() With {.Text = "  |  Stockage isolé :", .AutoSize = True, .Margin = New Padding(4, 9, 0, 0)})
        barre.Controls.Add(Bouton("Sauver", AddressOf SurSauverIsole))
        barre.Controls.Add(Bouton("Charger", AddressOf SurChargerIsole))
        barre.Controls.Add(Bouton("Existe ?", AddressOf SurExisteIsole))
        barre.Controls.Add(Bouton("Supprimer", AddressOf SurSupprimerIsole))

        Contenu.Controls.Add(_console)
        Contenu.Controls.Add(barre)
    End Sub

    Private Function FormatChoisi() As FormatSerialisation
        Return CType(_cboFormat.SelectedItem, FormatSerialisation)
    End Function

    Private Sub Tracer(ByVal texte As String)
        _console.AppendText(texte & vbCrLf)
    End Sub

    Private Sub Resumer(ByVal cat As Catalogue)
        If cat Is Nothing Then
            Tracer("  (objet nul)")
            Return
        End If
        Tracer("  " & cat.Nom & " — " & cat.Produits.Count.ToString() & " produits.")
    End Sub

    Private Sub SurEnregistrerFichier(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New SaveFileDialog()
            dlg.Filter = "Tous les fichiers|*.*"
            dlg.FileName = "catalogue." & FormatChoisi().ToString().ToLowerInvariant()
            If dlg.ShowDialog() = DialogResult.OK Then
                Serialiseur.SauverFichier(Catalogue.Exemple(), dlg.FileName, FormatChoisi())
                Tracer("Enregistré (" & FormatChoisi().ToString() & ") : " & dlg.FileName)
            End If
        End Using
    End Sub

    Private Sub SurChargerFichier(ByVal sender As Object, ByVal e As EventArgs)
        Using dlg As New OpenFileDialog()
            dlg.Filter = "Tous les fichiers|*.*"
            If dlg.ShowDialog() = DialogResult.OK Then
                Try
                    Dim cat As Catalogue = Serialiseur.ChargerFichier(Of Catalogue)(dlg.FileName, FormatChoisi())
                    Tracer("Chargé depuis " & dlg.FileName & " :")
                    Resumer(cat)
                Catch ex As Exception
                    Tracer("Échec du chargement (format incompatible ?) : " & ex.Message)
                End Try
            End If
        End Using
    End Sub

    Private Sub SurSauverIsole(ByVal sender As Object, ByVal e As EventArgs)
        StockageIsole.Sauver(Catalogue.Exemple(), FICHIER_ISOLE, FormatChoisi())
        Tracer("Stockage isolé : enregistré '" & FICHIER_ISOLE & "' (" & FormatChoisi().ToString() & ").")
    End Sub

    Private Sub SurChargerIsole(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim cat As Catalogue = StockageIsole.Charger(Of Catalogue)(FICHIER_ISOLE, FormatChoisi())
            Tracer("Stockage isolé : '" & FICHIER_ISOLE & "' chargé :")
            Resumer(cat)
        Catch ex As Exception
            Tracer("Stockage isolé : échec du chargement : " & ex.Message)
        End Try
    End Sub

    Private Sub SurExisteIsole(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("Stockage isolé : '" & FICHIER_ISOLE & "' existe = " & StockageIsole.Existe(FICHIER_ISOLE).ToString())
    End Sub

    Private Sub SurSupprimerIsole(ByVal sender As Object, ByVal e As EventArgs)
        StockageIsole.Supprimer(FICHIER_ISOLE)
        Tracer("Stockage isolé : '" & FICHIER_ISOLE & "' supprimé (s'il existait).")
    End Sub

End Class

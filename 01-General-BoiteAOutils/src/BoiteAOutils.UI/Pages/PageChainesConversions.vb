Imports System.Drawing
Imports System.Globalization
Imports System.Windows.Forms

''' <summary>Page "Chaines et conversions" : accents, caracteres speciaux, GUID, formatage.</summary>
Public NotInheritable Class PageChainesConversions
    Inherits PageBase

    Private ReadOnly _txtEntree As New TextBox()
    Private ReadOnly _console As TextBox

    Public Sub New()
        MyBase.New("Chaines et conversions",
                   "RetirerAccents, RetirerCaracteresSpeciaux, RendreFiltreValide, PremiereLettreEnMajuscule, ContientGuid, ConvertirEnEntier, Formater.")
        _console = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim haut As New Panel() With {.Dock = DockStyle.Top, .Height = 96}
        haut.Controls.Add(New Label() With {.Text = "Chaine d'entree :", .AutoSize = True, .Location = New Point(2, 4)})
        _txtEntree.Location = New Point(4, 24)
        _txtEntree.Width = 700
        _txtEntree.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right
        _txtEntree.Text = "Evenement n42 : Cafe a 3,50 EUR {3F2504E0-4F89-41D3-9A0C-0305E82C3301}"

        Dim barre As New FlowLayoutPanel() With {.Location = New Point(4, 52), .AutoSize = True, .Width = 1000}
        barre.Controls.Add(Bouton("Retirer accents", AddressOf SurAccents))
        barre.Controls.Add(Bouton("Retirer car. speciaux", AddressOf SurSpeciaux))
        barre.Controls.Add(Bouton("Filtre valide", AddressOf SurFiltre))
        barre.Controls.Add(Bouton("1re lettre majuscule", AddressOf SurMajuscule))
        barre.Controls.Add(Bouton("Contient un GUID ?", AddressOf SurGuid))
        barre.Controls.Add(Bouton("-> Entier", AddressOf SurEntier))
        barre.Controls.Add(Bouton("Formater (double)", AddressOf SurFormater))

        haut.Controls.Add(_txtEntree)
        haut.Controls.Add(barre)

        Contenu.Controls.Add(_console)
        Contenu.Controls.Add(haut)
    End Sub

    Private Sub Tracer(ByVal etiquette As String, ByVal resultat As String)
        _console.AppendText(etiquette & " => " & resultat & Environment.NewLine)
    End Sub

    Private Sub SurAccents(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("RetirerAccents", OutilsChaines.RetirerAccents(_txtEntree.Text))
    End Sub

    Private Sub SurSpeciaux(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("RetirerCaracteresSpeciaux", OutilsChaines.RetirerCaracteresSpeciaux(_txtEntree.Text))
    End Sub

    Private Sub SurFiltre(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("RendreFiltreValide", OutilsChaines.RendreFiltreValide(_txtEntree.Text))
    End Sub

    Private Sub SurMajuscule(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("PremiereLettreEnMajuscule", OutilsChaines.PremiereLettreEnMajuscule(_txtEntree.Text))
    End Sub

    Private Sub SurGuid(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("ContientGuid", OutilsChaines.ContientGuid(_txtEntree.Text).ToString())
    End Sub

    Private Sub SurEntier(ByVal sender As Object, ByVal e As EventArgs)
        Tracer("ConvertirEnEntier", OutilsConversions.ConvertirEnEntier(_txtEntree.Text).ToString())
    End Sub

    Private Sub SurFormater(ByVal sender As Object, ByVal e As EventArgs)
        Dim valeur As Double
        If Double.TryParse(_txtEntree.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, valeur) Then
            Tracer("Formater", OutilsConversions.Formater(valeur))
        Else
            Tracer("Formater", "(entree non numerique - saisissez un nombre, ex. 12.50)")
        End If
    End Sub

End Class

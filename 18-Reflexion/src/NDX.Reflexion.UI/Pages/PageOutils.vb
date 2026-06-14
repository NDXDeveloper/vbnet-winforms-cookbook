' ============================================================================
'  PageOutils.vb  -  Demo : copie de proprietes + purge d'evenements.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Reflexion

''' <summary>Démonstration de la copie de propriétés par réflexion et de la purge d'abonnés.</summary>
Public NotInheritable Class PageOutils
    Inherits PageBase

    ' --- Copie ---
    Private ReadOnly _txtNom As New TextBox() With {.Width = 160, .Text = "Martin"}
    Private ReadOnly _txtPrenom As New TextBox() With {.Width = 160, .Text = "Alice"}
    Private ReadOnly _txtAge As New TextBox() With {.Width = 60, .Text = "30"}
    Private ReadOnly _lblCopie As New Label() With {.AutoSize = True, .Font = New Font("Consolas", 9.5F), .ForeColor = Color.DimGray}

    ' --- Purge d'événements ---
    Private ReadOnly _article As New Article()
    Private _compte As Integer
    Private ReadOnly _lblPurge As New Label() With {.AutoSize = True, .Font = New Font("Consolas", 9.5F), .ForeColor = Color.DimGray}

    Public Sub New()
        MyBase.New("Outils (copie, purge)", "À gauche : copie de propriétés de même nom. À droite : abonnement puis purge de tous les abonnés d'un événement.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim copie As New GroupBox() With {.Text = "Copie de propriétés (CopieurProprietes)", .Dock = DockStyle.Top, .Height = 160, .Padding = New Padding(8)}
        Dim gc As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        gc.Controls.Add(New Label() With {.Text = "Nom :", .AutoSize = True, .Margin = New Padding(4, 8, 2, 0)})
        gc.Controls.Add(_txtNom)
        gc.Controls.Add(New Label() With {.Text = "Prénom :", .AutoSize = True, .Margin = New Padding(8, 8, 2, 0)})
        gc.Controls.Add(_txtPrenom)
        gc.Controls.Add(New Label() With {.Text = "Âge :", .AutoSize = True, .Margin = New Padding(8, 8, 2, 0)})
        gc.Controls.Add(_txtAge)
        gc.Controls.Add(Bouton("Copier vers une nouvelle Personne", AddressOf SurCopier))
        gc.Controls.Add(_lblCopie)
        copie.Controls.Add(gc)

        Dim purge As New GroupBox() With {.Text = "Purge d'événements (PurgeEvenements)", .Dock = DockStyle.Top, .Height = 150, .Padding = New Padding(8)}
        Dim gp As New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True}
        gp.Controls.Add(Bouton("Abonner 3 gestionnaires", AddressOf SurAbonner))
        gp.Controls.Add(Bouton("Déclencher l'événement", AddressOf SurDeclencher))
        gp.Controls.Add(Bouton("Purger les abonnés", AddressOf SurPurger))
        gp.Controls.Add(_lblPurge)
        purge.Controls.Add(gp)

        Contenu.Controls.Add(purge)
        Contenu.Controls.Add(copie)
        AfficherPurge("Aucun abonné. Compteur = 0.")
    End Sub

    Private Sub SurCopier(ByVal sender As Object, ByVal e As EventArgs)
        Dim age As Integer
        Integer.TryParse(_txtAge.Text, age)
        Dim source As New Personne() With {.Nom = _txtNom.Text, .Prenom = _txtPrenom.Text, .Age = age, .Courriel = "demo@exemple.test"}
        Dim destination As New Personne()
        Dim n As Integer = CopieurProprietes.Copier(source, destination)
        _lblCopie.ForeColor = Color.Green
        _lblCopie.Text = n.ToString() & " propriété(s) copiée(s) → " & destination.ToString() &
                         " (" & destination.Age.ToString() & " ans, " & destination.Courriel & ")"
    End Sub

    Private Sub SurAbonner(ByVal sender As Object, ByVal e As EventArgs)
        For i As Integer = 1 To 3
            AddHandler _article.PrixModifie, AddressOf Gestionnaire
        Next
        AfficherPurge("3 gestionnaires abonnés. Déclenchez pour voir le compteur monter de 3.")
    End Sub

    Private Sub SurDeclencher(ByVal sender As Object, ByVal e As EventArgs)
        _article.DeclencherPrixModifie()
        AfficherPurge("Événement déclenché. Compteur = " & _compte.ToString() & ".")
    End Sub

    Private Sub SurPurger(ByVal sender As Object, ByVal e As EventArgs)
        Dim ok As Boolean = PurgeEvenements.RetirerTous(_article, "PrixModifie")
        AfficherPurge("Purge " & If(ok, "réussie", "introuvable") & ". Re-déclenchez : le compteur ne bougera plus.")
    End Sub

    Private Sub Gestionnaire(ByVal sender As Object, ByVal e As EventArgs)
        _compte += 1
    End Sub

    Private Sub AfficherPurge(ByVal message As String)
        _lblPurge.ForeColor = Color.DimGray
        _lblPurge.Text = "Compteur = " & _compte.ToString() & "   |   " & message
    End Sub

End Class

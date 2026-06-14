Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Page d'accueil : presentation du projet et test de connexion a la base.</summary>
Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil",
                   "Galerie de techniques de programmation .NET (VB.NET / .NET Framework 4.8.1).")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Ce projet illustre, theme par theme, des techniques de programmation .NET," & vbCrLf &
            "regroupees dans une bibliotheque reutilisable (BoiteAOutils.Core) que cette" & vbCrLf &
            "galerie consomme." & vbCrLf & vbCrLf &
            "Parcourez les pages a gauche :" & vbCrLf &
            "  - Base de donnees : connexion MariaDB, requetes parametrees, transactions," & vbCrLf &
            "    logique de seconde chance, journalisation des erreurs." & vbCrLf &
            "  - Chaines et conversions : accents, caracteres speciaux, GUID, formatage." & vbCrLf &
            "  - Geometrie : milieu, distance au carre, rotation de point." & vbCrLf &
            "  - Images : nuances de gris, detection de format par signature, plan vide." & vbCrLf &
            "  - Controles WinForms : style de grille, double buffer, boutons arrondis, combo." & vbCrLf &
            "  - Systeme et OS : memoire, utilisateur, culture, serialisation XML." & vbCrLf &
            "  - Reseau et processus : adresse IP, moniteur en arriere-plan, processus." & vbCrLf & vbCrLf &
            "Le journal en bas de fenetre trace en direct le cycle de vie des connexions" & vbCrLf &
            "et des requetes : c'est le meilleur moyen d'observer les techniques a l'oeuvre." & vbCrLf & vbCrLf &
            "Pre-requis base de donnees : demarrez le conteneur via 'docker compose up -d'" & vbCrLf &
            "dans le dossier docker/ du projet, puis cliquez sur le bouton ci-dessous."

        Dim barre As New Panel() With {.Dock = DockStyle.Bottom, .Height = 70}
        Dim btnTest As Button = Bouton("Tester la connexion a la base", AddressOf SurTestConnexion)
        btnTest.Location = New Point(4, 8)
        barre.Controls.Add(btnTest)
        barre.Controls.Add(_lblEtat)

        Contenu.Controls.Add(texte)
        Contenu.Controls.Add(barre)
    End Sub

    Private Sub SurTestConnexion(ByVal sender As Object, ByVal e As EventArgs)
        Dim message As String = ""
        Dim ok As Boolean = AccesDonnees.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

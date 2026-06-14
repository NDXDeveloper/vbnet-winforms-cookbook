' ============================================================================
'  PageAccueil.vb
'  Page d'accueil : presentation et test de connexion a la base.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Serialisation

''' <summary>Page d'accueil : presentation du projet et test de connexion.</summary>
Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {
        .AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Démonstration des techniques de sérialisation d'objets en .NET.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Ce projet illustre, de façon autonome, la sérialisation d'objets .NET et leur" & vbCrLf &
            "persistance, regroupées dans une bibliothèque réutilisable (NDX.Serialisation.Core)." & vbCrLf & vbCrLf &
            "Parcourez les pages à gauche :" & vbCrLf &
            "  - Sérialisation et formats : XML, contrat XML, binaire compact et JSON ;" & vbCrLf &
            "    aller-retour (sérialiser puis désérialiser) et comparaison des tailles." & vbCrLf &
            "  - Fichiers et stockage isolé : enregistrer / recharger depuis un fichier ou" & vbCrLf &
            "    depuis le stockage isolé de l'utilisateur." & vbCrLf &
            "  - Persistance en base : enregistrer un objet sérialisé (BLOB + empreinte" & vbCrLf &
            "    SHA-256) dans MariaDB, le relister, le recharger et vérifier son intégrité." & vbCrLf & vbCrLf &
            "Quatre formats sont proposés, tous intégrés à .NET Framework. La sérialisation" & vbCrLf &
            "binaire historique (BinaryFormatter) est volontairement écartée (obsolète et" & vbCrLf &
            "exposée à des attaques par désérialisation) au profit d'un binaire sûr." & vbCrLf & vbCrLf &
            "Pré-requis base de données : démarrez le conteneur via 'docker compose up -d'" & vbCrLf &
            "dans le dossier docker/, puis cliquez sur le bouton ci-dessous."

        Dim barre As New Panel() With {.Dock = DockStyle.Bottom, .Height = 74}
        Dim btnTest As Button = Bouton("Tester la connexion à la base", AddressOf SurTestConnexion)
        btnTest.Location = New Point(4, 8)
        barre.Controls.Add(btnTest)
        barre.Controls.Add(_lblEtat)

        Contenu.Controls.Add(texte)
        Contenu.Controls.Add(barre)
    End Sub

    Private Sub SurTestConnexion(ByVal sender As Object, ByVal e As EventArgs)
        Dim message As String = ""
        Dim ok As Boolean = DepotDocuments.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

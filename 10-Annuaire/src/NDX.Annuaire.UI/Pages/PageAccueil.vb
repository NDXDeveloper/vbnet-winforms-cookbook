' ============================================================================
'  PageAccueil.vb  -  Presentation + test de connexion a l'annuaire.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Annuaire

Public NotInheritable Class PageAccueil
    Inherits PageBase

    Private ReadOnly _lblEtat As New Label() With {.AutoSize = True, .Location = New Point(4, 44), .Font = New Font("Segoe UI", 9.5F)}

    Public Sub New()
        MyBase.New("Accueil", "Authentification sur un annuaire LDAP : « bind », recherche d'attributs et protection contre l'injection.")
        Construire()
    End Sub

    Private Sub Construire()
        Dim texte As New TextBox() With {
            .Multiline = True, .ReadOnly = True, .ScrollBars = ScrollBars.Vertical, .Dock = DockStyle.Fill,
            .BorderStyle = BorderStyle.None, .BackColor = Color.White, .Font = New Font("Segoe UI", 10.0F)}
        texte.Text =
            "Cette bibliothèque (NDX.Annuaire.Core) authentifie des comptes via un annuaire LDAP :" & vbCrLf & vbCrLf &
            "  - ServiceAnnuaire.Authentifier : « bind » LDAP (le serveur valide lui-même" & vbCrLf &
            "    le couple DN + mot de passe ; aucune empreinte gérée par l'application) ;" & vbCrLf &
            "  - lecture des attributs du compte (uid, cn, mail) après authentification ;" & vbCrLf &
            "  - ListerUtilisateurs : recherche des comptes (bind administrateur) ;" & vbCrLf &
            "  - EchappeurLdap : échappement des filtres (RFC 4515) contre l'injection LDAP." & vbCrLf & vbCrLf &
            "Annuaire de démonstration (OpenLDAP) : base « dc=exemple,dc=test », comptes" & vbCrLf &
            "« alice » et « bob » (mots de passe alice_pwd / bob_pwd)." & vbCrLf & vbCrLf &
            "Pages : « Connexion » (s'authentifier) et « Annuaire » (lister les comptes)." & vbCrLf & vbCrLf &
            "Pré-requis : démarrez le conteneur (docker compose up -d), puis testez ci-dessous."

        Dim barre As New Panel() With {.Dock = DockStyle.Bottom, .Height = 74}
        Dim btn As Button = Bouton("Tester la connexion à l'annuaire", AddressOf SurTest)
        btn.Location = New Point(4, 8)
        barre.Controls.Add(btn)
        barre.Controls.Add(_lblEtat)

        Contenu.Controls.Add(texte)
        Contenu.Controls.Add(barre)
    End Sub

    Private Sub SurTest(ByVal sender As Object, ByVal e As EventArgs)
        Dim message As String = ""
        Dim ok As Boolean = ServiceAnnuaire.TesterConnexion(message)
        _lblEtat.ForeColor = If(ok, Color.Green, Color.Firebrick)
        _lblEtat.Text = message
    End Sub

End Class

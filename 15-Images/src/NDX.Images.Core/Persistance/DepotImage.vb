' ============================================================================
'  DepotImage.vb  -  Mediatheque : metadonnees + vignette PNG en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports System.Drawing
Imports System.IO
Imports MySql.Data.MySqlClient

''' <summary>
''' Enregistre une image : on conserve son nom, ses dimensions et une <b>vignette</b>
''' (PNG) en base. On stocke la version réduite plutôt que l'originale, pour une
''' médiathèque légère et rapide à afficher.
''' </summary>
Public Module DepotImage

    ''' <summary>Charge une image, en calcule la vignette, et enregistre le tout ; renvoie l'identifiant.</summary>
    Public Function Enregistrer(ByVal nom As String, ByVal donneesOriginales As Byte()) As Integer
        Dim largeur As Integer, hauteur As Integer
        Dim vignettePng As Byte()
        Using flux As New MemoryStream(donneesOriginales)
            Using image As Image = Image.FromStream(flux)
                largeur = image.Width
                hauteur = image.Height
                Using v As Bitmap = Vignette.Generer(image)
                    vignettePng = Vignette.VersPng(v)
                End Using
            End Using
        End Using

        Const requete As String =
            "INSERT INTO image (nom, largeur, hauteur, vignette) VALUES (@n, @l, @h, @v); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@n", nom)
                cmd.Parameters.AddWithValue("@l", largeur)
                cmd.Parameters.AddWithValue("@h", hauteur)
                cmd.Parameters.AddWithValue("@v", vignettePng)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Relit la vignette PNG d'une image.</summary>
    Public Function ChargerVignette(ByVal id As Integer) As Byte()
        Const requete As String = "SELECT vignette FROM image WHERE id = @id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@id", id)
                Dim r As Object = cmd.ExecuteScalar()
                If r Is Nothing OrElse Convert.IsDBNull(r) Then Throw New Exception("Image introuvable : " & id.ToString())
                Return DirectCast(r, Byte())
            End Using
        End Using
    End Function

    ''' <summary>Vue tabulaire des images (sans le binaire).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, nom AS `Nom`, largeur AS `Largeur`, hauteur AS `Hauteur`, cree_le AS `Ajoutée le` " &
            "FROM image ORDER BY id DESC;"
        Dim table As New DataTable()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using adaptateur As New MySqlDataAdapter(requete, cn)
                adaptateur.Fill(table)
            End Using
        End Using
        Return table
    End Function

    Public Function TesterConnexion(ByRef message As String) As Boolean
        Try
            Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
                cn.Open()
                Using cmd As New MySqlCommand("SELECT VERSION();", cn)
                    message = "Connexion OK - serveur : " & Convert.ToString(cmd.ExecuteScalar())
                    Return True
                End Using
            End Using
        Catch ex As Exception
            message = "Echec de connexion : " & ex.Message
            Return False
        End Try
    End Function

End Module

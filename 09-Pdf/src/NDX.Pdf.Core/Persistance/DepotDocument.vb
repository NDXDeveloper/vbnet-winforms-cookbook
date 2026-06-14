' ============================================================================
'  DepotDocument.vb  -  Bibliotheque des PDF generes (stockage + relecture).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient

''' <summary>
''' Archive les documents PDF produits (le binaire + ses métadonnées) et les relit
''' à l'identique. Le PDF généré par <see cref="DocumentPdf"/> est stocké en
''' <c>LONGBLOB</c> avec son empreinte SHA-256.
''' </summary>
Public Module DepotDocument

    ''' <summary>Enregistre un PDF généré ; renvoie l'identifiant créé.</summary>
    Public Function Enregistrer(ByVal titre As String, ByVal auteur As String, ByVal nbPages As Integer, ByVal pdf As Byte()) As Integer
        Const requete As String =
            "INSERT INTO document_genere (titre, auteur, nb_pages, taille_octets, contenu, empreinte_sha256) " &
            "VALUES (@t, @a, @n, @taille, @contenu, @emp); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@t", titre)
                cmd.Parameters.AddWithValue("@a", auteur)
                cmd.Parameters.AddWithValue("@n", nbPages)
                cmd.Parameters.AddWithValue("@taille", pdf.Length)
                cmd.Parameters.AddWithValue("@contenu", pdf)
                cmd.Parameters.AddWithValue("@emp", Sha256Hex(pdf))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Relit le binaire PDF d'un document archivé.</summary>
    Public Function Recharger(ByVal id As Integer) As Byte()
        Const requete As String = "SELECT contenu FROM document_genere WHERE id = @id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@id", id)
                Dim r As Object = cmd.ExecuteScalar()
                If r Is Nothing OrElse Convert.IsDBNull(r) Then Throw New Exception("Document introuvable : " & id.ToString())
                Return DirectCast(r, Byte())
            End Using
        End Using
    End Function

    ''' <summary>Liste les documents archivés (sans le binaire).</summary>
    Public Function Lister() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, titre AS `Titre`, auteur AS `Auteur`, nb_pages AS `Pages`, " &
            "taille_octets AS `Octets`, cree_le AS `Créé le` " &
            "FROM document_genere ORDER BY id DESC;"
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

    Private Function Sha256Hex(ByVal donnees As Byte()) As String
        Using algo As SHA256 = SHA256.Create()
            Dim emp As Byte() = algo.ComputeHash(donnees)
            Dim sb As New StringBuilder(emp.Length * 2)
            For Each o As Byte In emp
                sb.Append(o.ToString("x2"))
            Next
            Return sb.ToString()
        End Using
    End Function

End Module

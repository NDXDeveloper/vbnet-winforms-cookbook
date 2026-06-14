' ============================================================================
'  DepotArchive.vb  -  "Compress-then-store" : archive compressee en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient

''' <summary>
''' Enregistre des donnees <b>compressees</b> en base (avec tailles originale et
''' compressee + empreinte), puis les relit et les decompresse a l'identique.
''' Illustre la technique « compress-then-store » et le gain de place.
''' </summary>
Public Module DepotArchive

    ''' <summary>Compresse puis enregistre des donnees ; renvoie l'identifiant cree.</summary>
    Public Function Enregistrer(ByVal nom As String, ByVal donnees As Byte(),
                                Optional ByVal algo As AlgorithmeCompression = AlgorithmeCompression.GZip) As Integer
        Dim compresse As Byte() = Compresseur.Compresser(donnees, algo)
        Const requete As String =
            "INSERT INTO archive (nom, algorithme, taille_originale, taille_compressee, contenu, empreinte_sha256) " &
            "VALUES (@nom, @algo, @to, @tc, @contenu, @emp); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@nom", nom)
                cmd.Parameters.AddWithValue("@algo", CodeAlgo(algo))
                cmd.Parameters.AddWithValue("@to", donnees.Length)
                cmd.Parameters.AddWithValue("@tc", compresse.Length)
                cmd.Parameters.AddWithValue("@contenu", compresse)
                cmd.Parameters.AddWithValue("@emp", Sha256Hex(donnees))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Relit une archive et la decompresse vers les octets d'origine.</summary>
    Public Function Recharger(ByVal id As Integer) As Byte()
        Const requete As String = "SELECT contenu, algorithme FROM archive WHERE id = @id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@id", id)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    If Not lecteur.Read() Then Throw New Exception("Archive introuvable : " & id.ToString())
                    Dim compresse As Byte() = DirectCast(lecteur("contenu"), Byte())
                    Dim algo As AlgorithmeCompression = AlgoDepuisCode(Convert.ToString(lecteur("algorithme")))
                    Return Compresseur.Decompresser(compresse, algo)
                End Using
            End Using
        End Using
    End Function

    ''' <summary>Liste les archives (avec gain de place calcule).</summary>
    Public Function Lister() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, nom AS `Nom`, algorithme AS `Algo`, " &
            "taille_originale AS `Octets origine`, taille_compressee AS `Octets compresses`, " &
            "ROUND(100 * (1 - taille_compressee / NULLIF(taille_originale,0)), 1) AS `Gain %`, cree_le AS `Cree le` " &
            "FROM archive ORDER BY id DESC;"
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

    Private Function CodeAlgo(ByVal algo As AlgorithmeCompression) As String
        Return If(algo = AlgorithmeCompression.GZip, "gzip", "deflate")
    End Function

    Private Function AlgoDepuisCode(ByVal code As String) As AlgorithmeCompression
        Return If(String.Equals(code, "deflate", StringComparison.OrdinalIgnoreCase),
                  AlgorithmeCompression.Deflate, AlgorithmeCompression.GZip)
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

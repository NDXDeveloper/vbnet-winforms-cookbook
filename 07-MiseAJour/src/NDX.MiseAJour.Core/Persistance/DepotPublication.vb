' ============================================================================
'  DepotPublication.vb  -  Manifeste de deploiement persiste en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' Lit et alimente le manifeste des versions publiées (table <c>publication</c>).
''' C'est la source que <see cref="ServiceMiseAJour"/> interroge pour décider
''' s'il existe une mise à jour.
''' </summary>
Public Module DepotPublication

    ''' <summary>Insère une publication ; renvoie l'identifiant créé.</summary>
    Public Function Publier(ByVal p As Publication) As Integer
        Const requete As String =
            "INSERT INTO publication (version, notes, url_paquet, empreinte_sha256, obligatoire, publiee_le) " &
            "VALUES (@v, @n, @u, @e, @o, @d); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@v", p.Version.ToString())
                cmd.Parameters.AddWithValue("@n", If(p.Notes, CType(DBNull.Value, Object)))
                cmd.Parameters.AddWithValue("@u", If(p.UrlPaquet, CType(DBNull.Value, Object)))
                cmd.Parameters.AddWithValue("@e", If(p.EmpreinteSha256, CType(DBNull.Value, Object)))
                cmd.Parameters.AddWithValue("@o", p.Obligatoire)
                cmd.Parameters.AddWithValue("@d", p.PublieeLe)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Charge toutes les publications (objets typés).</summary>
    Public Function Lister() As List(Of Publication)
        Const requete As String =
            "SELECT version, notes, url_paquet, empreinte_sha256, obligatoire, publiee_le " &
            "FROM publication ORDER BY publiee_le DESC;"
        Dim liste As New List(Of Publication)()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        liste.Add(New Publication() With {
                            .Version = VersionSemantique.Analyser(Convert.ToString(lecteur("version"))),
                            .Notes = SiNull(lecteur("notes")),
                            .UrlPaquet = SiNull(lecteur("url_paquet")),
                            .EmpreinteSha256 = SiNull(lecteur("empreinte_sha256")),
                            .Obligatoire = Convert.ToBoolean(lecteur("obligatoire")),
                            .PublieeLe = Convert.ToDateTime(lecteur("publiee_le"))})
                    End While
                End Using
            End Using
        End Using
        Return liste
    End Function

    ''' <summary>Vue tabulaire des publications (pour la grille).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT version AS `Version`, IF(obligatoire, 'oui', 'non') AS `Obligatoire`, " &
            "publiee_le AS `Publiée le`, notes AS `Notes` " &
            "FROM publication ORDER BY publiee_le DESC;"
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

    Private Function SiNull(ByVal valeur As Object) As String
        Return If(valeur Is Nothing OrElse Convert.IsDBNull(valeur), Nothing, Convert.ToString(valeur))
    End Function

End Module

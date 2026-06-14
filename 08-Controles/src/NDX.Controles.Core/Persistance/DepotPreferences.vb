' ============================================================================
'  DepotPreferences.vb  -  Stockage clef/valeur des preferences (upsert).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' Persiste l'état des contrôles (préférences clef/valeur). L'écriture utilise un
''' <b>upsert</b> (<c>INSERT ... ON DUPLICATE KEY UPDATE</c>) : créer ou mettre à
''' jour en une seule requête.
''' </summary>
Public Module DepotPreferences

    ''' <summary>Lit une préférence ; renvoie <paramref name="defaut"/> si absente.</summary>
    Public Function Lire(ByVal cle As String, Optional ByVal defaut As String = Nothing) As String
        Const requete As String = "SELECT valeur FROM preference WHERE cle = @cle;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@cle", cle)
                Dim r As Object = cmd.ExecuteScalar()
                Return If(r Is Nothing OrElse Convert.IsDBNull(r), defaut, Convert.ToString(r))
            End Using
        End Using
    End Function

    ''' <summary>Crée ou met à jour une préférence.</summary>
    Public Sub Ecrire(ByVal cle As String, ByVal valeur As String)
        Const requete As String =
            "INSERT INTO preference (cle, valeur) VALUES (@cle, @val) " &
            "ON DUPLICATE KEY UPDATE valeur = @val, maj_le = CURRENT_TIMESTAMP;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@cle", cle)
                cmd.Parameters.AddWithValue("@val", If(valeur, CType(DBNull.Value, Object)))
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ''' <summary>Liste toutes les préférences (pour la grille).</summary>
    Public Function Lister() As DataTable
        Const requete As String =
            "SELECT cle AS `Clef`, valeur AS `Valeur`, maj_le AS `Mise à jour` " &
            "FROM preference ORDER BY cle;"
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

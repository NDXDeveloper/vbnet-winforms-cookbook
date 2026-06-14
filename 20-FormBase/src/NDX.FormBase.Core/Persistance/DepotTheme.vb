' ============================================================================
'  DepotTheme.vb  -  Themes persistes en base (couleurs en hexadecimal).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' Lit et écrit les thèmes en base : les couleurs y sont stockées en hexadécimal
''' (« #RRGGBB ») via <see cref="CouleurHex"/>, format lisible et portable.
''' </summary>
Public Module DepotTheme

    ''' <summary>Charge tous les thèmes enregistrés.</summary>
    Public Function Lister() As List(Of Theme)
        Const requete As String = "SELECT nom, fond_hex, texte_hex, accent_hex FROM theme ORDER BY nom;"
        Dim liste As New List(Of Theme)()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        liste.Add(New Theme(
                            Convert.ToString(lecteur("nom")),
                            CouleurHex.DepuisHex(Convert.ToString(lecteur("fond_hex"))),
                            CouleurHex.DepuisHex(Convert.ToString(lecteur("texte_hex"))),
                            CouleurHex.DepuisHex(Convert.ToString(lecteur("accent_hex")))))
                    End While
                End Using
            End Using
        End Using
        Return liste
    End Function

    ''' <summary>Crée ou met à jour un thème (upsert sur le nom).</summary>
    Public Sub Enregistrer(ByVal theme As Theme)
        Const requete As String =
            "INSERT INTO theme (nom, fond_hex, texte_hex, accent_hex) VALUES (@n, @f, @t, @a) " &
            "ON DUPLICATE KEY UPDATE fond_hex = @f, texte_hex = @t, accent_hex = @a, maj_le = CURRENT_TIMESTAMP;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@n", theme.Nom)
                cmd.Parameters.AddWithValue("@f", CouleurHex.VersHex(theme.Fond))
                cmd.Parameters.AddWithValue("@t", CouleurHex.VersHex(theme.Texte))
                cmd.Parameters.AddWithValue("@a", CouleurHex.VersHex(theme.Accent))
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ''' <summary>Vue tabulaire des thèmes (pour la grille).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT nom AS `Nom`, fond_hex AS `Fond`, texte_hex AS `Texte`, accent_hex AS `Accent`, maj_le AS `Mise à jour` " &
            "FROM theme ORDER BY nom;"
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

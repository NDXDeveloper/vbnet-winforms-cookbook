' ============================================================================
'  DepotSupervision.vb  -  Historique des echantillons de ressources.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Enregistre et liste des relevés (objets GDI/USER + inactivité) dans la table <c>echantillon</c>.</summary>
Public Module DepotSupervision

    ''' <summary>Enregistre un relevé ; renvoie l'identifiant créé.</summary>
    Public Function Enregistrer(ByVal objetsGdi As Integer, ByVal objetsUser As Integer, ByVal inactifMs As Long) As Integer
        Const requete As String =
            "INSERT INTO echantillon (objets_gdi, objets_user, inactif_ms) VALUES (@g, @u, @i); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@g", objetsGdi)
                cmd.Parameters.AddWithValue("@u", objetsUser)
                cmd.Parameters.AddWithValue("@i", inactifMs)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Vue tabulaire des relevés (les plus récents d'abord).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, objets_gdi AS `Objets GDI`, objets_user AS `Objets USER`, " &
            "inactif_ms AS `Inactif (ms)`, cree_le AS `Relevé le` " &
            "FROM echantillon ORDER BY id DESC LIMIT 200;"
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

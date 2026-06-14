' ============================================================================
'  DepotCommande.vb  -  Journal des commandes recues par l'instance primaire.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Journalise en base les commandes transmises par les instances secondaires.</summary>
Public Module DepotCommande

    ''' <summary>Enregistre une commande reçue ; renvoie l'identifiant créé.</summary>
    Public Function Enregistrer(ByVal source As String, ByVal arguments As String) As Integer
        Const requete As String =
            "INSERT INTO commande_recue (source, arguments) VALUES (@s, @a); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@s", source)
                cmd.Parameters.AddWithValue("@a", If(arguments, ""))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Vue tabulaire des commandes reçues (les plus récentes d'abord).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, source AS `Source`, arguments AS `Arguments`, recu_le AS `Reçue le` " &
            "FROM commande_recue ORDER BY id DESC LIMIT 200;"
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

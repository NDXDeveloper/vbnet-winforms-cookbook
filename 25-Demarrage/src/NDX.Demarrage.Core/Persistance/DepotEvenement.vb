' ============================================================================
'  DepotEvenement.vb  -  Journal des evenements de demarrage et d'exceptions.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Journalise les étapes de démarrage et les exceptions captées (table <c>evenement</c>).</summary>
Public Module DepotEvenement

    ''' <summary>Enregistre un événement ; renvoie l'identifiant créé.</summary>
    Public Function Enregistrer(ByVal type As String, ByVal message As String) As Integer
        Const requete As String =
            "INSERT INTO evenement (type, message) VALUES (@t, @m); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@t", type)
                cmd.Parameters.AddWithValue("@m", If(message, ""))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Vue tabulaire des événements (les plus récents d'abord).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, type AS `Type`, message AS `Message`, horodate AS `Horodatage` " &
            "FROM evenement ORDER BY id DESC LIMIT 200;"
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

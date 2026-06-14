' ============================================================================
'  DepotMetadonnees.vb  -  Catalogue des membres decouverts, persiste en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Enregistre la description (membres) d'un type dans la table <c>descripteur</c>.</summary>
Public Module DepotMetadonnees

    ''' <summary>Inspecte un type et enregistre tous ses membres ; renvoie le nombre inséré.</summary>
    Public Function EnregistrerDescription(ByVal t As Type) As Integer
        Dim membres As List(Of DescripteurMembre) = InspecteurType.ListerTout(t)
        Const requete As String =
            "INSERT INTO descripteur (type_complet, membre, genre, type_associe) VALUES (@t, @m, @g, @ta);"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            For Each d As DescripteurMembre In membres
                Using cmd As New MySqlCommand(requete, cn)
                    cmd.Parameters.AddWithValue("@t", t.FullName)
                    cmd.Parameters.AddWithValue("@m", d.Nom)
                    cmd.Parameters.AddWithValue("@g", d.Genre)
                    cmd.Parameters.AddWithValue("@ta", d.TypeAssocie)
                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Using
        Return membres.Count
    End Function

    ''' <summary>Vue tabulaire des descripteurs (les plus récents d'abord).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, type_complet AS `Type`, membre AS `Membre`, genre AS `Genre`, " &
            "type_associe AS `Type associé`, releve_le AS `Relevé le` " &
            "FROM descripteur ORDER BY id DESC LIMIT 500;"
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

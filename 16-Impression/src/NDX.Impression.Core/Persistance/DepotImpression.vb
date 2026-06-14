' ============================================================================
'  DepotImpression.vb  -  Historique / modeles de documents a imprimer.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Conserve les documents soumis à l'impression (titre + contenu + nb de pages).</summary>
Public Module DepotImpression

    ''' <summary>Enregistre un document ; renvoie l'identifiant créé.</summary>
    Public Function Enregistrer(ByVal titre As String, ByVal contenu As String, ByVal nbPages As Integer) As Integer
        Const requete As String =
            "INSERT INTO impression (titre, contenu, nb_pages) VALUES (@t, @c, @n); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@t", titre)
                cmd.Parameters.AddWithValue("@c", contenu)
                cmd.Parameters.AddWithValue("@n", nbPages)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Relit le contenu d'un document archivé.</summary>
    Public Function Recharger(ByVal id As Integer) As String
        Const requete As String = "SELECT contenu FROM impression WHERE id = @id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@id", id)
                Dim r As Object = cmd.ExecuteScalar()
                Return If(r Is Nothing OrElse Convert.IsDBNull(r), "", Convert.ToString(r))
            End Using
        End Using
    End Function

    ''' <summary>Vue tabulaire (sans le contenu).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, titre AS `Titre`, nb_pages AS `Pages`, cree_le AS `Imprimé le` " &
            "FROM impression ORDER BY id DESC;"
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

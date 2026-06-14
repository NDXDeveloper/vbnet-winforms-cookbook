' ============================================================================
'  DepotTache.vb  -  File d'attente de taches persistee en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' File d'attente de tâches (table <c>tache</c>). Le moteur asynchrone vide la
''' file en arrière-plan : chaque tâche passe de <c>en_attente</c> à <c>traitee</c>.
''' </summary>
Public Module DepotTache

    ''' <summary>Empile une tâche ; renvoie l'identifiant créé.</summary>
    Public Function Empiler(ByVal libelle As String, Optional ByVal charge As String = Nothing) As Integer
        Const requete As String =
            "INSERT INTO tache (libelle, charge_utile, etat) VALUES (@l, @c, 'en_attente'); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@l", libelle)
                cmd.Parameters.AddWithValue("@c", If(charge, CType(DBNull.Value, Object)))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Liste les tâches en attente (les plus anciennes d'abord).</summary>
    Public Function ListerEnAttente() As List(Of Tache)
        Const requete As String =
            "SELECT id, libelle, charge_utile, etat, cree_le FROM tache WHERE etat = 'en_attente' ORDER BY id;"
        Return Charger(requete)
    End Function

    ''' <summary>Marque une tâche comme traitée.</summary>
    Public Sub MarquerTraitee(ByVal id As Integer)
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand("UPDATE tache SET etat = 'traitee', traitee_le = CURRENT_TIMESTAMP WHERE id = @id;", cn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ''' <summary>Vue tabulaire de toutes les tâches (pour la grille).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, libelle AS `Libellé`, etat AS `État`, cree_le AS `Créée le`, traitee_le AS `Traitée le` " &
            "FROM tache ORDER BY id DESC;"
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

    Private Function Charger(ByVal requete As String) As List(Of Tache)
        Dim liste As New List(Of Tache)()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        liste.Add(New Tache() With {
                            .Id = Convert.ToInt32(lecteur("id")),
                            .Libelle = Convert.ToString(lecteur("libelle")),
                            .Charge = If(Convert.IsDBNull(lecteur("charge_utile")), Nothing, Convert.ToString(lecteur("charge_utile"))),
                            .Etat = Convert.ToString(lecteur("etat")),
                            .CreeLe = Convert.ToDateTime(lecteur("cree_le"))})
                    End While
                End Using
            End Using
        End Using
        Return liste
    End Function

End Module

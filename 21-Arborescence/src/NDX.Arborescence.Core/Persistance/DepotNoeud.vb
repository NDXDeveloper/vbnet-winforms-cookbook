' ============================================================================
'  DepotNoeud.vb  -  Arborescence persistee (liste d'adjacence + CTE recursive).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' Stocke l'arbre « à plat » (table <c>noeud</c> avec <c>parent_id</c>) et sait
''' récupérer tous les descendants d'un nœud via une <b>requête récursive</b>
''' (<c>WITH RECURSIVE</c>), gérée nativement par MariaDB.
''' </summary>
Public Module DepotNoeud

    ''' <summary>Charge tous les nœuds à plat (ordonnés par identifiant).</summary>
    Public Function Lister() As List(Of Noeud)
        Const requete As String = "SELECT id, parent_id, libelle, categorie FROM noeud ORDER BY id;"
        Dim liste As New List(Of Noeud)()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        liste.Add(New Noeud() With {
                            .Id = Convert.ToInt32(lecteur("id")),
                            .ParentId = If(Convert.IsDBNull(lecteur("parent_id")), CType(Nothing, Integer?), Convert.ToInt32(lecteur("parent_id"))),
                            .Libelle = Convert.ToString(lecteur("libelle")),
                            .Categorie = Convert.ToString(lecteur("categorie"))})
                    End While
                End Using
            End Using
        End Using
        Return liste
    End Function

    ''' <summary>Ajoute un nœud ; renvoie l'identifiant créé.</summary>
    Public Function Ajouter(ByVal parentId As Integer?, ByVal libelle As String, Optional ByVal categorie As String = "") As Integer
        Const requete As String =
            "INSERT INTO noeud (parent_id, libelle, categorie) VALUES (@p, @l, @c); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@p", If(parentId.HasValue, CType(parentId.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@l", libelle)
                cmd.Parameters.AddWithValue("@c", If(categorie, ""))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Supprime un nœud (ses enfants sont supprimés en cascade par la base).</summary>
    Public Sub Supprimer(ByVal id As Integer)
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand("DELETE FROM noeud WHERE id = @id;", cn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ''' <summary>Tous les descendants d'un nœud (hors lui-même), via requête récursive.</summary>
    Public Function Descendants(ByVal id As Integer) As DataTable
        Const requete As String =
            "WITH RECURSIVE arbre AS (" &
            "  SELECT id, parent_id, libelle, 0 AS profondeur FROM noeud WHERE id = @id" &
            "  UNION ALL" &
            "  SELECT n.id, n.parent_id, n.libelle, a.profondeur + 1 FROM noeud n JOIN arbre a ON n.parent_id = a.id" &
            ") SELECT id AS `Id`, libelle AS `Libellé`, profondeur AS `Profondeur` FROM arbre WHERE id <> @id ORDER BY profondeur, id;"
        Dim table As New DataTable()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@id", id)
                Using adaptateur As New MySqlDataAdapter(cmd)
                    adaptateur.Fill(table)
                End Using
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

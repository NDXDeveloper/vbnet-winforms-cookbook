' ============================================================================
'  DepotLien.vb  -  Catalogue de liens / raccourcis, persiste en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' Stocke le catalogue des liens (applications .lnk et liens Web .url). La galerie
''' peut ensuite « exporter » chaque entrée en un fichier de raccourci réel.
''' </summary>
Public Module DepotLien

    ''' <summary>Ajoute une entrée ; renvoie l'identifiant créé.</summary>
    Public Function Ajouter(ByVal lien As Lien) As Integer
        Const requete As String =
            "INSERT INTO lien (categorie, nom, cible, description) VALUES (@c, @n, @ci, @d); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@c", lien.Categorie)
                cmd.Parameters.AddWithValue("@n", lien.Nom)
                cmd.Parameters.AddWithValue("@ci", lien.Cible)
                cmd.Parameters.AddWithValue("@d", If(lien.Description, CType(DBNull.Value, Object)))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Supprime une entrée par identifiant.</summary>
    Public Sub Supprimer(ByVal id As Integer)
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand("DELETE FROM lien WHERE id = @id;", cn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ''' <summary>Charge toutes les entrées (objets typés).</summary>
    Public Function ListerObjets() As List(Of Lien)
        Const requete As String = "SELECT categorie, nom, cible, description FROM lien ORDER BY categorie, nom;"
        Dim liste As New List(Of Lien)()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        liste.Add(New Lien() With {
                            .Categorie = Convert.ToString(lecteur("categorie")),
                            .Nom = Convert.ToString(lecteur("nom")),
                            .Cible = Convert.ToString(lecteur("cible")),
                            .Description = SiNull(lecteur("description"))})
                    End While
                End Using
            End Using
        End Using
        Return liste
    End Function

    ''' <summary>Vue tabulaire (pour la grille).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, categorie AS `Catégorie`, nom AS `Nom`, cible AS `Cible`, description AS `Description` " &
            "FROM lien ORDER BY categorie, nom;"
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

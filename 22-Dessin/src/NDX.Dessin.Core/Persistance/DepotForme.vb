' ============================================================================
'  DepotForme.vb  -  Sauvegarde / chargement d'une scene de formes.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Persiste la scène : on enregistre toutes les formes (dans une transaction) et on les recharge.</summary>
Public Module DepotForme

    ''' <summary>Remplace le contenu de la table par les formes de la scène (transaction).</summary>
    Public Sub EnregistrerScene(ByVal scene As Scene)
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using tx As MySqlTransaction = cn.BeginTransaction()
                Using vider As New MySqlCommand("DELETE FROM forme;", cn, tx)
                    vider.ExecuteNonQuery()
                End Using
                For Each f As Forme In scene.Formes
                    Using cmd As New MySqlCommand(
                        "INSERT INTO forme (type, x, y, largeur, hauteur, couleur_hex) VALUES (@t, @x, @y, @l, @h, @c);", cn, tx)
                        cmd.Parameters.AddWithValue("@t", CInt(f.Type))
                        cmd.Parameters.AddWithValue("@x", f.X)
                        cmd.Parameters.AddWithValue("@y", f.Y)
                        cmd.Parameters.AddWithValue("@l", f.Largeur)
                        cmd.Parameters.AddWithValue("@h", f.Hauteur)
                        cmd.Parameters.AddWithValue("@c", f.CouleurHex)
                        cmd.ExecuteNonQuery()
                    End Using
                Next
                tx.Commit()
            End Using
        End Using
    End Sub

    ''' <summary>Recharge la scène depuis la base.</summary>
    Public Function ChargerScene() As Scene
        Dim scene As New Scene()
        Const requete As String = "SELECT type, x, y, largeur, hauteur, couleur_hex FROM forme ORDER BY id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        scene.Ajouter(New Forme() With {
                            .Type = CType(Convert.ToInt32(lecteur("type")), TypeForme),
                            .X = Convert.ToInt32(lecteur("x")),
                            .Y = Convert.ToInt32(lecteur("y")),
                            .Largeur = Convert.ToInt32(lecteur("largeur")),
                            .Hauteur = Convert.ToInt32(lecteur("hauteur")),
                            .CouleurHex = Convert.ToString(lecteur("couleur_hex"))})
                    End While
                End Using
            End Using
        End Using
        Return scene
    End Function

    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, type AS `Type`, x AS `X`, y AS `Y`, largeur AS `Largeur`, hauteur AS `Hauteur`, couleur_hex AS `Couleur` " &
            "FROM forme ORDER BY id;"
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

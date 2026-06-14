' ============================================================================
'  DepotArticle.vb  -  Donnees affichees dans la grille personnalisee.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Fournit les articles affichés par la grille personnalisée.</summary>
Public Module DepotArticle

    ''' <summary>Vue tabulaire des articles.</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT reference AS `Référence`, designation AS `Désignation`, prix AS `Prix`, stock AS `Stock` " &
            "FROM article ORDER BY reference;"
        Dim table As New DataTable()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using adaptateur As New MySqlDataAdapter(requete, cn)
                adaptateur.Fill(table)
            End Using
        End Using
        Return table
    End Function

    ''' <summary>Ajoute un article.</summary>
    Public Sub Ajouter(ByVal reference As String, ByVal designation As String, ByVal prix As Decimal, ByVal stock As Integer)
        Const requete As String = "INSERT INTO article (reference, designation, prix, stock) VALUES (@r, @d, @p, @s);"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@r", reference)
                cmd.Parameters.AddWithValue("@d", designation)
                cmd.Parameters.AddWithValue("@p", prix)
                cmd.Parameters.AddWithValue("@s", stock)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

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

' ============================================================================
'  DepotDocument.vb  -  Documents enrichis (RTF) persistes en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Enregistre et relit des documents au format RTF (texte enrichi).</summary>
Public Module DepotDocument

    ''' <summary>Enregistre un document ; renvoie l'identifiant créé.</summary>
    Public Function Enregistrer(ByVal titre As String, ByVal contenuRtf As String) As Integer
        Const requete As String =
            "INSERT INTO document (titre, contenu_rtf) VALUES (@t, @c); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@t", titre)
                cmd.Parameters.AddWithValue("@c", If(contenuRtf, ""))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Relit le contenu RTF d'un document.</summary>
    Public Function Recharger(ByVal id As Integer) As String
        Const requete As String = "SELECT contenu_rtf FROM document WHERE id = @id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@id", id)
                Dim r As Object = cmd.ExecuteScalar()
                Return If(r Is Nothing OrElse Convert.IsDBNull(r), "", Convert.ToString(r))
            End Using
        End Using
    End Function

    ''' <summary>Entêtes (id, titre) pour alimenter une liste déroulante.</summary>
    Public Function ListerEntetes() As List(Of KeyValuePair(Of Integer, String))
        Const requete As String = "SELECT id, titre FROM document ORDER BY id DESC;"
        Dim liste As New List(Of KeyValuePair(Of Integer, String))()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        liste.Add(New KeyValuePair(Of Integer, String)(Convert.ToInt32(lecteur("id")), Convert.ToString(lecteur("titre"))))
                    End While
                End Using
            End Using
        End Using
        Return liste
    End Function

    ''' <summary>Vue tabulaire (sans le RTF).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, titre AS `Titre`, CHAR_LENGTH(contenu_rtf) AS `Taille RTF`, cree_le AS `Créé le` " &
            "FROM document ORDER BY id DESC;"
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

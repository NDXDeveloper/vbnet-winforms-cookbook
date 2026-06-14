' ============================================================================
'  DepotRaccourci.vb  -  Liaisons action <-> raccourci, persistees en base.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' Stocke les liaisons « action ↔ raccourci » (table <c>liaison_clavier</c>) et
''' les recharge dans un <see cref="GestionnaireRaccourcis"/>. L'écriture est un
''' upsert (<c>INSERT ... ON DUPLICATE KEY UPDATE</c>).
''' </summary>
Public Module DepotRaccourci

    ''' <summary>Crée ou met à jour la liaison d'une action.</summary>
    Public Sub Definir(ByVal action As String, ByVal raccourciTexte As String)
        ' On valide (et normalise) le raccourci avant de l'enregistrer.
        Dim normalise As String = Raccourci.Analyser(raccourciTexte).Texte
        Const requete As String =
            "INSERT INTO liaison_clavier (action, raccourci) VALUES (@a, @r) " &
            "ON DUPLICATE KEY UPDATE raccourci = @r, maj_le = CURRENT_TIMESTAMP;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@a", action)
                cmd.Parameters.AddWithValue("@r", normalise)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ''' <summary>Charge les liaisons (action, raccourci).</summary>
    Public Function Charger() As List(Of KeyValuePair(Of String, String))
        Const requete As String = "SELECT action, raccourci FROM liaison_clavier ORDER BY action;"
        Dim liste As New List(Of KeyValuePair(Of String, String))()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        liste.Add(New KeyValuePair(Of String, String)(Convert.ToString(lecteur("action")), Convert.ToString(lecteur("raccourci"))))
                    End While
                End Using
            End Using
        End Using
        Return liste
    End Function

    ''' <summary>Construit un gestionnaire à partir des liaisons enregistrées.</summary>
    Public Function ConstruireGestionnaire() As GestionnaireRaccourcis
        Dim g As New GestionnaireRaccourcis()
        For Each liaison As KeyValuePair(Of String, String) In Charger()
            g.Inscrire(liaison.Key, liaison.Value)
        Next
        Return g
    End Function

    ''' <summary>Vue tabulaire (pour la grille).</summary>
    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT action AS `Action`, raccourci AS `Raccourci`, maj_le AS `Mise à jour` " &
            "FROM liaison_clavier ORDER BY action;"
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

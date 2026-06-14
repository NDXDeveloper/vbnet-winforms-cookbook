' ============================================================================
'  DepotMesure.vb  -  Serie de mesures persistee (source du graphique).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Charge / alimente la série de mesures affichée par le graphique (table <c>mesure</c>).</summary>
Public Module DepotMesure

    ''' <summary>Construit la série à tracer à partir des mesures en base.</summary>
    Public Function ChargerSerie() As SerieDonnees
        Dim serie As New SerieDonnees() With {.Nom = "Mesures"}
        Const requete As String = "SELECT libelle, valeur FROM mesure ORDER BY ordre, id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        serie.Ajouter(Convert.ToString(lecteur("libelle")), Convert.ToDouble(lecteur("valeur")))
                    End While
                End Using
            End Using
        End Using
        Return serie
    End Function

    ''' <summary>Ajoute une mesure (placée en fin de série).</summary>
    Public Sub Ajouter(ByVal libelle As String, ByVal valeur As Double)
        Const requete As String =
            "INSERT INTO mesure (libelle, valeur, ordre) VALUES (@l, @v, (SELECT COALESCE(MAX(ordre), 0) + 1 FROM mesure AS m));"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@l", libelle)
                cmd.Parameters.AddWithValue("@v", valeur)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, ordre AS `Ordre`, libelle AS `Libellé`, valeur AS `Valeur` FROM mesure ORDER BY ordre, id;"
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

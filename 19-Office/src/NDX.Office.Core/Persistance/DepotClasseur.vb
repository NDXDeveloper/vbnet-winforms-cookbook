' ============================================================================
'  DepotClasseur.vb  -  Table de donnees a exporter / importer en .xlsx.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' Table simple (3 colonnes) servant de source/destination aux échanges .xlsx :
''' on peut exporter ses lignes vers un classeur et importer un classeur vers elle.
''' </summary>
Public Module DepotClasseur

    ''' <summary>Ajoute une ligne ; renvoie l'identifiant créé.</summary>
    Public Function Ajouter(ByVal valeur1 As String, ByVal valeur2 As String, ByVal valeur3 As String) As Integer
        Const requete As String =
            "INSERT INTO enregistrement (valeur1, valeur2, valeur3) VALUES (@a, @b, @c); SELECT LAST_INSERT_ID();"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@a", If(valeur1, ""))
                cmd.Parameters.AddWithValue("@b", If(valeur2, ""))
                cmd.Parameters.AddWithValue("@c", If(valeur3, ""))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Toutes les lignes sous forme de tableaux (avec un en-tête en première ligne).</summary>
    Public Function ListerLignes() As List(Of String())
        Dim lignes As New List(Of String())()
        lignes.Add(New String() {"Valeur 1", "Valeur 2", "Valeur 3"})
        Const requete As String = "SELECT valeur1, valeur2, valeur3 FROM enregistrement ORDER BY id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    While lecteur.Read()
                        lignes.Add(New String() {Convert.ToString(lecteur(0)), Convert.ToString(lecteur(1)), Convert.ToString(lecteur(2))})
                    End While
                End Using
            End Using
        End Using
        Return lignes
    End Function

    ''' <summary>Importe des lignes (la première, en-tête, est ignorée) ; renvoie le nombre inséré.</summary>
    Public Function Importer(ByVal lignes As List(Of String())) As Integer
        Dim insere As Integer = 0
        For i As Integer = 0 To lignes.Count - 1
            If i = 0 Then Continue For   ' ignorer l'en-tête
            Dim l As String() = lignes(i)
            Ajouter(Element(l, 0), Element(l, 1), Element(l, 2))
            insere += 1
        Next
        Return insere
    End Function

    Public Function ListerTable() As DataTable
        Const requete As String =
            "SELECT id AS `Id`, valeur1 AS `Valeur 1`, valeur2 AS `Valeur 2`, valeur3 AS `Valeur 3`, cree_le AS `Créé le` " &
            "FROM enregistrement ORDER BY id DESC;"
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

    Private Function Element(ByVal tableau As String(), ByVal index As Integer) As String
        Return If(tableau IsNot Nothing AndAlso index < tableau.Length, tableau(index), "")
    End Function

End Module

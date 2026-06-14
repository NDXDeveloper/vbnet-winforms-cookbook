' ============================================================================
'  SourceDonnees.vb  -  Requetes fournissant les DataTables a exporter.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>Source des donnees a exporter (base <c>entrepot</c>).</summary>
Public Module SourceDonnees

    ''' <summary>Liste detaillee des ventes.</summary>
    Public Function ToutesLesVentes() As DataTable
        Return RemplirTable(
            "SELECT date_vente AS `Date`, produit AS `Produit`, categorie AS `Categorie`, " &
            "quantite AS `Quantite`, montant AS `Montant` " &
            "FROM vente ORDER BY date_vente, id;")
    End Function

    ''' <summary>Synthese des ventes par categorie (agregats).</summary>
    Public Function VentesParCategorie() As DataTable
        Return RemplirTable(
            "SELECT categorie AS `Categorie`, COUNT(*) AS `Nb ventes`, " &
            "SUM(quantite) AS `Quantite`, ROUND(SUM(montant), 2) AS `Montant total` " &
            "FROM vente GROUP BY categorie ORDER BY `Montant total` DESC;")
    End Function

    ''' <summary>Teste la disponibilite de la base.</summary>
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

    Private Function RemplirTable(ByVal requete As String) As DataTable
        Dim table As New DataTable()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using adaptateur As New MySqlDataAdapter(requete, cn)
                adaptateur.Fill(table)
            End Using
        End Using
        Return table
    End Function

End Module

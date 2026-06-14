' ============================================================================
'  DepotProduit.vb  -  Depot (Repository) des produits.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports MySql.Data.MySqlClient

''' <summary>
''' Depot des <see cref="Produit"/> : CRUD par requetes parametrees, pagination,
''' mapping via <see cref="Mappeur"/>. Peut fonctionner de facon autonome (une
''' connexion par operation, protegee par <see cref="Resilience"/>) ou participer
''' a une <see cref="UniteDeTravail"/> (operations dans une meme transaction).
''' </summary>
Public NotInheritable Class DepotProduit
    Implements IDepot(Of Produit)

    ' Colonnes aliasees sur les noms de proprietes (pour le mapping par reflexion).
    Private Const COLONNES As String = "id AS Id, reference AS Reference, designation AS Designation, prix_ht AS PrixHt, stock AS Stock"

    Private ReadOnly _uow As UniteDeTravail

    ''' <summary>Depot autonome (chaque operation ouvre sa propre connexion).</summary>
    Public Sub New()
        _uow = Nothing
    End Sub

    ''' <summary>Depot rattache a une unite de travail (transaction partagee).</summary>
    Public Sub New(ByVal uow As UniteDeTravail)
        _uow = uow
    End Sub

    Public Function Lister(ByVal page As Integer, ByVal taille As Integer) As List(Of Produit) Implements IDepot(Of Produit).Lister
        If page < 1 Then page = 1
        If taille < 1 Then taille = 20
        Dim decalage As Integer = (page - 1) * taille
        Return AvecConnexion(Function(cn, tr)
                                 Using cmd As New MySqlCommand("SELECT " & COLONNES & " FROM produit ORDER BY id LIMIT @taille OFFSET @decalage;", cn, tr)
                                     cmd.Parameters.AddWithValue("@taille", taille)
                                     cmd.Parameters.AddWithValue("@decalage", decalage)
                                     Using lecteur = cmd.ExecuteReader()
                                         Return Mappeur.LireListe(Of Produit)(lecteur)
                                     End Using
                                 End Using
                             End Function)
    End Function

    Public Function Compter() As Long Implements IDepot(Of Produit).Compter
        Return AvecConnexion(Function(cn, tr)
                                 Using cmd As New MySqlCommand("SELECT COUNT(*) FROM produit;", cn, tr)
                                     Return Convert.ToInt64(cmd.ExecuteScalar())
                                 End Using
                             End Function)
    End Function

    Public Function ParId(ByVal id As Integer) As Produit Implements IDepot(Of Produit).ParId
        Return AvecConnexion(Function(cn, tr)
                                 Using cmd As New MySqlCommand("SELECT " & COLONNES & " FROM produit WHERE id = @id;", cn, tr)
                                     cmd.Parameters.AddWithValue("@id", id)
                                     Using lecteur = cmd.ExecuteReader()
                                         Dim liste = Mappeur.LireListe(Of Produit)(lecteur)
                                         Return If(liste.Count > 0, liste(0), Nothing)
                                     End Using
                                 End Using
                             End Function)
    End Function

    Public Function Inserer(ByVal entite As Produit) As Integer Implements IDepot(Of Produit).Inserer
        Return AvecConnexion(Function(cn, tr)
                                 Using cmd As New MySqlCommand("INSERT INTO produit (reference, designation, prix_ht, stock) " &
                                                               "VALUES (@ref, @des, @prix, @stock); SELECT LAST_INSERT_ID();", cn, tr)
                                     AjouterParametres(cmd, entite)
                                     Return Convert.ToInt32(cmd.ExecuteScalar())
                                 End Using
                             End Function)
    End Function

    Public Function MettreAJour(ByVal entite As Produit) As Boolean Implements IDepot(Of Produit).MettreAJour
        Return AvecConnexion(Function(cn, tr)
                                 Using cmd As New MySqlCommand("UPDATE produit SET reference = @ref, designation = @des, " &
                                                               "prix_ht = @prix, stock = @stock WHERE id = @id;", cn, tr)
                                     AjouterParametres(cmd, entite)
                                     cmd.Parameters.AddWithValue("@id", entite.Id)
                                     Return cmd.ExecuteNonQuery() > 0
                                 End Using
                             End Function)
    End Function

    Public Function Supprimer(ByVal id As Integer) As Boolean Implements IDepot(Of Produit).Supprimer
        Return AvecConnexion(Function(cn, tr)
                                 Using cmd As New MySqlCommand("DELETE FROM produit WHERE id = @id;", cn, tr)
                                     cmd.Parameters.AddWithValue("@id", id)
                                     Return cmd.ExecuteNonQuery() > 0
                                 End Using
                             End Function)
    End Function

    ''' <summary>Teste la disponibilite de la base.</summary>
    Public Shared Function TesterConnexion(ByRef message As String) As Boolean
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

#Region "Interne"

    Private Sub AjouterParametres(ByVal cmd As MySqlCommand, ByVal p As Produit)
        cmd.Parameters.AddWithValue("@ref", p.Reference)
        cmd.Parameters.AddWithValue("@des", p.Designation)
        cmd.Parameters.AddWithValue("@prix", p.PrixHt)
        cmd.Parameters.AddWithValue("@stock", p.Stock)
    End Sub

    ' Avec UoW : utilise sa connexion/transaction (operation transactionnelle).
    ' Sans UoW : ouvre une connexion dediee, le tout protege par Resilience.
    Private Function AvecConnexion(Of TR)(ByVal operation As Func(Of MySqlConnection, MySqlTransaction, TR)) As TR
        If _uow IsNot Nothing Then
            Return operation(_uow.Connexion, _uow.Transaction)
        End If
        Return Resilience.Executer(Function()
                                       Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
                                           cn.Open()
                                           Return operation(cn, Nothing)
                                       End Using
                                   End Function)
    End Function

#End Region

End Class

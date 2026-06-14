' ============================================================================
'  PuitsBase.vb
'  Puits ecrivant les entrees de journal dans une base MariaDB.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Data
Imports System.Diagnostics
Imports MySql.Data.MySqlClient

''' <summary>
''' Puits base de donnees : insere chaque entree dans la table <c>entree_journal</c>.
''' L'ecriture est defensive (toute erreur est avalee) : un incident de base ne
''' doit jamais faire echouer l'application qui journalise.
''' </summary>
''' <remarks>
''' Pour la clarte pedagogique, chaque entree ouvre sa propre connexion. Un puits
''' a fort debit batcherait les insertions ; ici la simplicite prime.
''' </remarks>
Public NotInheritable Class PuitsBase
    Implements IPuits

    Public Sub Ecrire(ByVal entree As EntreeJournal) Implements IPuits.Ecrire
        Try
            Const requete As String =
                "INSERT INTO entree_journal (survenu_le, niveau, niveau_libelle, categorie, message, exception, machine) " &
                "VALUES (@date, @niveau, @libelle, @categorie, @message, @exception, @machine);"
            Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
                cn.Open()
                Using cmd As New MySqlCommand(requete, cn)
                    cmd.Parameters.AddWithValue("@date", entree.Horodatage)
                    cmd.Parameters.AddWithValue("@niveau", CInt(entree.Niveau))
                    cmd.Parameters.AddWithValue("@libelle", EntreeJournal.LibelleNiveau(entree.Niveau))
                    cmd.Parameters.AddWithValue("@categorie", entree.Categorie)
                    cmd.Parameters.AddWithValue("@message", entree.Message)
                    cmd.Parameters.AddWithValue("@exception",
                        If(String.IsNullOrEmpty(entree.Exception), CType(DBNull.Value, Object), CType(entree.Exception, Object)))
                    cmd.Parameters.AddWithValue("@machine", Environment.MachineName)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("[PuitsBase] ecriture impossible : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Relit les entrees enregistrees (niveau minimal + nombre maximal), pour l'IHM/les tests.</summary>
    Public Shared Function Lire(Optional ByVal niveauMin As Niveau = Niveau.Debogage,
                                Optional ByVal maximum As Integer = 200) As DataTable
        Dim requete As String =
            "SELECT id AS `Id`, survenu_le AS `Survenu le`, niveau_libelle AS `Niveau`, " &
            "       categorie AS `Categorie`, message AS `Message` " &
            "FROM entree_journal WHERE niveau >= @min ORDER BY id DESC LIMIT @max;"
        Dim table As New DataTable()
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@min", CInt(niveauMin))
                cmd.Parameters.AddWithValue("@max", maximum)
                Using adaptateur As New MySqlDataAdapter(cmd)
                    adaptateur.Fill(table)
                End Using
            End Using
        End Using
        Return table
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

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Rien a liberer (connexion par operation).
    End Sub

End Class

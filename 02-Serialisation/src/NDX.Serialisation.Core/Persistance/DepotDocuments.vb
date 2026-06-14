' ============================================================================
'  DepotDocuments.vb
'  Persistance d'objets serialises dans la base "coffre" : enregistrement du
'  payload (BLOB) avec ses metadonnees, puis relecture et deserialisation.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient

''' <summary>
''' Depot de documents serialises. Illustre la technique « sérialiser puis
''' persister » : un objet est converti en octets par le <see cref="Serialiseur"/>,
''' stocke dans une colonne BLOB avec sa taille et son empreinte SHA-256, puis
''' relu et reconstruit a la demande.
''' </summary>
Public Module DepotDocuments

#Region "Ecriture / lecture"

    ''' <summary>
    ''' Serialise un objet, l'enregistre en base et renvoie l'identifiant cree.
    ''' </summary>
    Public Function Enregistrer(Of T)(ByVal libelle As String,
                                      ByVal obj As T,
                                      ByVal format As FormatSerialisation,
                                      Optional ByVal fkCategorie As Integer? = Nothing) As Integer
        Dim octets As Byte() = Serialiseur.VersOctets(obj, format)
        Dim empreinte As String = Sha256Hex(octets)
        Dim typeClr As String = GetType(T).FullName

        Const requete As String =
            "INSERT INTO document (libelle, type_clr, format, contenu, taille_octets, empreinte_sha256, fk_categorie) " &
            "VALUES (@libelle, @type, @format, @contenu, @taille, @empreinte, @categorie); " &
            "SELECT LAST_INSERT_ID();"

        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@libelle", libelle)
                cmd.Parameters.AddWithValue("@type", typeClr)
                cmd.Parameters.AddWithValue("@format", CodeFormat(format))
                cmd.Parameters.AddWithValue("@contenu", octets)
                cmd.Parameters.AddWithValue("@taille", octets.Length)
                cmd.Parameters.AddWithValue("@empreinte", empreinte)
                cmd.Parameters.AddWithValue("@categorie",
                    If(fkCategorie.HasValue, CType(fkCategorie.Value, Object), CType(DBNull.Value, Object)))
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    ''' <summary>Relit un document et le deserialise dans le type attendu.</summary>
    Public Function Recharger(Of T)(ByVal id As Integer) As T
        Const requete As String = "SELECT contenu, format FROM document WHERE id = @id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@id", id)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    If Not lecteur.Read() Then
                        Throw New KeyNotFoundException("Document introuvable : " & id.ToString())
                    End If
                    Dim octets As Byte() = DirectCast(lecteur("contenu"), Byte())
                    Dim format As FormatSerialisation = FormatDepuisCode(Convert.ToString(lecteur("format")))
                    Return Serialiseur.DepuisOctets(Of T)(octets, format)
                End Using
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Verifie l'integrite d'un document : recalcule l'empreinte du payload stocke
    ''' et la compare a l'empreinte enregistree.
    ''' </summary>
    Public Function VerifierIntegrite(ByVal id As Integer) As Boolean
        Const requete As String = "SELECT contenu, empreinte_sha256 FROM document WHERE id = @id;"
        Using cn As New MySqlConnection(ConfigBdd.ChaineConnexion())
            cn.Open()
            Using cmd As New MySqlCommand(requete, cn)
                cmd.Parameters.AddWithValue("@id", id)
                Using lecteur As MySqlDataReader = cmd.ExecuteReader()
                    If Not lecteur.Read() Then Return False
                    Dim octets As Byte() = DirectCast(lecteur("contenu"), Byte())
                    Dim attendue As String = Convert.ToString(lecteur("empreinte_sha256"))
                    Return String.Equals(attendue, Sha256Hex(octets), StringComparison.OrdinalIgnoreCase)
                End Using
            End Using
        End Using
    End Function

#End Region

#Region "Listes / diagnostic"

    ''' <summary>Liste les documents (metadonnees) sous forme de <see cref="DataTable"/>.</summary>
    Public Function Lister() As DataTable
        Const requete As String =
            "SELECT d.id AS `Id`, d.libelle AS `Libelle`, d.format AS `Format`, " &
            "       d.taille_octets AS `Taille`, c.libelle AS `Categorie`, d.cree_le AS `Cree le` " &
            "FROM document d LEFT JOIN categorie c ON c.id = d.fk_categorie " &
            "ORDER BY d.id DESC;"
        Return RemplirTable(requete)
    End Function

    ''' <summary>Liste les categories (pour alimenter une liste deroulante).</summary>
    Public Function ListerCategories() As DataTable
        Return RemplirTable("SELECT id, libelle FROM categorie ORDER BY libelle;")
    End Function

    ''' <summary>Teste la disponibilite de la base (ouverture + SELECT VERSION()).</summary>
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

#End Region

#Region "Interne"

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

    ''' <summary>Empreinte SHA-256 d'un tableau d'octets, en hexadecimal minuscule.</summary>
    Private Function Sha256Hex(ByVal octets As Byte()) As String
        Using algo As SHA256 = SHA256.Create()
            Dim empreinte As Byte() = algo.ComputeHash(octets)
            Dim sb As New StringBuilder(empreinte.Length * 2)
            For Each o As Byte In empreinte
                sb.Append(o.ToString("x2"))
            Next
            Return sb.ToString()
        End Using
    End Function

    ' Correspondance entre l'enum de format et le code stocke (colonne ENUM en base).
    Private Function CodeFormat(ByVal format As FormatSerialisation) As String
        Select Case format
            Case FormatSerialisation.Xml : Return "xml"
            Case FormatSerialisation.ContratXml : Return "contrat_xml"
            Case FormatSerialisation.Binaire : Return "binaire"
            Case FormatSerialisation.Json : Return "json"
            Case Else : Throw New ArgumentOutOfRangeException(NameOf(format))
        End Select
    End Function

    Private Function FormatDepuisCode(ByVal code As String) As FormatSerialisation
        Select Case If(code, "").Trim().ToLowerInvariant()
            Case "xml" : Return FormatSerialisation.Xml
            Case "contrat_xml" : Return FormatSerialisation.ContratXml
            Case "binaire" : Return FormatSerialisation.Binaire
            Case "json" : Return FormatSerialisation.Json
            Case Else : Throw New ArgumentOutOfRangeException(NameOf(code), "Code de format inconnu : " & code)
        End Select
    End Function

#End Region

End Module

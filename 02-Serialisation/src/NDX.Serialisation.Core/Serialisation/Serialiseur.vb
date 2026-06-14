' ============================================================================
'  Serialiseur.vb
'  Serialisation / deserialisation multi-format d'objets fortement types
'  (chaine, tableau d'octets, fichier).
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization

''' <summary>
''' Facade unifiee de serialisation : convertit un objet fortement type en
''' octets / chaine / fichier (et inversement) selon un <see cref="FormatSerialisation"/>.
''' </summary>
''' <remarks>
''' <para>
''' Quatre formats sont proposes : XML lisible, contrat de donnees XML, binaire
''' compact (contrat de donnees + writer binaire) et JSON. Tous reposent sur des
''' serialiseurs integres a .NET Framework.
''' </para>
''' <para>
''' Choix de securite : la serialisation binaire historique
''' (<c>BinaryFormatter</c>) n'est volontairement PAS utilisee. Elle est obsolete
''' et exposee a des attaques par desserialisation. Le format binaire propose ici
''' s'appuie sur <see cref="XmlDictionaryWriter"/>, sur et performant.
''' </para>
''' </remarks>
Public Module Serialiseur

    ' Encodage UTF-8 sans BOM, partage pour les formats textuels.
    Private ReadOnly _utf8 As New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)

#Region "Octets"

    ''' <summary>Serialise un objet en tableau d'octets selon le format demande.</summary>
    Public Function VersOctets(Of T)(ByVal obj As T, ByVal format As FormatSerialisation) As Byte()
        Using flux As New MemoryStream()
            Select Case format

                Case FormatSerialisation.Xml
                    Using ecrivain As XmlWriter = XmlWriter.Create(flux, ParametresXml())
                        Dim serialiseur As New XmlSerializer(GetType(T))
                        serialiseur.Serialize(ecrivain, obj)
                    End Using

                Case FormatSerialisation.ContratXml
                    Using ecrivain As XmlWriter = XmlWriter.Create(flux, ParametresXml())
                        Dim serialiseur As New DataContractSerializer(GetType(T))
                        serialiseur.WriteObject(ecrivain, obj)
                    End Using

                Case FormatSerialisation.Binaire
                    Using ecrivain As XmlDictionaryWriter = XmlDictionaryWriter.CreateBinaryWriter(flux)
                        Dim serialiseur As New DataContractSerializer(GetType(T))
                        serialiseur.WriteObject(ecrivain, obj)
                        ecrivain.Flush()
                    End Using

                Case FormatSerialisation.Json
                    Dim serialiseur As New DataContractJsonSerializer(GetType(T))
                    serialiseur.WriteObject(flux, obj)

                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(format))
            End Select

            Return flux.ToArray()
        End Using
    End Function

    ''' <summary>Reconstruit un objet a partir d'un tableau d'octets et d'un format.</summary>
    Public Function DepuisOctets(Of T)(ByVal octets As Byte(), ByVal format As FormatSerialisation) As T
        If octets Is Nothing Then Throw New ArgumentNullException(NameOf(octets))
        Using flux As New MemoryStream(octets)
            Select Case format

                Case FormatSerialisation.Xml
                    Dim serialiseur As New XmlSerializer(GetType(T))
                    Return DirectCast(serialiseur.Deserialize(flux), T)

                Case FormatSerialisation.ContratXml
                    Dim serialiseur As New DataContractSerializer(GetType(T))
                    Return DirectCast(serialiseur.ReadObject(flux), T)

                Case FormatSerialisation.Binaire
                    Using lecteur As XmlDictionaryReader =
                        XmlDictionaryReader.CreateBinaryReader(flux, XmlDictionaryReaderQuotas.Max)
                        Dim serialiseur As New DataContractSerializer(GetType(T))
                        Return DirectCast(serialiseur.ReadObject(lecteur), T)
                    End Using

                Case FormatSerialisation.Json
                    Dim serialiseur As New DataContractJsonSerializer(GetType(T))
                    Return DirectCast(serialiseur.ReadObject(flux), T)

                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(format))
            End Select
        End Using
    End Function

#End Region

#Region "Chaine"

    ''' <summary>Indique si le format produit un texte lisible encode en UTF-8.</summary>
    Public Function EstFormatTexte(ByVal format As FormatSerialisation) As Boolean
        Return format <> FormatSerialisation.Binaire
    End Function

    ''' <summary>
    ''' Serialise un objet en chaine de caracteres (formats textuels uniquement).
    ''' </summary>
    Public Function VersChaine(Of T)(ByVal obj As T, ByVal format As FormatSerialisation) As String
        If Not EstFormatTexte(format) Then
            Throw New InvalidOperationException(
                "Le format binaire n'est pas representable en chaine ; utilisez VersOctets.")
        End If
        Return _utf8.GetString(VersOctets(obj, format))
    End Function

    ''' <summary>Reconstruit un objet a partir d'une chaine (formats textuels uniquement).</summary>
    Public Function DepuisChaine(Of T)(ByVal donnees As String, ByVal format As FormatSerialisation) As T
        If Not EstFormatTexte(format) Then
            Throw New InvalidOperationException(
                "Le format binaire n'est pas representable en chaine ; utilisez DepuisOctets.")
        End If
        Return DepuisOctets(Of T)(_utf8.GetBytes(donnees), format)
    End Function

#End Region

#Region "Fichier"

    ''' <summary>Serialise un objet directement dans un fichier.</summary>
    Public Sub SauverFichier(Of T)(ByVal obj As T, ByVal chemin As String, ByVal format As FormatSerialisation)
        File.WriteAllBytes(chemin, VersOctets(obj, format))
    End Sub

    ''' <summary>Recharge un objet depuis un fichier.</summary>
    Public Function ChargerFichier(Of T)(ByVal chemin As String, ByVal format As FormatSerialisation) As T
        Return DepuisOctets(Of T)(File.ReadAllBytes(chemin), format)
    End Function

#End Region

#Region "Interne"

    ' Parametres communs aux ecritures XML textuelles : UTF-8 sans BOM, indente.
    Private Function ParametresXml() As XmlWriterSettings
        Return New XmlWriterSettings With {
            .Encoding = _utf8,
            .Indent = True,
            .IndentChars = "  "
        }
    End Function

#End Region

End Module

' ============================================================================
'  StockageIsole.vb
'  Persistance d'objets serialises dans le stockage isole de l'utilisateur
'  (IsolatedStorage).
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.IO
Imports System.IO.IsolatedStorage

''' <summary>
''' Sauvegarde et relecture d'objets serialises dans le <b>stockage isole</b> de
''' l'utilisateur : un espace de fichiers privatif, propre a l'application et a
''' l'utilisateur Windows, sans avoir a gerer de chemin sur le disque.
''' </summary>
Public Module StockageIsole

    ''' <summary>Serialise un objet et l'ecrit dans le stockage isole.</summary>
    Public Sub Sauver(Of T)(ByVal obj As T, ByVal nomFichier As String, ByVal format As FormatSerialisation)
        Dim octets As Byte() = Serialiseur.VersOctets(obj, format)
        Using magasin As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForAssembly()
            Using flux As New IsolatedStorageFileStream(nomFichier, FileMode.Create, magasin)
                flux.Write(octets, 0, octets.Length)
            End Using
        End Using
    End Sub

    ''' <summary>Relit et deserialise un objet depuis le stockage isole.</summary>
    Public Function Charger(Of T)(ByVal nomFichier As String, ByVal format As FormatSerialisation) As T
        Using magasin As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForAssembly()
            Using flux As New IsolatedStorageFileStream(nomFichier, FileMode.Open, magasin)
                Using tampon As New MemoryStream()
                    flux.CopyTo(tampon)
                    Return Serialiseur.DepuisOctets(Of T)(tampon.ToArray(), format)
                End Using
            End Using
        End Using
    End Function

    ''' <summary>Indique si un fichier existe dans le stockage isole.</summary>
    Public Function Existe(ByVal nomFichier As String) As Boolean
        Using magasin As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForAssembly()
            Return magasin.FileExists(nomFichier)
        End Using
    End Function

    ''' <summary>Supprime un fichier du stockage isole s'il existe.</summary>
    Public Sub Supprimer(ByVal nomFichier As String)
        Using magasin As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForAssembly()
            If magasin.FileExists(nomFichier) Then magasin.DeleteFile(nomFichier)
        End Using
    End Sub

End Module

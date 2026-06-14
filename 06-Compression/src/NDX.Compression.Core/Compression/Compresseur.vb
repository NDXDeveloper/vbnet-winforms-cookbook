' ============================================================================
'  Compresseur.vb  -  Compression / decompression de donnees (GZip, Deflate).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.IO
Imports System.IO.Compression
Imports System.Text

''' <summary>
''' Compresse et decompresse des tableaux d'octets (et du texte UTF-8) via les
''' flux de <see cref="System.IO.Compression"/>.
''' </summary>
''' <remarks>
''' Point d'attention : le flux de compression doit etre <b>ferme</b> (Dispose)
''' AVANT de lire le tampon de sortie, sinon les derniers octets ne sont pas vides.
''' On ouvre donc le flux avec <c>leaveOpen:=True</c> pour ne pas fermer le tampon
''' sous-jacent en meme temps.
''' </remarks>
Public Module Compresseur

    Private ReadOnly _utf8 As New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)

    ''' <summary>Compresse un tableau d'octets.</summary>
    Public Function Compresser(ByVal donnees As Byte(), Optional ByVal algo As AlgorithmeCompression = AlgorithmeCompression.GZip) As Byte()
        If donnees Is Nothing Then Throw New ArgumentNullException(NameOf(donnees))
        Using sortie As New MemoryStream()
            Using flux As Stream = FluxCompression(sortie, algo)
                flux.Write(donnees, 0, donnees.Length)
            End Using   ' ferme le flux de compression (vide les tampons) sans fermer "sortie"
            Return sortie.ToArray()
        End Using
    End Function

    ''' <summary>Decompresse un tableau d'octets.</summary>
    Public Function Decompresser(ByVal donnees As Byte(), Optional ByVal algo As AlgorithmeCompression = AlgorithmeCompression.GZip) As Byte()
        If donnees Is Nothing Then Throw New ArgumentNullException(NameOf(donnees))
        Using entree As New MemoryStream(donnees)
            Using flux As Stream = FluxDecompression(entree, algo)
                Using sortie As New MemoryStream()
                    flux.CopyTo(sortie)
                    Return sortie.ToArray()
                End Using
            End Using
        End Using
    End Function

    ''' <summary>Compresse une chaine (encodee en UTF-8).</summary>
    Public Function CompresserTexte(ByVal texte As String, Optional ByVal algo As AlgorithmeCompression = AlgorithmeCompression.GZip) As Byte()
        Return Compresser(_utf8.GetBytes(If(texte, "")), algo)
    End Function

    ''' <summary>Decompresse vers une chaine (UTF-8).</summary>
    Public Function DecompresserTexte(ByVal donnees As Byte(), Optional ByVal algo As AlgorithmeCompression = AlgorithmeCompression.GZip) As String
        Return _utf8.GetString(Decompresser(donnees, algo))
    End Function

    ''' <summary>Ratio taille compressee / taille originale (0..1 ; plus petit = mieux).</summary>
    Public Function Ratio(ByVal tailleOriginale As Long, ByVal tailleCompressee As Long) As Double
        If tailleOriginale <= 0 Then Return 0
        Return tailleCompressee / CDbl(tailleOriginale)
    End Function

    ''' <summary>Gain de place en pourcentage (100 * (1 - ratio)).</summary>
    Public Function PourcentageGain(ByVal tailleOriginale As Long, ByVal tailleCompressee As Long) As Double
        Return (1.0 - Ratio(tailleOriginale, tailleCompressee)) * 100.0
    End Function

    Private Function FluxCompression(ByVal sousJacent As Stream, ByVal algo As AlgorithmeCompression) As Stream
        Select Case algo
            Case AlgorithmeCompression.GZip : Return New GZipStream(sousJacent, CompressionMode.Compress, leaveOpen:=True)
            Case AlgorithmeCompression.Deflate : Return New DeflateStream(sousJacent, CompressionMode.Compress, leaveOpen:=True)
            Case Else : Throw New ArgumentOutOfRangeException(NameOf(algo))
        End Select
    End Function

    Private Function FluxDecompression(ByVal sousJacent As Stream, ByVal algo As AlgorithmeCompression) As Stream
        Select Case algo
            Case AlgorithmeCompression.GZip : Return New GZipStream(sousJacent, CompressionMode.Decompress, leaveOpen:=True)
            Case AlgorithmeCompression.Deflate : Return New DeflateStream(sousJacent, CompressionMode.Decompress, leaveOpen:=True)
            Case Else : Throw New ArgumentOutOfRangeException(NameOf(algo))
        End Select
    End Function

End Module

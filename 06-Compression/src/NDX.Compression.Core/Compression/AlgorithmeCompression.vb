' ============================================================================
'  AlgorithmeCompression.vb  -  Algorithmes de compression pris en charge.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Algorithmes de compression disponibles (tous integres a .NET).</summary>
Public Enum AlgorithmeCompression
    ''' <summary>DEFLATE brut (RFC 1951) : compact, sans en-tete.</summary>
    Deflate = 0
    ''' <summary>GZIP (RFC 1952) : DEFLATE + en-tete + CRC (format de fichier .gz).</summary>
    GZip = 1
End Enum

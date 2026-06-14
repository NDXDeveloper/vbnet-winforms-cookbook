' ============================================================================
'  IExportateur.vb  -  Contrat d'un exportateur de DataTable.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data

''' <summary>Convertit un <see cref="DataTable"/> en un document binaire (CSV, xlsx, PDF...).</summary>
Public Interface IExportateur
    ''' <summary>Format produit.</summary>
    ReadOnly Property Format As FormatExport
    ''' <summary>Extension de fichier conseillee (ex. ".csv").</summary>
    ReadOnly Property Extension As String
    ''' <summary>Type MIME du document produit.</summary>
    ReadOnly Property TypeMime As String
    ''' <summary>Produit le document a partir de la table.</summary>
    Function Exporter(ByVal table As DataTable) As Byte()
End Interface

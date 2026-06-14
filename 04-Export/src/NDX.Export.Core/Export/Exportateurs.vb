' ============================================================================
'  Exportateurs.vb  -  Fabrique et raccourcis d'export.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Data
Imports System.IO

''' <summary>Point d'entree pratique : choisit l'exportateur selon le format et exporte.</summary>
Public Module Exportateurs

    ''' <summary>Cree l'exportateur correspondant au format.</summary>
    Public Function Creer(ByVal format As FormatExport) As IExportateur
        Select Case format
            Case FormatExport.Csv : Return New ExportateurCsv()
            Case FormatExport.Excel : Return New ExportateurExcel()
            Case FormatExport.Pdf : Return New ExportateurPdf()
            Case Else : Throw New ArgumentOutOfRangeException(NameOf(format))
        End Select
    End Function

    ''' <summary>Exporte une table dans le format demande et renvoie les octets.</summary>
    Public Function Exporter(ByVal table As DataTable, ByVal format As FormatExport) As Byte()
        Return Creer(format).Exporter(table)
    End Function

    ''' <summary>Exporte une table directement dans un fichier.</summary>
    Public Sub SauverFichier(ByVal table As DataTable, ByVal chemin As String, ByVal format As FormatExport)
        File.WriteAllBytes(chemin, Exporter(table, format))
    End Sub

End Module

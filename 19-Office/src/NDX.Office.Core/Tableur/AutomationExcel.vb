Option Strict Off
' ============================================================================
'  AutomationExcel.vb  -  Pilotage de Microsoft Excel par COM (liaison tardive).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================
'  « Option Strict Off » est volontaire et limité à CE fichier : on pilote Excel
'  en LIAISON TARDIVE (sans référence d'interop). Cette voie est OPTIONNELLE et
'  n'est utilisable QUE si Microsoft Excel est installé (voir EstDisponible).
'  La lecture/écriture .xlsx « sans Office » (ClasseurXlsx) reste la voie par défaut.
' ============================================================================

Imports System.Collections.Generic
Imports System.Runtime.InteropServices

''' <summary>Exporte des données en pilotant Excel par automation COM (si Excel est présent).</summary>
Public NotInheritable Class AutomationExcel

    Private Sub New()
    End Sub

    ''' <summary>Indique si Microsoft Excel est installé (sans le démarrer).</summary>
    Public Shared Function EstDisponible() As Boolean
        Return Type.GetTypeFromProgID("Excel.Application") IsNot Nothing
    End Function

    ''' <summary>Crée un classeur Excel, y écrit les lignes et l'enregistre en .xlsx.</summary>
    Public Shared Sub Exporter(ByVal chemin As String, ByVal lignes As IEnumerable(Of String()))
        Dim typeExcel As Type = Type.GetTypeFromProgID("Excel.Application")
        If typeExcel Is Nothing Then Throw New InvalidOperationException("Microsoft Excel n'est pas installé sur ce poste.")

        Dim application As Object = Nothing
        Dim classeur As Object = Nothing
        Try
            application = Activator.CreateInstance(typeExcel)
            application.Visible = False
            application.DisplayAlerts = False
            classeur = application.Workbooks.Add()
            Dim feuille As Object = classeur.Worksheets(1)

            Dim numeroLigne As Integer = 1
            For Each ligne As String() In lignes
                For colonne As Integer = 1 To ligne.Length
                    feuille.Cells(numeroLigne, colonne).Value = ligne(colonne - 1)
                Next
                numeroLigne += 1
            Next

            ' 51 = xlOpenXMLWorkbook (.xlsx).
            classeur.SaveAs(chemin, 51)
            classeur.Close(False)
            application.Quit()
        Finally
            If classeur IsNot Nothing Then Marshal.FinalReleaseComObject(classeur)
            If application IsNot Nothing Then Marshal.FinalReleaseComObject(application)
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
    End Sub

End Class

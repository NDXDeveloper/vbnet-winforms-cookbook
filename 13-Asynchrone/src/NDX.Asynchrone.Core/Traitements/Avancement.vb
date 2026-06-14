' ============================================================================
'  Avancement.vb  -  Etat d'avancement transmis a IProgress.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Instantané de progression d'un traitement (rapporté via <see cref="IProgress(Of T)"/>).</summary>
Public NotInheritable Class Avancement

    Public Sub New(ByVal indexCourant As Integer, ByVal total As Integer, ByVal message As String)
        Me.IndexCourant = indexCourant
        Me.Total = total
        Me.Message = message
        Me.Pourcentage = CalculAvancement.Pourcentage(indexCourant, total)
    End Sub

    Public ReadOnly Property IndexCourant As Integer
    Public ReadOnly Property Total As Integer
    Public ReadOnly Property Pourcentage As Integer
    Public ReadOnly Property Message As String

End Class

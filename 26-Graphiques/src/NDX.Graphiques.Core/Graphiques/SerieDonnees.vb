' ============================================================================
'  SerieDonnees.vb  -  Une serie de donnees a tracer (libelles + valeurs).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing

''' <summary>Série de mesures : des couples (libellé, valeur) et une couleur de tracé.</summary>
Public NotInheritable Class SerieDonnees

    Public Property Nom As String = "Série"
    Public Property Couleur As Color = Color.FromArgb(0, 172, 193)
    Public ReadOnly Property Libelles As New List(Of String)()
    Public ReadOnly Property Valeurs As New List(Of Double)()

    Public Sub Ajouter(ByVal libelle As String, ByVal valeur As Double)
        Libelles.Add(libelle)
        Valeurs.Add(valeur)
    End Sub

    Public ReadOnly Property Nombre As Integer
        Get
            Return Valeurs.Count
        End Get
    End Property

End Class

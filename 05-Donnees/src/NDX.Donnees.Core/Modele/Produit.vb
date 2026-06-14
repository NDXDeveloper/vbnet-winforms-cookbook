' ============================================================================
'  Produit.vb  -  Entite du domaine de demonstration.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Un produit du catalogue. Les noms de proprietes servent au mapping objet-relationnel.</summary>
Public Class Produit
    Public Property Id As Integer
    Public Property Reference As String
    Public Property Designation As String
    Public Property PrixHt As Decimal
    Public Property Stock As Integer

    ''' <summary>Constructeur sans parametre requis par le mappeur.</summary>
    Public Sub New()
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0} - {1} ({2:0.00} EUR, stock {3})", Reference, Designation, PrixHt, Stock)
    End Function
End Class

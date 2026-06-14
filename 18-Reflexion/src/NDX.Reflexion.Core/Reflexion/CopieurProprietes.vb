' ============================================================================
'  CopieurProprietes.vb  -  Copie de proprietes par reflexion (mapping par nom).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Reflection

''' <summary>
''' Copie les propriétés de même nom (et de type compatible) d'un objet vers un
''' autre, par réflexion. Pratique pour cloner des données sans écrire de mapping à
''' la main. Logique pure : testable directement.
''' </summary>
Public NotInheritable Class CopieurProprietes

    Private Sub New()
    End Sub

    Private Const PUBLICS_INSTANCE As BindingFlags = BindingFlags.Public Or BindingFlags.Instance

    ''' <summary>Copie les propriétés compatibles de <paramref name="source"/> vers <paramref name="destination"/> ; renvoie le nombre copié.</summary>
    Public Shared Function Copier(ByVal source As Object, ByVal destination As Object) As Integer
        If source Is Nothing Then Throw New ArgumentNullException(NameOf(source))
        If destination Is Nothing Then Throw New ArgumentNullException(NameOf(destination))

        Dim copiees As Integer = 0
        For Each pDest As PropertyInfo In destination.GetType().GetProperties(PUBLICS_INSTANCE)
            If Not pDest.CanWrite Then Continue For
            Dim pSrc As PropertyInfo = source.GetType().GetProperty(pDest.Name, PUBLICS_INSTANCE)
            If pSrc Is Nothing OrElse Not pSrc.CanRead Then Continue For
            If Not pDest.PropertyType.IsAssignableFrom(pSrc.PropertyType) Then Continue For
            pDest.SetValue(destination, pSrc.GetValue(source, Nothing), Nothing)
            copiees += 1
        Next
        Return copiees
    End Function

End Class

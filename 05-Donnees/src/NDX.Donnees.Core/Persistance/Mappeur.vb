' ============================================================================
'  Mappeur.vb  -  Mapping objet-relationnel simple (DataReader -> objets).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Reflection

''' <summary>
''' Mappe les lignes d'un <see cref="IDataReader"/> vers des objets fortement
''' types par <b>reflexion</b> : chaque colonne est affectee a la propriete
''' publique de meme nom (insensible a la casse). Les requetes aliasent leurs
''' colonnes sur les noms de proprietes (ex. <c>prix_ht AS PrixHt</c>).
''' </summary>
Public Module Mappeur

    ''' <summary>Lit toutes les lignes et les transforme en liste d'objets de type T.</summary>
    Public Function LireListe(Of T As {New})(ByVal lecteur As IDataReader) As List(Of T)
        Dim resultat As New List(Of T)()

        ' Index des proprietes accessibles en ecriture, par nom (minuscule).
        Dim proprietes As New Dictionary(Of String, PropertyInfo)(StringComparer.OrdinalIgnoreCase)
        For Each p As PropertyInfo In GetType(T).GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            If p.CanWrite Then proprietes(p.Name) = p
        Next

        ' Correspondance colonne -> propriete (resolue une seule fois).
        Dim correspondance As New List(Of KeyValuePair(Of Integer, PropertyInfo))()
        For i As Integer = 0 To lecteur.FieldCount - 1
            Dim prop As PropertyInfo = Nothing
            If proprietes.TryGetValue(lecteur.GetName(i), prop) Then
                correspondance.Add(New KeyValuePair(Of Integer, PropertyInfo)(i, prop))
            End If
        Next

        While lecteur.Read()
            Dim obj As New T()
            For Each paire As KeyValuePair(Of Integer, PropertyInfo) In correspondance
                Dim valeur As Object = lecteur.GetValue(paire.Key)
                If valeur IsNot Nothing AndAlso Not Convert.IsDBNull(valeur) Then
                    paire.Value.SetValue(obj, ConvertirVers(paire.Value.PropertyType, valeur), Nothing)
                End If
            Next
            resultat.Add(obj)
        End While
        Return resultat
    End Function

    ' Convertit une valeur vers le type cible (gere les types Nullable).
    Private Function ConvertirVers(ByVal typeCible As Type, ByVal valeur As Object) As Object
        Dim sousType As Type = Nullable.GetUnderlyingType(typeCible)
        If sousType IsNot Nothing Then typeCible = sousType
        If typeCible.IsAssignableFrom(valeur.GetType()) Then Return valeur
        Return Convert.ChangeType(valeur, typeCible, CultureInfo.InvariantCulture)
    End Function

End Module

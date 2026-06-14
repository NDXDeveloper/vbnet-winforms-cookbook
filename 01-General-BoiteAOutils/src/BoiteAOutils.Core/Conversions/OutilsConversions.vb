Imports System.Globalization
Imports System.Reflection

''' <summary>
''' Conversions et formatage de valeurs numeriques.
''' </summary>
''' <remarks>
''' Conversion robuste d'une chaine en entier et formatage compact d'un nombre
''' decimal (suppression des decimales inutiles).
''' </remarks>
Public Module OutilsConversions

    ''' <summary>
    ''' Convertit une chaine en entier. Une chaine vide est traitee comme "0".
    ''' Retourne -1 en cas d'echec (valeur sentinelle).
    ''' </summary>
    Public Function ConvertirEnEntier(ByVal valeur As String) As Integer
        If String.IsNullOrEmpty(valeur) Then valeur = "0"
        Try
            Return CInt(Convert.ToInt64(valeur))
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' Formate un nombre decimal en supprimant les decimales inutiles : un
    ''' nombre entier est rendu sans virgule, sinon deux decimales sont conservees.
    ''' </summary>
    Public Function Formater(ByVal nombre As Double) As String
        Try
            ' On force la culture invariante pour un separateur decimal stable.
            Dim s As String = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", nombre)
            If s.EndsWith("00") Then
                Return CInt(Math.Truncate(nombre)).ToString(CultureInfo.InvariantCulture)
            End If
            Return s
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return String.Empty
        End Try
    End Function

End Module

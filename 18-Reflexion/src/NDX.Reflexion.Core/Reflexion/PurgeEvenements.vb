' ============================================================================
'  PurgeEvenements.vb  -  Suppression de tous les abonnes d'un evenement (reflexion).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Reflection

''' <summary>
''' Retire d'un coup TOUS les abonnés d'un événement d'un objet, par réflexion : on
''' atteint le champ délégué qui sous-tend l'événement et on le remet à
''' <c>Nothing</c>. Utile pour éviter les fuites d'abonnements (un abonné qui survit
''' empêche le garbage collector de libérer l'émetteur).
''' </summary>
Public NotInheritable Class PurgeEvenements

    Private Sub New()
    End Sub

    ''' <summary>Retire tous les abonnés de l'événement nommé ; renvoie False s'il est introuvable.</summary>
    Public Shared Function RetirerTous(ByVal cible As Object, ByVal nomEvenement As String) As Boolean
        If cible Is Nothing Then Throw New ArgumentNullException(NameOf(cible))
        Dim t As Type = cible.GetType()
        ' Le champ qui stocke le délégué : « <Nom>Event » (VB) ou « <Nom> » (C#).
        Dim champ As FieldInfo = TrouverChamp(t, nomEvenement & "Event")
        If champ Is Nothing Then champ = TrouverChamp(t, nomEvenement)
        If champ Is Nothing OrElse Not GetType([Delegate]).IsAssignableFrom(champ.FieldType) Then Return False
        champ.SetValue(cible, Nothing)
        Return True
    End Function

    Private Shared Function TrouverChamp(ByVal t As Type, ByVal nom As String) As FieldInfo
        Dim drapeaux As BindingFlags = BindingFlags.Instance Or BindingFlags.Static Or
                                       BindingFlags.Public Or BindingFlags.NonPublic
        Dim courant As Type = t
        While courant IsNot Nothing
            Dim f As FieldInfo = courant.GetField(nom, drapeaux)
            If f IsNot Nothing Then Return f
            courant = courant.BaseType
        End While
        Return Nothing
    End Function

End Class

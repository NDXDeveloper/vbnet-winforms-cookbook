' ============================================================================
'  InspecteurType.vb  -  Decouverte des membres d'un type par reflexion.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection

''' <summary>
''' Inspecte un <see cref="Type"/> et liste ses membres publics d'instance
''' (propriétés, champs, événements) via la <b>réflexion</b> et des
''' <see cref="BindingFlags"/>. Logique pure : testable sur n'importe quel type.
''' </summary>
Public NotInheritable Class InspecteurType

    Private Sub New()
    End Sub

    Private Const PUBLICS_INSTANCE As BindingFlags = BindingFlags.Public Or BindingFlags.Instance

    ''' <summary>Propriétés publiques d'instance.</summary>
    Public Shared Function ListerProprietes(ByVal t As Type) As List(Of DescripteurMembre)
        Return t.GetProperties(PUBLICS_INSTANCE).
            Select(Function(p) New DescripteurMembre(p.Name, "Propriété", NomCourt(p.PropertyType))).
            OrderBy(Function(d) d.Nom).ToList()
    End Function

    ''' <summary>Champs publics d'instance.</summary>
    Public Shared Function ListerChamps(ByVal t As Type) As List(Of DescripteurMembre)
        Return t.GetFields(PUBLICS_INSTANCE).
            Select(Function(f) New DescripteurMembre(f.Name, "Champ", NomCourt(f.FieldType))).
            OrderBy(Function(d) d.Nom).ToList()
    End Function

    ''' <summary>Événements publics d'instance.</summary>
    Public Shared Function ListerEvenements(ByVal t As Type) As List(Of DescripteurMembre)
        Return t.GetEvents(PUBLICS_INSTANCE).
            Select(Function(ev) New DescripteurMembre(ev.Name, "Événement", NomCourt(ev.EventHandlerType))).
            OrderBy(Function(d) d.Nom).ToList()
    End Function

    ''' <summary>Tous les membres ci-dessus réunis.</summary>
    Public Shared Function ListerTout(ByVal t As Type) As List(Of DescripteurMembre)
        Dim tout As New List(Of DescripteurMembre)()
        tout.AddRange(ListerProprietes(t))
        tout.AddRange(ListerChamps(t))
        tout.AddRange(ListerEvenements(t))
        Return tout
    End Function

    Private Shared Function NomCourt(ByVal t As Type) As String
        Return If(t Is Nothing, "?", t.Name)
    End Function

End Class

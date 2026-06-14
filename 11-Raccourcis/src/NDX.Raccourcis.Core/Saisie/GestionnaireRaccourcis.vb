' ============================================================================
'  GestionnaireRaccourcis.vb  -  Inscription + reconnaissance des accords.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' Associe des raccourcis à des actions et reconnaît les frappes — y compris les
''' <b>accords</b> (séquences à plusieurs frappes). Une petite machine à états
''' suit la séquence en cours : on déclenche quand un accord complet est reconnu,
''' on attend tant que la séquence est un préfixe valide, on réinitialise sinon.
''' </summary>
Public NotInheritable Class GestionnaireRaccourcis

    Private NotInheritable Class Inscription
        Public Property Racc As Raccourci
        Public Property Action As String
    End Class

    Private ReadOnly _inscriptions As New List(Of Inscription)()
    Private ReadOnly _enCours As New List(Of Combinaison)()

    ''' <summary>
    ''' Inscrit une action sous un raccourci. Lève en cas de doublon ou de
    ''' <b>conflit de préfixe</b> (ex. « Ctrl+K » et « Ctrl+K, Ctrl+S » : le premier
    ''' se déclencherait avant que le second puisse se compléter).
    ''' </summary>
    Public Sub Inscrire(ByVal action As String, ByVal raccourciTexte As String)
        If String.IsNullOrWhiteSpace(action) Then Throw New ArgumentException("Action obligatoire.")
        Dim r As Raccourci = Raccourci.Analyser(raccourciTexte)
        For Each e As Inscription In _inscriptions
            If e.Racc.Equals(r) Then
                Throw New InvalidOperationException("Raccourci déjà inscrit : " & r.Texte & " (action « " & e.Action & " »).")
            End If
            If e.Racc.EstPrefixeDe(r) OrElse r.EstPrefixeDe(e.Racc) Then
                Throw New InvalidOperationException("Conflit de préfixe entre « " & r.Texte & " » et « " & e.Racc.Texte & " ».")
            End If
        Next
        _inscriptions.Add(New Inscription() With {.Racc = r, .Action = action})
    End Sub

    ''' <summary>Nombre de raccourcis inscrits.</summary>
    Public ReadOnly Property Nombre As Integer
        Get
            Return _inscriptions.Count
        End Get
    End Property

    ''' <summary>Indique si une séquence d'accord est en cours.</summary>
    Public ReadOnly Property AccordEnCours As Boolean
        Get
            Return _enCours.Count > 0
        End Get
    End Property

    ''' <summary>Réinitialise la séquence en cours (ex. sur Échap).</summary>
    Public Sub Reinitialiser()
        _enCours.Clear()
    End Sub

    ''' <summary>Transmet une combinaison (forme texte) — pratique pour les tests.</summary>
    Public Function Appuyer(ByVal combinaisonTexte As String) As ResultatTouche
        Return Appuyer(Combinaison.Analyser(combinaisonTexte))
    End Function

    ''' <summary>Transmet une combinaison et fait progresser la machine à états.</summary>
    Public Function Appuyer(ByVal c As Combinaison) As ResultatTouche
        _enCours.Add(c)
        Dim issue As ResultatTouche = Evaluer()
        If issue IsNot Nothing Then Return issue

        ' Echec : on tente de repartir de zéro avec la seule dernière frappe.
        _enCours.Clear()
        _enCours.Add(c)
        issue = Evaluer()
        If issue IsNot Nothing Then Return issue

        _enCours.Clear()
        Return ResultatTouche.Aucun()
    End Function

    ''' <summary>Évalue la séquence courante ; renvoie Nothing si elle est sans issue.</summary>
    Private Function Evaluer() As ResultatTouche
        ' Correspondance exacte ? (les conflits de préfixe étant interdits, elle est non ambiguë)
        Dim exact As Inscription = _inscriptions.FirstOrDefault(Function(e) e.Racc.Combinaisons.SequenceEqual(_enCours))
        If exact IsNot Nothing Then
            Dim sequence As String = SequenceTexte()
            _enCours.Clear()
            Return ResultatTouche.Declenche(exact.Action, sequence)
        End If
        ' Préfixe d'un accord plus long ? -> on attend la suite.
        If _inscriptions.Any(Function(e) EstPrefixe(_enCours, e.Racc)) Then
            Return ResultatTouche.EnAttente(SequenceTexte())
        End If
        Return Nothing
    End Function

    Private Shared Function EstPrefixe(ByVal sequence As List(Of Combinaison), ByVal racc As Raccourci) As Boolean
        If sequence.Count >= racc.Combinaisons.Count Then Return False
        For i As Integer = 0 To sequence.Count - 1
            If Not sequence(i).Equals(racc.Combinaisons(i)) Then Return False
        Next
        Return True
    End Function

    Private Function SequenceTexte() As String
        Return String.Join(", ", _enCours.Select(Function(c) c.ToString()))
    End Function

    ''' <summary>Liste (action, raccourci) des inscriptions, pour affichage.</summary>
    Public Function Lister() As List(Of KeyValuePair(Of String, String))
        Return _inscriptions.Select(Function(e) New KeyValuePair(Of String, String)(e.Action, e.Racc.Texte)).ToList()
    End Function

End Class

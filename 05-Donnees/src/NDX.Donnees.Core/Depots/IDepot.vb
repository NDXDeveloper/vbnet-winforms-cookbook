' ============================================================================
'  IDepot.vb  -  Contrat generique d'un depot (Repository).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic

''' <summary>Operations CRUD standard d'un depot d'entites de type T.</summary>
Public Interface IDepot(Of T)
    ''' <summary>Page d'entites (pagination 1-based).</summary>
    Function Lister(ByVal page As Integer, ByVal taille As Integer) As List(Of T)
    ''' <summary>Nombre total d'entites.</summary>
    Function Compter() As Long
    ''' <summary>Entite par identifiant, ou Nothing si absente.</summary>
    Function ParId(ByVal id As Integer) As T
    ''' <summary>Insere une entite et renvoie l'identifiant cree.</summary>
    Function Inserer(ByVal entite As T) As Integer
    ''' <summary>Met a jour une entite ; True si une ligne a ete modifiee.</summary>
    Function MettreAJour(ByVal entite As T) As Boolean
    ''' <summary>Supprime par identifiant ; True si une ligne a ete supprimee.</summary>
    Function Supprimer(ByVal id As Integer) As Boolean
End Interface

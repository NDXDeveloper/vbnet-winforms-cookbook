' ============================================================================
'  IPuits.vb
'  Contrat d'un "puits" de journalisation (destination des entrees).
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

''' <summary>
''' Destination d'ecriture des entrees de journal (fichier, console, base,
''' memoire...). Un puits ne doit jamais laisser remonter d'exception : une
''' panne de journalisation ne doit pas faire tomber l'application appelante.
''' </summary>
Public Interface IPuits
    Inherits IDisposable

    ''' <summary>Ecrit une entree dans la destination.</summary>
    Sub Ecrire(ByVal entree As EntreeJournal)

End Interface

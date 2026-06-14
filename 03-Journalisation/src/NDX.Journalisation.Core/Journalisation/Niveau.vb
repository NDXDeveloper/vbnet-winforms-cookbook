' ============================================================================
'  Niveau.vb
'  Niveaux de gravite d'une entree de journal.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

''' <summary>Niveaux de gravite, du plus bavard au plus critique.</summary>
Public Enum Niveau
    ''' <summary>Trace technique fine, utile au diagnostic.</summary>
    Debogage = 0
    ''' <summary>Information de fonctionnement normal.</summary>
    Information = 1
    ''' <summary>Anomalie non bloquante.</summary>
    Avertissement = 2
    ''' <summary>Erreur ayant empeche une operation.</summary>
    Erreur = 3
    ''' <summary>Defaillance grave compromettant l'application.</summary>
    Critique = 4
End Enum

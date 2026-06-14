# 03 — Les puits (destinations)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Un **puits** (`IPuits`) est une destination d'écriture. Le contrat est minimal :

```vbnet
Public Interface IPuits
    Inherits IDisposable
    Sub Ecrire(ByVal entree As EntreeJournal)
End Interface
```

## `PuitsMemoire`

Tampon **circulaire** des dernières entrées (taille `Capacite`) + événement
`EntreeAjoutee`. Idéal pour un **affichage temps réel** : la galerie s'y abonne et
met à jour la vue (en marshalant vers le thread d'interface).

## `PuitsFichier` — avec rotation

Écrit dans un fichier texte. Quand le fichier atteint `tailleMaxOctets`, il est
**roulé** :

```
app.log  ->  app.1.log
app.1.log -> app.2.log
... (au-delà de nbArchives, les plus anciens sont supprimés)
```

Cela borne l'espace disque tout en conservant un historique récent.

## `PuitsConsole`

Écrit la ligne formatée dans la **sortie de débogage** (visible dans Visual Studio).

## `PuitsBase`

Insère l'entrée dans la table `entree_journal` (MariaDB). **Défensif** : toute
erreur d'écriture est avalée (journaliser ne doit jamais faire tomber l'appli).
`PuitsBase.Lire(niveauMin, max)` relit les entrées pour l'IHM/les tests.

> Pour un très gros volume, on **batcherait** les insertions (file d'attente +
> écriture groupée). Ici, une connexion par entrée privilégie la clarté.

## Écrire son propre puits

Implémentez `IPuits` (ex. e-mail, service distant, fichier JSON), puis
`journal.AjouterPuits(...)`. Le journal n'a pas à être modifié.

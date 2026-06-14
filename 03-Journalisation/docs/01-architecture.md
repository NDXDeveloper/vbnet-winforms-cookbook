# 01 — Architecture

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

```
        NDX.Journalisation.UI            NDX.Journalisation.Tests
        (WinExe, WinForms)               (MSTest)
                  │                            │
                  └────────────┬───────────────┘
                               ▼
                     NDX.Journalisation.Core
                     (journal + puits)  ──►  MariaDB 12.3 (puits base)
```

## Les trois projets

| Projet | Type | Rôle |
|---|---|---|
| `NDX.Journalisation.Core` | bibliothèque | journal, niveaux, puits (mémoire/console/fichier/base) |
| `NDX.Journalisation.UI` | WinForms | galerie : émettre et observer, relire la base |
| `NDX.Journalisation.Tests` | MSTest | logique (mémoire, filtre, rotation) + intégration base |

## Principe de conception

Le `Journal` ne dépend que de l'**abstraction** `IPuits`. Ajouter une destination
(fichier, base, service distant…) n'impose **aucune** modification du journal :
c'est le **principe ouvert/fermé**. Le journal est **thread-safe** (`SyncLock`) et
**tolérant aux pannes** : un puits qui échoue n'interrompt ni les autres puits ni
l'application.

## Format de projet

SDK-style, cible **`net481`**, `Option Strict On`. S'ouvre dans Visual Studio 2022 ;
compile via `dotnet build` ou MSBuild.

## Organisation de `Core`

```
Core/
├── Journalisation/  Niveau, EntreeJournal, IPuits, Journal
├── Puits/           PuitsMemoire, PuitsConsole, PuitsFichier, PuitsBase
└── Persistance/     ConfigBdd
```

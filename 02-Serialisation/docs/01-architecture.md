# 01 — Architecture

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Vue d'ensemble

```
        NDX.Serialisation.UI            NDX.Serialisation.Tests
        (WinExe, WinForms)              (MSTest)
                  │                            │
                  └────────────┬───────────────┘
                               │ référence
                               ▼
                     NDX.Serialisation.Core
                     (sérialisation + persistance)
                     dépend de : MySql.Data
                               │
                               ▼
                       MariaDB 12.3 (Docker)
```

## Les trois projets

| Projet | Type | Rôle |
|---|---|---|
| `NDX.Serialisation.Core` | bibliothèque | sérialisation multi-format, stockage fichier/isolé, persistance en base |
| `NDX.Serialisation.UI` | WinForms (`WinExe`) | galerie interactive : une page par thème |
| `NDX.Serialisation.Tests` | MSTest | allers-retours + intégration base |

## Format de projet : SDK-style, cible `net481`

Les projets adoptent le **format SDK-style** (inclusion automatique des fichiers,
restauration NuGet via `PackageReference`, compilation sous Visual Studio comme
via `dotnet`) tout en ciblant **.NET Framework 4.8.1**
(`<TargetFramework>net481</TargetFramework>`). Ils s'ouvrent normalement dans
Visual Studio 2022.

`Option Strict On`, `Option Explicit On`, `Option Infer On` sur les trois projets.

## Espaces de noms

- `Core` : `RootNamespace = NDX.Serialisation` → `Serialiseur`, `FormatSerialisation`,
  `Catalogue`, `Produit`, `DepotDocuments`, `ConfigBdd`, `StockageIsole`.
- `UI` : `NDX.Serialisation.UI`.
- `Tests` : `NDX.Serialisation.Tests`.

## Organisation de `Core`

```
Core/
├── Serialisation/  FormatSerialisation, Serialiseur, StockageIsole
├── Modele/         ModeleDemo (Produit, Catalogue)
└── Persistance/    ConfigBdd, DepotDocuments
```

## Compilation & tests

```bash
cd src
dotnet build NDX.Serialisation.sln
dotnet test  NDX.Serialisation.sln
```

Les tests d'intégration se mettent en *Inconclusive* si le conteneur MariaDB
n'est pas démarré.

## Licence

Projet sous licence **MIT**. Chaque fichier source porte un en-tête de licence
mentionnant l'auteur et la licence.

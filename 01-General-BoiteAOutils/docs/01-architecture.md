# 01 — Architecture

## Vue d'ensemble

La solution est **hybride** : une bibliothèque de techniques réutilisable, une
galerie WinForms qui la consomme, et un projet de tests.

```
        BoiteAOutils.UI              BoiteAOutils.Tests
        (WinExe, WinForms)           (MSTest)
                 │                          │
                 └────────────┬─────────────┘
                              │ référence
                              ▼
                     BoiteAOutils.Core
                     bibliothèque réutilisable (les techniques)
                     dépend de : MySql.Data, System.Windows.Forms, System.Drawing
                              │
                              ▼
                     MariaDB 12.3 (Docker)
```

## Pourquoi des projets SDK-style ciblant `net481` ?

Les projets .NET Framework « classiques » imposent une longue liste de fichiers,
un `packages.config` et un dossier `My Project` complet. Ce projet adopte le
**format SDK-style** :

- **inclusion automatique** des fichiers `.vb` (pas de liste à maintenir) ;
- **restauration NuGet transparente** via `PackageReference` (pas besoin de
  `nuget.exe` ; les dépendances transitives de `MySql.Data` sont résolues
  automatiquement) ;
- compilation **sous Visual Studio 2022 comme via `dotnet build`** ;
- redirections de liaison générées automatiquement
  (`AutoGenerateBindingRedirects`).

Le **ciblage reste `.NET Framework 4.8.1`** (`<TargetFramework>net481</TargetFramework>`),
comme demandé. Les projets s'ouvrent normalement dans Visual Studio 2022.

## Espaces de noms

- `Core` : `RootNamespace = BoiteAOutils`. Les modules y sont donc
  directement accessibles : `AccesDonnees`, `OutilsChaines`, `ConnexionMySql`…
- `UI` : `BoiteAOutils.UI` (espace **enfant**) — les membres de `Core`
  sont accessibles sans import explicite grâce à la résolution ascendante des
  espaces de noms VB.
- `Tests` : `BoiteAOutils.Tests`.

## Organisation de `Core`

Un dossier (et un module statique) par thème :

```
Core/
├── Infrastructure/  ConfigBdd, Journal, ConnexionMySql
├── Exceptions/      GestionExceptions
├── Bdd/             AccesDonnees
├── Chaines/         OutilsChaines
├── Conversions/     OutilsConversions
├── Geometrie/       OutilsGeometrie
├── Fichiers/        OutilsFichiers
├── Images/          OutilsImages
├── Controles/       OutilsControles
├── Systeme/         OutilsSysteme
├── Reseau/          OutilsReseau, MoniteurReseau
├── Processus/       OutilsProcessus
└── Interop/         ApiWindows
```

> VB n'autorisant pas un `Module` *partial* réparti sur plusieurs fichiers, on a
> choisi **un module par thème** : plus lisible, et chaque fichier reste focalisé.

## Options de compilation

`Option Strict On`, `Option Explicit On`, `Option Infer On` sur les trois projets :
typage strict, déclaration obligatoire — de bonnes pratiques qui évitent les
conversions implicites silencieuses.

## Compilation & tests

```bash
cd src
dotnet build BoiteAOutils.sln
dotnet test  BoiteAOutils.sln
```

Les tests d'intégration BDD se mettent en *Inconclusive* si le conteneur n'est
pas démarré (voir [`08`](08-systeme-reseau-processus.md) et le projet `Tests`).

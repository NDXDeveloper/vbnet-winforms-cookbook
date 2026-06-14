# Interop Windows Shell — raccourcis `.lnk` (COM) et liens `.url`

> Projet pédagogique : créer et lire des raccourcis Windows en VB.NET (.NET
> Framework 4.8.1) — fichiers `.lnk` via l'objet COM `WScript.Shell` (liaison
> tardive) et fichiers `.url` (format INI) — avec galerie WinForms et catalogue de
> liens en base MariaDB, exportable en raccourcis réels.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Raccourci .lnk** | objet COM `WScript.Shell` en liaison tardive (create/read) | `Raccourcis/RaccourciWindows.vb` |
| **Raccourci .url** | génération / analyse du format INI (pur, testable) | `Raccourcis/RaccourciInternet.vb` |
| **Catalogue** | liens en base, ajout / suppression / liste | `Persistance/DepotLien.vb` |
| **Export** | écriture de raccourcis réels (.lnk / .url) sur le Bureau | galerie `Pages/PageCatalogue.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**, **Windows**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "catalogue_liens" sur 127.0.0.1:3317
```

1. Ouvrir `src/NDX.Shell.sln` ; démarrer **`NDX.Shell.UI`** ; **F5**.
2. Page *Créer / lire un raccourci* : créer un `.lnk` (ex. vers `notepad.exe`) ou un
   `.url`, puis relire la cible d'un raccourci existant.
3. Page *Catalogue* : ajouter des liens, puis **Exporter sur le Bureau** (crée le
   raccourci réel).

```bash
cd src && dotnet test NDX.Shell.sln    # 10 tests (7 .url + 1 .lnk + 2 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Shell.Core/   → raccourcis .lnk (COM) et .url + catalogue en base
├── NDX.Shell.UI/     → galerie WinForms
└── NDX.Shell.Tests/  → tests MSTest
```

Le format `.url` (texte pur) est **entièrement testé** hors disque. Les `.lnk`
passent par COM : leur test s'exécute sur un **thread STA** (exigence de
`WScript.Shell`).

---

## 5. Deux idées clés

- **Liaison tardive plutôt que P/Invoke.** Piloter `WScript.Shell` via
  `CreateObject` (dans un fichier `Option Strict Off` dédié) évite les fragiles
  déclarations d'interfaces COM `IShellLink`. Détails en
  [`docs/01-raccourcis-windows.md`](docs/01-raccourcis-windows.md).
- **Un `.url` est un simple fichier INI.** `[InternetShortcut]` + `URL=…` :
  [`docs/02-raccourcis-internet.md`](docs/02-raccourcis-internet.md).

---

## 6. Base de démonstration (`catalogue_liens`)

Table `lien` (catalogue). 5 liens amorcés, exportables. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **10/10** (dont 2 d'intégration contre **MariaDB 12.3** ; le test
  `.lnk` s'exécute sur thread STA).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

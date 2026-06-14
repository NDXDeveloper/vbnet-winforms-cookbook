# Journalisation applicative — boîte à outils & démonstration

> Projet pédagogique : un **journal applicatif** (VB.NET, .NET Framework 4.8.1) —
> niveaux, puits multiples, rotation de fichiers, sink base de données —
> accompagné d'une galerie WinForms et d'une base MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Niveaux & entrée** | 5 niveaux, entrée immuable, formatage | `Journalisation/Niveau.vb`, `EntreeJournal.vb` |
| **Journal thread-safe** | diffusion vers N puits, filtre par niveau, `SyncLock` | `Journalisation/Journal.vb` |
| **Puits (sinks)** | contrat `IPuits`, robustesse (un puits ne fait pas planter l'appli) | `Journalisation/IPuits.vb` |
| **Mémoire** | tampon circulaire + événement (affichage live) | `Puits/PuitsMemoire.vb` |
| **Fichier + rotation** | rotation par taille avec archives `.1`, `.2`… | `Puits/PuitsFichier.vb` |
| **Console** | sortie de débogage | `Puits/PuitsConsole.vb` |
| **Base de données** | insertion en table, relecture filtrée | `Puits/PuitsBase.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

- **Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.
- *(facultatif)* le **SDK .NET** pour `dotnet build` / `dotnet test`.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "journal" sur 127.0.0.1:3309
```

1. Ouvrir `src/NDX.Journalisation.sln` ; projet de démarrage **`NDX.Journalisation.UI`** ; **F5**.
2. Page *Journalisation en direct* : émettez des entrées, observez la diffusion ; cochez « Vers base ».
3. Page *Journal en base* : relisez les entrées enregistrées.

```bash
cd src && dotnet test NDX.Journalisation.sln   # 7 tests (5 logique + 2 intégration)
```

---

## 4. Architecture

Solution hybride, trois projets SDK-style **`net481`** :

```
src/
├── NDX.Journalisation.Core/   → bibliothèque (journal + puits)
├── NDX.Journalisation.UI/     → galerie WinForms
└── NDX.Journalisation.Tests/  → tests MSTest
```

Le **journal** ne connaît que l'abstraction `IPuits` : on ajoute autant de
destinations que voulu sans le modifier (principe ouvert/fermé).

---

## 5. Base de démonstration (`coffre` → `journal`)

Table `entree_journal` (horodatage, niveau numérique + libellé, catégorie,
message, exception, machine). Détails : [`docker/README.md`](docker/README.md).

---

## 6. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **7/7** (dont 2 d'intégration contre **MariaDB 12.3** réelle).
- Galerie WinForms : démarrage **sans plantage**.

---

## 7. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;. En-tête de
licence dans chaque fichier source.

# Traitements asynchrones — `Async/Await`, progression, annulation

> Projet pédagogique : exécuter des traitements **sans figer l'interface** en
> VB.NET (.NET Framework 4.8.1) — `Async/Await`, `Task.Run`, progression via
> `IProgress(Of T)`, annulation coopérative par `CancellationToken` — avec galerie
> WinForms et une file de tâches persistée en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Exécution asynchrone** | `Async/Await`, `Task.Run` (travail hors du fil UI) | `Traitements/MoteurTaches.vb` |
| **Progression** | `IProgress(Of Avancement)` (rapport vers le fil UI) | `Traitements/MoteurTaches.vb` |
| **Annulation** | `CancellationToken` coopératif (`ThrowIfCancellationRequested`) | `Traitements/MoteurTaches.vb` |
| **Calcul isolé** | pourcentage borné, testable sans tâche réelle | `Traitements/CalculAvancement.vb` |
| **File de tâches** | empiler / drainer en arrière-plan | `Persistance/DepotTache.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "traitements" sur 127.0.0.1:3318
```

1. Ouvrir `src/NDX.Asynchrone.sln` ; démarrer **`NDX.Asynchrone.UI`** ; **F5**.
2. Page *Progression / annulation* : lancer la tâche longue, déplacer la fenêtre
   (elle reste réactive), puis **Annuler**.
3. Page *File de tâches* : empiler des tâches, puis **drainer la file** en async.

```bash
cd src && dotnet test NDX.Asynchrone.sln    # 12 tests (11 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Asynchrone.Core/   → moteur async + calcul d'avancement + file de tâches
├── NDX.Asynchrone.UI/     → galerie WinForms
└── NDX.Asynchrone.Tests/  → tests MSTest (dont des tests asynchrones)
```

Le moteur est **générique** et **sans interface** : on lui passe une collection,
une action, éventuellement un rapporteur de progression et un jeton d'annulation.
La logique testable (`CalculAvancement`) est isolée ; le moteur est couvert par des
**tests asynchrones** (`Async Function … As Task`).

---

## 5. Les trois idées clés

- **Ne jamais bloquer le fil UI** : le travail part dans `Task.Run`, l'`Await`
  rend la main à l'interface.
- **Progresser proprement** : `Progress(Of T)` capture le contexte de l'UI, donc
  `Report` met à jour les contrôles **sur le bon fil**.
- **Annuler coopérativement** : on vérifie le `CancellationToken` à chaque étape.

Détails en [`docs/01-async-await-progression.md`](docs/01-async-await-progression.md)
et [`docs/02-annulation-cooperative.md`](docs/02-annulation-cooperative.md).

---

## 6. Base de démonstration (`traitements`)

Table `tache` (file d'attente). 5 tâches amorcées. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **12/12** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage** ; interface réactive pendant le traitement.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

# Contrôles WinForms personnalisés — owner-draw, double tampon, zoom/pan

> Projet pédagogique : créer ses propres contrôles WinForms en VB.NET (.NET
> Framework 4.8.1) — dessin propriétaire (`OnPaint`), double tampon contre le
> scintillement, transformation géométrique pour le zoom/déplacement — avec
> galerie WinForms et persistance des réglages en base MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Owner-draw** | `OnPaint`, `GraphicsPath`, propriété + événement personnalisés | `Controles/BoutonBascule.vb` |
| **Double tampon** | `ControlStyles.OptimizedDoubleBuffer` (anti-scintillement) | `Controles/BoutonBascule.vb`, `VisionneuseImage.vb` |
| **Zoom / déplacement** | molette + glisser, `TranslateTransform` / `ScaleTransform` | `Controles/VisionneuseImage.vb` |
| **Logique isolée** | arithmétique du zoom, testable sans interface | `Zoom/CalculZoom.vb` |
| **Persistance** | réglages clef/valeur en base (upsert) | `Persistance/DepotPreferences.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "preferences" sur 127.0.0.1:3314
```

1. Ouvrir `src/NDX.Controles.sln` ; démarrer **`NDX.Controles.UI`** ; **F5**.
2. Page *Interrupteur* : cliquer pour basculer, enregistrer / recharger l'état.
3. Page *Visionneuse* : molette pour zoomer, glisser pour déplacer, recentrer.
4. Page *Préférences* : lister et écrire des réglages (upsert).

```bash
cd src && dotnet test NDX.Controles.sln    # 13 tests (10 logique + 3 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Controles.Core/   → contrôles réutilisables + calcul de zoom + persistance
├── NDX.Controles.UI/     → galerie WinForms
└── NDX.Controles.Tests/  → tests MSTest
```

Les contrôles sont des **classes héritant de `Control`** : réutilisables dans
n'importe quel projet WinForms. La logique sensible (zoom) est **extraite** dans
une classe pure (`CalculZoom`) pour être testée sans fenêtre.

---

## 5. Les deux idées clés

- **Double tampon** : activer `OptimizedDoubleBuffer` fait peindre le contrôle dans
  une image mémoire puis la copier d'un coup → plus de scintillement.
- **Séparer le calcul du rendu** : le zoom est de l'arithmétique. L'isoler le rend
  testable et réutilisable. Détails en [`docs/`](docs/).

---

## 6. Base de démonstration (`preferences`)

Table `preference` (clef/valeur). 3 réglages amorcés. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **13/13** (dont 3 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

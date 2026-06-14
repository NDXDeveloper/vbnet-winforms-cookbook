# Héritage de formulaires & thèmes — factoriser l'apparence

> Projet pédagogique : factoriser l'apparence des fenêtres en VB.NET (.NET
> Framework 4.8.1) — une **fiche de base** dont héritent les formulaires concrets
> (héritage visuel) et un **moteur de thèmes** (clair / sombre) appliqué par
> récursion — avec galerie WinForms et thèmes persistés en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Héritage visuel** | `FormulaireBase` (Inherits Form) : en-tête + contenu communs | `Formulaires/FormulaireBase.vb` |
| **Thèmes** | modèle de couleurs + application récursive aux contrôles | `Theme/Theme.vb`, `Theme/GestionnaireThemes.vb` |
| **Couleurs** | conversion `Color` ↔ « #RRGGBB » (pur, testable) | `Theme/CouleurHex.vb` |
| **Persistance** | thèmes en base (couleurs en hexadécimal) | `Persistance/DepotTheme.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "themes" sur 127.0.0.1:3325
```

1. Ouvrir `src/NDX.FormBase.sln` ; démarrer **`NDX.FormBase.UI`** ; **F5**.
2. Page *Thèmes & héritage* : appliquer clair/sombre à l'aperçu, puis **ouvrir une
   fiche héritée** (elle reçoit en-tête et thème de la classe de base).
3. Page *Catalogue* : enregistrer / lister les thèmes en base.

```bash
cd src && dotnet test NDX.FormBase.sln    # 10 tests (9 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.FormBase.Core/   → fiche de base + thèmes + conversion couleur + persistance
├── NDX.FormBase.UI/     → galerie WinForms + FrmExemple (hérite de FormulaireBase)
└── NDX.FormBase.Tests/  → tests MSTest
```

`FrmExemple` (dans l'UI) **hérite** de `FormulaireBase` (dans le Core) : elle ne
définit que ses propres contrôles et reçoit gratuitement l'en-tête, la zone de
contenu et l'application du thème.

---

## 5. Les deux idées clés

- **Héritage visuel** : on définit l'ossature et le comportement communs **une
  seule fois** dans une fiche de base ; chaque fiche concrète en hérite.
- **Thème par récursion** : appliquer un jeu de couleurs descend l'arbre des
  contrôles (`Controls`) et colore chacun — c'est le principe d'un « mode sombre ».

Détails en [`docs/01-heritage-visuel.md`](docs/01-heritage-visuel.md) et
[`docs/02-themes.md`](docs/02-themes.md).

---

## 6. Base de démonstration (`themes`)

Table `theme` (couleurs en `#RRGGBB`). 2 thèmes amorcés. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **10/10** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage** ; fiche héritée ouvrable.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

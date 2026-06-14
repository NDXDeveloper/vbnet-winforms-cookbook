# Contrôles WinForms avancés — bouton à états, onglets peints, grille

> Projet pédagogique : des contrôles WinForms soignés en VB.NET (.NET Framework
> 4.8.1) — un **bouton dessiné à la main** à états (et `IButtonControl`), un
> **`TabControl` en owner-draw**, une **grille pré-stylée** — avec galerie WinForms
> et données en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Bouton à états** | owner-draw + suivi survol/enfoncé/désactivé + `IButtonControl` | `Controles/BoutonEtat.vb` |
| **Machine à états** | calcul de l'état visuel, isolé et testable | `Controles/CalculEtat.vb` |
| **Onglets peints** | `TabControl` `OwnerDrawFixed` (`OnDrawItem`) | `Controles/OngletsPeints.vb` |
| **Grille pré-stylée** | `DataGridView` dérivé (en-tête, lignes alternées) | `Controles/GrillePersonnalisee.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "inventaire" sur 127.0.0.1:3329
```

1. Ouvrir `src/NDX.ControlesII.sln` ; démarrer **`NDX.ControlesII.UI`** ; **F5**.
2. Page *Boutons à états* : survoler/cliquer ; le 3ᵉ bouton est désactivé.
3. Page *Onglets peints* : onglets dessinés sur mesure.
4. Page *Grille* : articles en base dans la grille pré-stylée.

```bash
cd src && dotnet test NDX.ControlesII.sln    # 7 tests (6 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.ControlesII.Core/   → contrôles réutilisables + machine à états + données
├── NDX.ControlesII.UI/     → galerie WinForms
└── NDX.ControlesII.Tests/  → tests MSTest
```

L'apparence des contrôles est dans le **dessin** (`OnPaint`/`OnDrawItem`) ; la
**décision** d'apparence du bouton (quel état ?) est extraite dans `CalculEtat`,
**pure** et testée. C'est le complément « avancé » des contrôles de base.

---

## 5. Les idées clés

- **Bouton à états** : on suit survol/enfoncé/activé et on peint selon l'état
  (calculé par `CalculEtat`) ; `IButtonControl` permet d'en faire le bouton par
  défaut d'une boîte de dialogue. Détails en
  [`docs/01-bouton-a-etats.md`](docs/01-bouton-a-etats.md).
- **Owner-draw** : on peut reprendre la main sur le rendu d'un contrôle standard
  (`TabControl`) en activant le mode propriétaire. Détails en
  [`docs/02-owner-draw.md`](docs/02-owner-draw.md).

---

## 6. Base de démonstration (`inventaire`)

Table `article`. 5 articles amorcés. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **7/7** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

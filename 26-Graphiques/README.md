# Graphiques de données — contrôle owner-draw (barres / courbe / points)

> Projet pédagogique : un contrôle de **visualisation de données** en VB.NET (.NET
> Framework 4.8.1) — tracé d'une série en **barres**, **courbe** ou **points**, par
> dessin propriétaire (owner-draw) ; le cœur est la **mise à l'échelle valeur →
> pixel** (axes, bornes) — avec galerie WinForms et série en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Mise à l'échelle** | valeur → pixel, bornes min/max, échelle auto (pur) | `Graphiques/EchelleGraphique.vb` |
| **Série de données** | libellés + valeurs + couleur | `Graphiques/SerieDonnees.vb` |
| **Contrôle de tracé** | owner-draw barres / courbe / points (double tampon) | `Graphiques/ControleGraphique.vb` |
| **Persistance** | série lue depuis la base | `Persistance/DepotMesure.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "mesures" sur 127.0.0.1:3331
```

1. Ouvrir `src/NDX.Graphiques.sln` ; démarrer **`NDX.Graphiques.UI`** ; **F5**.
2. Page *Graphique* : changer le mode (barres / courbe / points), charger la série de
   démo ou celle de la base.
3. Page *Mesures* : ajouter des mesures, puis recharger le graphique.

```bash
cd src && dotnet test NDX.Graphiques.sln    # 9 tests (8 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Graphiques.Core/   → échelle (pure) + série + contrôle de tracé + persistance
├── NDX.Graphiques.UI/     → galerie WinForms
└── NDX.Graphiques.Tests/  → tests MSTest
```

La différence avec l'éditeur de formes (projet « Dessin ») : ici on ne dessine pas
des formes libres, on **projette des données**. Le calcul sensible — la **mise à
l'échelle** — est isolé dans `EchelleGraphique`, **pur** et testé ; le contrôle ne
fait que dessiner.

---

## 5. L'idée clé : valeur → pixel

Tracer un graphique, c'est convertir chaque **valeur** en **position pixel** dans la
zone de tracé, en fonction des bornes (min/max). En isolant ce calcul (`VersY`,
`Auto`), on le teste exhaustivement (bas/haut, bornes, échelle plate, valeurs
négatives) **sans rien dessiner**. Détails en
[`docs/01-echelle.md`](docs/01-echelle.md) et
[`docs/02-controle-de-trace.md`](docs/02-controle-de-trace.md).

---

## 6. Base de démonstration (`mesures`)

Table `mesure`. 7 mesures amorcées. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **9/9** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

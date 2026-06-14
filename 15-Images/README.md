# Traitement d'images — filtres `ColorMatrix` et vignettes (GDI+)

> Projet pédagogique : traiter des images en VB.NET (.NET Framework 4.8.1) avec
> GDI+ — filtres par **matrice de couleur** (niveaux de gris, négatif, luminosité)
> et génération de **vignettes** à proportions conservées — avec galerie WinForms
> et médiathèque (vignettes PNG) en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Filtres couleur** | `ColorMatrix` + `ImageAttributes` (gris, négatif, luminosité) | `Images/Filtres.vb` |
| **Calcul de vignette** | dimensions à ratio conservé, sans agrandissement (pur) | `Images/CalculVignette.vb` |
| **Vignette** | redimensionnement bicubique + export PNG | `Images/Vignette.vb` |
| **Médiathèque** | métadonnées + vignette PNG en base | `Persistance/DepotImage.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "mediatheque" sur 127.0.0.1:3320
```

1. Ouvrir `src/NDX.Images.sln` ; démarrer **`NDX.Images.UI`** ; **F5**.
2. Page *Filtres* : image de démo (ou ouvrir un fichier), appliquer gris / négatif /
   éclaircir / assombrir — comparaison avant/après.
3. Page *Médiathèque* : ajouter une image (vignette stockée), lister, réafficher.

```bash
cd src && dotnet test NDX.Images.sln    # 8 tests (7 logique/GDI+ + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Images.Core/   → filtres + calcul/génération de vignettes + médiathèque
├── NDX.Images.UI/     → galerie WinForms
└── NDX.Images.Tests/  → tests MSTest
```

La règle de dimensionnement (pur calcul) est **isolée et testée** ; les filtres sont
vérifiés sur de **vrais pixels** (un pixel passé en niveaux de gris a R = V = B). On
stocke la **vignette** (réduite) plutôt que l'originale : médiathèque légère.

---

## 5. L'idée clé : la matrice de couleur

Un filtre colorimétrique est une **transformation linéaire** appliquée à chaque
pixel (R, V, B, A). Une matrice 5×5 (`ColorMatrix`) encode gris, négatif,
luminosité… GDI+ l'applique en une passe via `ImageAttributes`. Détails en
[`docs/01-colormatrix.md`](docs/01-colormatrix.md) et
[`docs/02-vignettes.md`](docs/02-vignettes.md).

---

## 6. Base de démonstration (`mediatheque`)

Table `image` (vignettes PNG). Démarre **vide**. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **8/8** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

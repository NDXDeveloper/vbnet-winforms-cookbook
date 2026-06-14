# Données hiérarchiques — liste d'adjacence, `TreeView` et requête récursive

> Projet pédagogique : représenter et afficher des **arbres** en VB.NET (.NET
> Framework 4.8.1) — stockage « à plat » par `parent_id` (liste d'adjacence),
> reconstruction de l'arbre en mémoire, affichage `TreeView`, et requête
> **récursive** côté base (`WITH RECURSIVE`) — avec galerie WinForms et MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier |
|---|---|---|
| **Reconstruction d'arbre** | liste plate → racines + enfants, en deux passes O(n) (pur) | `Arbre/ConstructeurArbre.vb` |
| **Modèle hiérarchique** | liste d'adjacence (`parent_id`), nœud « gonflé » | `Modele/Noeud.vb`, `Arbre/NoeudArbre.vb` |
| **Requête récursive** | `WITH RECURSIVE` pour tous les descendants | `Persistance/DepotNoeud.vb` |
| **TreeView** | projection de l'arbre dans un contrôle arborescent | galerie `Pages/PageArbre.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "arborescence" sur 127.0.0.1:3326
```

1. Ouvrir `src/NDX.Arborescence.sln` ; démarrer **`NDX.Arborescence.UI`** ; **F5**.
2. Page *Arbre* : voir l'arbre, ajouter un enfant au nœud sélectionné, supprimer.
3. Page *Descendants* : choisir un nœud, lister tous ses descendants (requête récursive).

```bash
cd src && dotnet test NDX.Arborescence.sln    # 5 tests (4 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Arborescence.Core/   → modèle + reconstruction d'arbre + dépôt (CTE récursive)
├── NDX.Arborescence.UI/     → galerie WinForms (TreeView)
└── NDX.Arborescence.Tests/  → tests MSTest
```

La reconstruction (`ConstructeurArbre`) est **pure** : on la teste sans base. Le
parcours profond des descendants est délégué à la **base** (requête récursive),
plus efficace qu'un aller-retour par niveau.

---

## 5. Les deux approches d'un arbre

- **Liste d'adjacence** (`parent_id`) : simple à écrire et à modifier ; pour
  reconstruire l'arbre complet, on charge tout puis on relie en mémoire
  (`ConstructeurArbre`, deux passes O(n)).
- **Requête récursive** (`WITH RECURSIVE`) : laisse la base parcourir l'arbre (ex.
  « tous les descendants de X ») sans multiplier les requêtes.

Détails en [`docs/01-liste-adjacence.md`](docs/01-liste-adjacence.md) et
[`docs/02-requete-recursive.md`](docs/02-requete-recursive.md).

---

## 6. Base de démonstration (`arborescence`)

Table `noeud` (`parent_id` auto-référencé, `ON DELETE CASCADE`). Petit arbre amorcé.
Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **5/5** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

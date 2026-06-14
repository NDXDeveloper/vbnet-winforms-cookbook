# Raccourcis clavier — combinaisons, accords multi-frappes, conflits

> Projet pédagogique : gérer des raccourcis clavier en VB.NET (.NET Framework
> 4.8.1) — analyse et normalisation des combinaisons, **accords** à plusieurs
> frappes (ex. `Ctrl+K, Ctrl+S`), reconnaissance par machine à états et détection
> de conflits — avec galerie WinForms et persistance des liaisons en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Combinaison** | analyse/normalisation `Ctrl+Maj+P` (sur `Keys`), alias FR | `Saisie/Combinaison.vb` |
| **Accord** | suite de combinaisons `Ctrl+K, Ctrl+S`, préfixes | `Saisie/Raccourci.vb` |
| **Reconnaissance** | machine à états (déclenché / en attente / aucun) | `Saisie/GestionnaireRaccourcis.vb` |
| **Conflits** | refus des doublons et des conflits de préfixe | `Saisie/GestionnaireRaccourcis.vb` |
| **Persistance** | liaisons action ↔ raccourci en base (upsert) | `Persistance/DepotRaccourci.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "raccourcis" sur 127.0.0.1:3316
```

1. Ouvrir `src/NDX.Raccourcis.sln` ; démarrer **`NDX.Raccourcis.UI`** ; **F5**.
2. Page *Démonstration* : cliquer dans la zone, puis taper `Ctrl+S`, `Ctrl+Maj+P`,
   ou l'accord `Ctrl+K` puis `Ctrl+S`.
3. Page *Liaisons* : définir des liaisons, vérifier l'absence de conflit.

```bash
cd src && dotnet test NDX.Raccourcis.sln    # 26 tests (24 logique + 2 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Raccourcis.Core/   → combinaison + accord + gestionnaire + persistance
├── NDX.Raccourcis.UI/     → galerie WinForms (capture clavier en direct)
└── NDX.Raccourcis.Tests/  → tests MSTest
```

Toute la logique (analyse, normalisation, machine à états, conflits) est **pure**
et testée hors interface. La galerie capture les frappes via `ProcessCmdKey` (qui
intercepte aussi `Ctrl+S`, `Tab`…) et les transmet au gestionnaire.

---

## 5. Idées clés

- **Accord** = séquence de frappes. La reconnaissance est une **machine à états** :
  *en attente* tant que la séquence est un préfixe valide, *déclenché* à la
  correspondance exacte, *réinitialisé* sinon.
- **Conflit de préfixe** : si `Ctrl+K` est inscrit, `Ctrl+K, Ctrl+S` est impossible
  (le premier se déclencherait avant que le second se complète). Détails en
  [`docs/02-accords-et-conflits.md`](docs/02-accords-et-conflits.md).

---

## 6. Base de démonstration (`raccourcis`)

Table `liaison_clavier`. 7 liaisons amorcées. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **26/26** (dont 2 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

# Mise à jour applicative — versionnage sémantique, manifeste, intégrité

> Projet pédagogique : la détection des mises à jour en VB.NET (.NET Framework
> 4.8.1) — type de version comparable (`IComparable`, opérateurs), manifeste des
> versions publiées, et vérification d'intégrité par SHA-256 — avec galerie
> WinForms et base MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Version comparable** | `IComparable(Of T)`, surcharge d'opérateurs, `TryAnalyser` | `Versionnage/VersionSemantique.vb` |
| **Manifeste** | modèle d'une version publiée | `MiseAJour/Publication.vb` |
| **Détection** | dernière version applicable, mise à jour obligatoire | `MiseAJour/ServiceMiseAJour.vb` |
| **Intégrité** | empreinte SHA-256 et vérification d'un paquet | `MiseAJour/ServiceMiseAJour.vb` |
| **Persistance** | lecture / écriture du manifeste en base | `Persistance/DepotPublication.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "deploiement" sur 127.0.0.1:3313
```

1. Ouvrir `src/NDX.MiseAJour.sln` ; démarrer **`NDX.MiseAJour.UI`** ; **F5**.
2. Page *Versions* : comparer deux versions, trier une liste.
3. Page *Publications* : charger le manifeste, rechercher une mise à jour pour la version installée.

```bash
cd src && dotnet test NDX.MiseAJour.sln    # 23 tests (21 logique + 2 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.MiseAJour.Core/   → version sémantique + manifeste + service + persistance
├── NDX.MiseAJour.UI/     → galerie WinForms
└── NDX.MiseAJour.Tests/  → tests MSTest
```

`VersionSemantique` et `ServiceMiseAJour` sont **purs** (aucune dépendance base ni
réseau) : toute la logique de comparaison et de décision est testable hors-ligne.
La base ne fait que **stocker le manifeste**.

---

## 5. Le piège à connaître

Comparer des versions **comme du texte** est faux : `"1.10.0" < "1.2.0"` en
alphabétique, alors que `1.10.0` est **plus récente**. `VersionSemantique` compare
**composante par composante, numériquement**. Détail en
[`docs/01-version-comparable.md`](docs/01-version-comparable.md).

---

## 6. Base de démonstration (`deploiement`)

Table `publication` (manifeste). 6 versions amorcées (`1.0.0` → `1.10.0`, dont la
`1.3.0` obligatoire). Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **23/23** (dont 2 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

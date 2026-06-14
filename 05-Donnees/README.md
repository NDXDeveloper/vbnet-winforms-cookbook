# Couche d'accès aux données — Repository, Unit of Work, mapping, résilience

> Projet pédagogique : les patrons d'accès aux données en ADO.NET (VB.NET, .NET
> Framework 4.8.1) — dépôt CRUD, pagination, transactions, réexécution sur fautes
> transitoires, mapping objet-relationnel — avec galerie WinForms et base MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Repository** | `IDepot(Of T)` + `DepotProduit` : CRUD, pagination, requêtes paramétrées | `Depots/IDepot.vb`, `DepotProduit.vb` |
| **Unit of Work** | connexion + transaction partagées (commit / rollback) | `Persistance/UniteDeTravail.vb` |
| **Mapping O/R** | `DataReader` → objets par réflexion (colonnes aliasées) | `Persistance/Mappeur.vb` |
| **Résilience** | réexécution sur fautes transitoires (interbloquage…) + back-off | `Persistance/Resilience.vb` |
| **Entité** | objet du domaine | `Modele/Produit.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "boutique" sur 127.0.0.1:3311
```

1. Ouvrir `src/NDX.Donnees.sln` ; démarrer **`NDX.Donnees.UI`** ; **F5**.
2. Page *Catalogue* : lister (paginé), ajouter, supprimer.
3. Page *Transaction* : insérer en lot puis valider ou annuler.

```bash
cd src && dotnet test NDX.Donnees.sln    # 5 tests (2 logique + 3 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Donnees.Core/   → entité + dépôt + Unit of Work + mappeur + résilience
├── NDX.Donnees.UI/     → galerie WinForms
└── NDX.Donnees.Tests/  → tests MSTest
```

Le dépôt peut être **autonome** (une connexion par opération, protégée par
résilience) ou rattaché à une **Unit of Work** (opérations dans une même
transaction). Le mapping est **générique** (réflexion) : un nouveau dépôt se
résume à ses requêtes.

---

## 5. Base de démonstration (`boutique`)

Table `produit` (référence unique, désignation, prix HT, stock). 15 produits
amorcés. Voir [`docker/README.md`](docker/README.md).

---

## 6. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **5/5** (dont 3 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 7. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

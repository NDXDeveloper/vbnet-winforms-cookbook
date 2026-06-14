# Réflexion & métaprogrammation — inspection, copie, purge d'événements

> Projet pédagogique : exploiter la **réflexion** .NET en VB.NET (.NET Framework
> 4.8.1) — inspecter les membres d'un type, copier des propriétés par nom, purger
> tous les abonnés d'un événement — avec galerie WinForms et catalogue de membres
> en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Inspection de type** | `GetProperties`/`GetFields`/`GetEvents`, `BindingFlags` | `Reflexion/InspecteurType.vb` |
| **Copie par réflexion** | mapping de propriétés par nom + type compatible | `Reflexion/CopieurProprietes.vb` |
| **Purge d'événements** | atteindre le champ délégué et le remettre à `Nothing` | `Reflexion/PurgeEvenements.vb` |
| **Catalogue** | enregistrer les membres d'un type en base | `Persistance/DepotMetadonnees.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "metadonnees" sur 127.0.0.1:3323
```

1. Ouvrir `src/NDX.Reflexion.sln` ; démarrer **`NDX.Reflexion.UI`** ; **F5**.
2. Page *Inspecteur* : choisir un type, lister ses membres, les **cataloguer en base**.
3. Page *Outils* : copier des propriétés ; abonner/déclencher/**purger** un événement.
4. Page *Catalogue* : consulter les membres catalogués.

```bash
cd src && dotnet test NDX.Reflexion.sln    # 10 tests (9 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Reflexion.Core/   → inspecteur + copieur + purge + modèles + catalogue
├── NDX.Reflexion.UI/     → galerie WinForms
└── NDX.Reflexion.Tests/  → tests MSTest
```

Les trois outils sont **purs** (ils opèrent sur des `Type`/objets) : on les teste
directement sur des classes d'exemple (`Article`, `Personne`).

---

## 5. Les trois usages

- **Inspecter** : découvrir, à l'exécution, les membres d'un type inconnu.
- **Copier** : recopier les propriétés homonymes d'un objet vers un autre, sans
  écrire de mapping à la main.
- **Purger** : retirer d'un coup tous les abonnés d'un événement (évite les fuites).

Détails en [`docs/01-inspection.md`](docs/01-inspection.md) et
[`docs/02-copie-et-purge.md`](docs/02-copie-et-purge.md).

---

## 6. Base de démonstration (`metadonnees`)

Table `descripteur`. Démarre **vide**. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **10/10** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

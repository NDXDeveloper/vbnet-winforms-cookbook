# Sérialisation d'objets en .NET — boîte à outils & démonstration

> Projet pédagogique : une bibliothèque de **sérialisation** et de **persistance**
> d'objets (VB.NET, .NET Framework 4.8.1), accompagnée d'une galerie WinForms et
> d'une base MariaDB de démonstration.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

Ce projet explique, thème par thème, comment **convertir un objet en données**
(et inversement) selon plusieurs formats, comment le **persister** (fichier,
stockage isolé, base de données) et comment **contrôler son intégrité**.

---

## 1. Ce que démontre ce projet

| Thème | Techniques illustrées | Fichier `Core` |
|---|---|---|
| **Formats de sérialisation** | XML lisible, contrat de données XML, **binaire compact sûr**, JSON | `Serialisation/Serialiseur.vb` |
| **Cible mémoire / chaîne / fichier** | `byte()`, chaîne UTF-8, fichier | `Serialisation/Serialiseur.vb` |
| **Stockage isolé** | espace privatif par application/utilisateur (`IsolatedStorage`) | `Serialisation/StockageIsole.vb` |
| **Modèle sérialisable** | objet racine + types imbriqués + collection | `Modele/ModeleDemo.vb` |
| **Persistance en base** | enregistrer un payload en **BLOB**, taille + empreinte **SHA-256**, relecture | `Persistance/DepotDocuments.vb` |
| **Configuration d'accès** | chaîne de connexion via constructeur typé | `Persistance/ConfigBdd.vb` |

Chaque thème fait l'objet d'une fiche détaillée dans [`docs/`](docs/).

> **Note de sécurité.** La sérialisation binaire historique (`BinaryFormatter`)
> n'est **pas** utilisée : obsolète et exposée à des attaques par désérialisation.
> Le format binaire proposé ici s'appuie sur un écrivain binaire XML, sûr et
> performant.

---

## 2. Prérequis

- **Visual Studio Community 2022** (ou plus récent).
- **.NET Framework 4.8.1** (Developer Pack ; fourni avec VS 2022).
- **Docker Desktop** (pour la base MariaDB de démonstration).
- *(facultatif)* le **SDK .NET** pour compiler/tester en ligne de commande.

---

## 3. Démarrage rapide

### 3.1 Lancer la base de données

```bash
cd docker
docker compose up -d --wait
```

MariaDB 12.3 crée la base `coffre` et ses catégories de démonstration. La base
est exposée sur **`127.0.0.1:3308`** (voir [`docker/README.md`](docker/README.md)).

### 3.2 Ouvrir et lancer dans Visual Studio

1. Ouvrir `src/NDX.Serialisation.sln`.
2. Projet de démarrage : **`NDX.Serialisation.UI`**.
3. **F5**. Page *Accueil* → « Tester la connexion » doit afficher la version du
   serveur en vert.

### 3.3 En ligne de commande (facultatif)

```bash
cd src
dotnet build NDX.Serialisation.sln
dotnet test  NDX.Serialisation.sln    # tests (intégration BDD réelle si conteneur démarré)
```

---

## 4. Architecture

Solution **hybride** en trois projets (tous **SDK-style**, ciblant **`net481`**) :

```
src/
├── NDX.Serialisation.sln
├── NDX.Serialisation.Core/   → bibliothèque (sérialisation + persistance)
├── NDX.Serialisation.UI/     → galerie WinForms (consomme Core)
└── NDX.Serialisation.Tests/  → tests MSTest (logique + intégration BDD)
```

- **`Core`** regroupe la sérialisation multi-format, le stockage (fichier /
  isolé) et la persistance en base. Connecteur **`MySql.Data`**, compatible MariaDB.
- **`UI`** est une galerie interactive : une page par thème.
- **`Tests`** valide les allers-retours de sérialisation (sans dépendance) et,
  si le conteneur est démarré, la persistance de bout en bout.

> Le format **SDK-style** est retenu (concision, restauration NuGet automatique,
> compilation sous Visual Studio comme via `dotnet`). Le ciblage reste **.NET
> Framework 4.8.1**.

---

## 5. La base de démonstration (`coffre`)

Un **coffre de documents** : chaque document conserve la forme sérialisée d'un
objet, avec ses métadonnées.

| Table | Rôle |
|---|---|
| `categorie` | classement logique des documents |
| `document` | objet sérialisé (BLOB) + type, format, taille, empreinte SHA-256 |

Détails : [`docker/README.md`](docker/README.md) et
[`docs/04-persistance-bdd.md`](docs/04-persistance-bdd.md).

---

## 6. Tour des techniques

- **Quatre formats**, un seul point d'entrée (`Serialiseur`) :
  `VersOctets` / `DepuisOctets`, `VersChaine` / `DepuisChaine`,
  `SauverFichier` / `ChargerFichier`. → [`docs/02-serialisation.md`](docs/02-serialisation.md)
- **Stockage isolé** : `StockageIsole.Sauver` / `Charger` / `Existe` / `Supprimer`.
  → [`docs/03-fichiers-et-stockage.md`](docs/03-fichiers-et-stockage.md)
- **Persistance** : `DepotDocuments.Enregistrer` (sérialise → BLOB + SHA-256),
  `Recharger` (relit → désérialise), `VerifierIntegrite`.
  → [`docs/04-persistance-bdd.md`](docs/04-persistance-bdd.md)

```vbnet
' Sérialiser puis désérialiser, tout format confondu :
Dim octets As Byte() = Serialiseur.VersOctets(monObjet, FormatSerialisation.Json)
Dim copie As Catalogue = Serialiseur.DepuisOctets(Of Catalogue)(octets, FormatSerialisation.Json)
```

---

## 7. Vérifications effectuées

- `dotnet build` des 3 projets : **0 erreur, 0 avertissement**.
- `dotnet test` : **11 tests** — 7 de sérialisation (allers-retours, fichier,
  stockage isolé) + 4 d'intégration contre un conteneur **MariaDB 12.3 réel** :
  **tous au vert**.
- Démarrage de la galerie WinForms : **sans plantage**.

---

## 8. Licence

Distribué sous licence **MIT** — Copyright (c) 2026 Nicolas DEOUX
&lt;NDXDev@gmail.com&gt;. Voir [`LICENSE`](LICENSE). Chaque fichier source porte
l'en-tête de licence.

---

## 9. Structure des fichiers

```
├── LICENSE                       ← licence MIT
├── README.md                     ← ce document
├── docs/                         ← fiches détaillées par thème
├── docker/                       ← MariaDB 12.3 (compose + scripts d'init)
└── src/
    ├── NDX.Serialisation.sln
    ├── NDX.Serialisation.Core/   ← bibliothèque
    ├── NDX.Serialisation.UI/     ← galerie WinForms
    └── NDX.Serialisation.Tests/  ← tests MSTest
```

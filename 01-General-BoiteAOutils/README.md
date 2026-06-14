# Boîte à outils — techniques de programmation .NET

> Projet pédagogique : une bibliothèque de techniques de programmation .NET
> (VB.NET, .NET Framework 4.8.1), accompagnée d'une galerie WinForms et d'une
> base MariaDB de démonstration.

Ce projet regroupe et explique, **thème par thème**, un ensemble de techniques de
programmation .NET : gestion d'exceptions outillée, accès aux données avec logique
de « seconde chance », threads d'arrière-plan, P/Invoke, GDI+, réflexion, etc.

Le tout est organisé en une **bibliothèque réutilisable** richement commentée, une
**galerie WinForms** interactive et une **base de données de démonstration sous
Docker**.

---

## 1. Ce que démontre ce projet

Les techniques sont réparties par thème dans la bibliothèque `Core` :

| Thème | Techniques illustrées | Fichier `Core` |
|---|---|---|
| **Accès aux données** | `ExecuteNonQuery`/`ExecuteScalar`/`GetDTfromCommand`, requêtes **paramétrées**, transactions, logique de **seconde chance** (retry), `INSERT` + `LAST_INSERT_ID()` | `Bdd/AccesDonnees.vb` |
| **Connexion** | wrapper `IDisposable`, timer d'auto-fermeture, historique d'actions `[n-1..n-5]`, compteur thread-safe | `Infrastructure/ConnexionMySql.vb` |
| **Exceptions** | rapport diagnostic détaillé (pile, frames, code MySQL, exception racine), traitement centralisé **anti-récursion** | `Exceptions/GestionExceptions.vb` |
| **Chaînes** | suppression d'accents (normalisation Unicode), nettoyage par RegEx, détection de GUID | `Chaines/OutilsChaines.vb` |
| **Conversions** | conversion robuste, formatage culture-invariant | `Conversions/OutilsConversions.vb` |
| **Géométrie** | point milieu, distance au carré, rotation (matrice) | `Geometrie/OutilsGeometrie.vb` |
| **Fichiers** | chemins, droits d'écriture, recherche récursive, fusion d'octets | `Fichiers/OutilsFichiers.vb` |
| **Images** | nuances de gris (`ColorMatrix`), détection de format par **signature binaire**, plan vide | `Images/OutilsImages.vb` |
| **Contrôles WinForms** | style de grille, **double buffer par réflexion**, boutons arrondis (`GraphicsPath`), ComboBox *owner-draw*, centrage | `Controles/OutilsControles.vb` |
| **Système / OS** | mémoire (compteur de perf.), utilisateur/machine/session, culture, GC, sérialisation XML | `Systeme/OutilsSysteme.vb` |
| **Réseau** | IP locale, validation IPv4, envoi SMTP, **moniteur `BackgroundWorker`** | `Reseau/OutilsReseau.vb`, `Reseau/MoniteurReseau.vb` |
| **Processus** | arrêt de processus, P/Invoke `GetWindowThreadProcessId` | `Processus/OutilsProcessus.vb` |
| **Interop** | `DllImport` (`SystemParametersInfo`, etc.) | `Interop/ApiWindows.vb` |

Chaque thème fait l'objet d'une fiche détaillée dans [`docs/`](docs/).

---

## 2. Prérequis

- **Visual Studio Community 2022** (ou plus récent).
- **.NET Framework 4.8.1** (Developer Pack ; fourni avec VS 2022).
- **Docker Desktop** (pour la base MariaDB de démonstration).
- *(facultatif)* le **SDK .NET** pour compiler/tester en ligne de commande
  (`dotnet build`, `dotnet test`).

---

## 3. Démarrage rapide

### 3.1 Lancer la base de données

```bash
cd docker
docker compose up -d --wait
```

Au premier démarrage, MariaDB 12.3 crée la base `etabli`, le compte applicatif
`etabli_app`, puis exécute le schéma et les données de démonstration
(voir [`docker/README.md`](docker/README.md)). La base est exposée sur
**`127.0.0.1:3307`**.

### 3.2 Ouvrir et lancer dans Visual Studio

1. Ouvrir `src/BoiteAOutils.sln`.
2. Projet de démarrage : **`BoiteAOutils.UI`**.
3. **F5**. Dans la galerie, page *Accueil* → bouton **« Tester la connexion »**
   doit afficher la version du serveur en vert.

### 3.3 En ligne de commande (facultatif)

```bash
cd src
dotnet build BoiteAOutils.sln       # compile les 3 projets
dotnet test  BoiteAOutils.sln       # exécute les tests (BDD réelle si conteneur démarré)
```

---

## 4. Architecture

Solution **hybride** en trois projets (tous **SDK-style**, ciblant **`net481`**) :

```
src/
├── BoiteAOutils.sln
├── BoiteAOutils.Core/      → bibliothèque réutilisable (les techniques)
├── BoiteAOutils.UI/        → galerie WinForms (consomme Core)
└── BoiteAOutils.Tests/     → tests MSTest (logique + intégration BDD)
```

- **`Core`** regroupe toutes les techniques dans des modules réutilisables. Elle
  utilise le connecteur **`MySql.Data`** (`Imports MySql.Data.MySqlClient`),
  pleinement compatible avec MariaDB.
- **`UI`** est une galerie : un menu à gauche, une page de démonstration par
  thème, et **un journal applicatif en bas** qui trace en direct le cycle de vie
  des connexions et des requêtes — le meilleur moyen d'observer les techniques.
- **`Tests`** valide la logique pure (sans dépendance) et, si le conteneur est
  démarré, l'accès aux données de bout en bout.

> **Choix de format de projet.** Le format **SDK-style** est retenu : plus concis,
> restauration NuGet automatique (sans `nuget.exe`), compilation aussi bien sous
> Visual Studio que via `dotnet`. Le ciblage reste **.NET Framework 4.8.1**.

---

## 5. La base de données de démonstration (`etabli`)

Le domaine retenu est une **gestion de commandes d'atelier**, choisi pour sa
clarté : il met en jeu une hiérarchie relationnelle (secteurs → clients →
commandes → lignes), un catalogue, des fiches techniques (colonnes calculées) et
un journal d'incidents.

| Table | Rôle | Technique illustrée |
|---|---|---|
| `secteur` | zones commerciales (référence) | table de référence |
| `client` | clients (datés, actif/inactif) | statistiques groupées |
| `commande` | en-têtes de commande (datées) | entité datée (filtre ≥ date) |
| `article` | catalogue (libellé, prix réf.) | libellé + prix unitaire |
| `fiche_article` | caractéristiques | **colonnes calculées** (ratios) |
| `ligne_commande` | détail (quantité × prix) | **agrégats** quantité × prix |
| `journal_erreur` | journal applicatif des exceptions | `INSERT` + `LAST_INSERT_ID()` |

Détails, schéma et exploitation : [`docker/README.md`](docker/README.md) et
[`docs/02-base-de-donnees.md`](docs/02-base-de-donnees.md).

---

## 6. Tour des techniques

Chaque fiche détaillée se trouve dans `docs/`. Aperçu des plus marquantes.

### 6.1 Accès aux données « avec seconde chance »

Trois méthodes (`ExecuteNonQuery`, `ExecuteScalar`, `GetDTfromCommand`) qui, sur
une **erreur transitoire** (interbloquage InnoDB, `DataReader` resté ouvert),
**régénèrent la connexion et rejouent** la commande, dans la limite d'un nombre
d'essais — tout en consignant un **historique des dernières actions SQL** pour le
diagnostic.

```vbnet
' Toujours des requêtes PARAMÉTRÉES (@param) : protection contre l'injection SQL.
Using oMy As New ConnexionMySql(False, MethodBase.GetCurrentMethod())
    oMy.Open(MethodBase.GetCurrentMethod())
    Using cmd As New MySqlCommand(requete, oMy.Conn, oMy.Tr)
        cmd.Parameters.AddWithValue("@idFiche", idFiche)
        resultat = GetDTfromCommand(cmd, False, oMy, MethodBase.GetCurrentMethod(), Environment.StackTrace)
    End Using
End Using
```

→ [`docs/02-base-de-donnees.md`](docs/02-base-de-donnees.md)

### 6.2 Gestion d'exceptions outillée

`PreparerException` construit un rapport riche (message, code MariaDB, pile,
première frame source, exception racine). `TraiterException` le journalise —
avec une **garde anti-récursion** indispensable, car la journalisation en base
peut elle-même échouer.

→ [`docs/03-exceptions.md`](docs/03-exceptions.md)

### 6.3 Le double buffer par réflexion, les coins arrondis, l'owner-draw…

La propriété `DoubleBuffered` d'un `DataGridView` est *protégée* : on y accède par
**réflexion**. Les boutons arrondis s'obtiennent en affectant une `Region`
calculée par un `GraphicsPath`. Une `ComboBox` peut être **dessinée
intégralement** (owner-draw).

→ [`docs/07-controles-winforms.md`](docs/07-controles-winforms.md)

### 6.4 Threads d'arrière-plan

`MoniteurReseau` illustre le patron `BackgroundWorker` : une boucle de test sur un
thread d'arrière-plan, dont les résultats **remontent vers le thread d'interface**
via `ReportProgress` (donc manipulables sans risque dans l'UI).

→ [`docs/08-systeme-reseau-processus.md`](docs/08-systeme-reseau-processus.md)

---

## 7. Le journal applicatif

En bas de la galerie, le journal affiche en temps réel les écritures de
`Journal.Ecrire(...)` : ouverture/fermeture de connexion, début/commit/rollback
de transaction, fermeture automatique sur inactivité, rapports d'exception…
C'est l'outil d'observation central du projet.

---

## 8. Vérifications effectuées

Ce projet a été **compilé et testé** avant livraison :

- `dotnet build` des 3 projets : **0 erreur, 0 avertissement**.
- `dotnet test` : **20 tests** — 15 de logique pure + 5 d'intégration exécutés
  contre un conteneur **MariaDB 12.3 réel** : **tous au vert**.
- Démarrage de la galerie WinForms : **sans plantage**.
- Conteneur Docker : image `mariadb:12.3` tirée, base initialisée, requêtes OK.

---

## 9. Choix de conception notables

1. **Techniques autonomes** : aucune dépendance applicative externe ; chaque
   module se suffit à lui-même.
2. **Configuration de session paresseuse** : `ConnexionMySql` ne se connecte pas
   dans son constructeur (robustesse si la base est arrêtée).
3. **Garde anti-récursion** dans le traitement d'exception.
4. **Validation IPv4 stricte** (format **et** bornes 0–255).
5. **Connecteur** `MySql.Data`, compatible MariaDB au niveau du protocole : aucun
   code spécifique n'est requis.

---

## 10. Structure des fichiers

```
├── README.md                     ← ce document
├── docs/                         ← fiches détaillées par thème
├── docker/                       ← MariaDB 12.3 (compose + scripts d'init)
│   ├── docker-compose.yml
│   ├── .env.example
│   └── init/ (01-schema.sql, 02-donnees.sql)
└── src/
    ├── BoiteAOutils.sln
    ├── BoiteAOutils.Core/  ← la bibliothèque (techniques)
    ├── BoiteAOutils.UI/    ← la galerie WinForms
    └── BoiteAOutils.Tests/ ← les tests MSTest
```

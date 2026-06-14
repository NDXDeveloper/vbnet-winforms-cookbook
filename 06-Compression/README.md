# Compression de données — GZip / Deflate, ratio de gain, stockage compressé

> Projet pédagogique : la compression de données en VB.NET (.NET Framework 4.8.1)
> — flux `GZipStream` / `DeflateStream`, calcul du gain de place, et technique
> « compress-then-store » (stocker en base la version compressée avec contrôle
> d'intégrité SHA-256) — avec galerie WinForms et base MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Compression** | `Compresser` / `Decompresser` d'octets et de texte UTF-8 | `Compression/Compresseur.vb` |
| **Algorithmes** | DEFLATE (RFC 1951) et GZIP (RFC 1952), tous deux intégrés à .NET | `Compression/AlgorithmeCompression.vb` |
| **Mesure du gain** | ratio (compressé / origine) et pourcentage de gain | `Compression/Compresseur.vb` |
| **Compress-then-store** | stocker en base le binaire compressé + tailles + SHA-256 | `Persistance/DepotArchive.vb` |
| **Configuration** | chaîne de connexion paramétrable (`App.config`) | `Persistance/ConfigBdd.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "archive" sur 127.0.0.1:3312
```

1. Ouvrir `src/NDX.Compression.sln` ; démarrer **`NDX.Compression.UI`** ; **F5**.
2. Page *Compression (mémoire)* : compresser un texte, lire le gain, vérifier l'aller-retour.
3. Page *Archives (base)* : enregistrer (compressé), lister, recharger et vérifier.

```bash
cd src && dotnet test NDX.Compression.sln    # 10 tests (8 logique + 2 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Compression.Core/   → compresseur (GZip/Deflate) + dépôt d'archives
├── NDX.Compression.UI/     → galerie WinForms
└── NDX.Compression.Tests/  → tests MSTest
```

Le **compresseur** est autonome (mémoire pure, aucune dépendance base). Le **dépôt**
ajoute la persistance « compress-then-store » : il compresse avant l'insertion,
mémorise les deux tailles et l'empreinte SHA-256 des données d'origine, puis
restitue les octets exacts à la relecture.

---

## 5. Le piège à connaître

Un flux de compression ne vide ses derniers octets **qu'à sa fermeture**. Il faut
donc fermer le `GZipStream`/`DeflateStream` **avant** de lire le tampon de sortie,
tout en gardant ce tampon ouvert (`leaveOpen:=True`). Détail en
[`docs/01-flux-compression.md`](docs/01-flux-compression.md).

---

## 6. Base de démonstration (`archive`)

Table `archive` (nom, algorithme, tailles, contenu binaire, SHA-256). La table
démarre **vide** : c'est l'application qui compresse puis stocke. Voir
[`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **10/10** (dont 2 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

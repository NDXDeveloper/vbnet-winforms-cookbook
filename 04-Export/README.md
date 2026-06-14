# Export de données — CSV, Excel (.xlsx), PDF

> Projet pédagogique : exporter un `DataTable` vers **CSV**, **Excel .xlsx**
> (Office Open XML, sans Excel installé) et **PDF** — le tout **sans dépendance
> tierce** — avec une galerie WinForms et une base MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Contrat commun** | `IExportateur` (format, extension, type MIME) | `Export/IExportateur.vb` |
| **CSV** | échappement RFC 4180, séparateur, BOM UTF-8 | `Export/ExportateurCsv.vb` |
| **Excel .xlsx** | Office Open XML assemblé via `ZipArchive` (parties XML, `inlineStr`, cellules numériques) | `Export/ExportateurExcel.vb` |
| **PDF** | génération bas niveau (objets, `xref`, `trailer`, police Courier, pagination) | `Export/ExportateurPdf.vb` |
| **Fabrique** | sélection par format + raccourcis fichier | `Export/Exportateurs.vb` |
| **Source** | requêtes (liste + agrégats) à exporter | `Persistance/SourceDonnees.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "entrepot" sur 127.0.0.1:3310
```

1. Ouvrir `src/NDX.Export.sln` ; démarrer **`NDX.Export.UI`** ; **F5**.
2. Page *Exporter des données* : choisir une source + un format, **Charger**, puis
   **Exporter vers un fichier**.

```bash
cd src && dotnet test NDX.Export.sln    # 7 tests (4 logique + 3 intégration)
```

---

## 4. Architecture

Solution hybride, trois projets SDK-style **`net481`** :

```
src/
├── NDX.Export.Core/   → bibliothèque (exportateurs + source de données)
├── NDX.Export.UI/     → galerie WinForms
└── NDX.Export.Tests/  → tests MSTest
```

Tous les exportateurs partagent l'interface **`IExportateur`** : ajouter un format
(HTML, Markdown…) revient à écrire une nouvelle classe, sans toucher au reste.

---

## 5. Base de démonstration (`entrepot`)

Table `vente` (date, produit, catégorie, quantité, montant). Deux sources
proposées : liste détaillée et synthèse par catégorie. Voir
[`docker/README.md`](docker/README.md).

---

## 6. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **7/7** (dont 3 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 7. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;. En-tête de
licence dans chaque fichier source.

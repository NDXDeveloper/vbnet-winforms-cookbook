# Impression de documents — pagination, aperçu et `PrintDocument`

> Projet pédagogique : imprimer du texte multi-pages en VB.NET (.NET Framework
> 4.8.1) — pagination calculée, aperçu avant impression (`PrintPreviewDialog`) et
> impression réelle (`PrintDocument` + `PrintDialog`) — avec galerie WinForms et
> historique des documents en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Pagination** | lignes/page, nombre de pages, lignes d'une page (pur) | `Impression/Paginateur.vb` |
| **Impression** | `PrintDocument`, `PrintPage`, `HasMorePages` | `Impression/ImprimeurTexte.vb` |
| **Aperçu / dialogue** | `PrintPreviewDialog`, `PrintDialog` | galerie `Pages/PageImpression.vb` |
| **Historique** | documents soumis à l'impression | `Persistance/DepotImpression.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.
(Un aperçu/impression nécessite au moins une imprimante installée — par ex.
« Microsoft Print to PDF », présente par défaut sous Windows.)

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "impressions" sur 127.0.0.1:3321
```

1. Ouvrir `src/NDX.Impression.sln` ; démarrer **`NDX.Impression.UI`** ; **F5**.
2. Page *Aperçu & impression* : saisir un texte, **Aperçu** (paginé), **Imprimer**,
   ou **Archiver en base**.
3. Page *Historique* : relire un document archivé.

```bash
cd src && dotnet test NDX.Impression.sln    # 9 tests (8 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Impression.Core/   → paginateur (pur) + imprimeur PrintDocument + historique
├── NDX.Impression.UI/     → galerie WinForms (aperçu, impression)
└── NDX.Impression.Tests/  → tests MSTest
```

L'arithmétique de la pagination (`Paginateur`) est **isolée et testée sans
imprimante**. L'`ImprimeurTexte` ne fait que le rendu : à chaque `PrintPage`, il
dessine les lignes qui tiennent puis positionne `HasMorePages`.

---

## 5. L'idée clé : `PrintDocument` et `HasMorePages`

L'impression .NET est **événementielle** : on s'abonne à `PrintPage`, on dessine une
page via l'objet `Graphics`, et on indique s'il reste des pages
(`e.HasMorePages = True`) — le moteur rappelle alors `PrintPage`. Le même
`PrintDocument` sert à l'aperçu (`PrintPreviewDialog`) **et** à l'impression
(`PrintDialog` + `Print`). Détails en
[`docs/01-printdocument.md`](docs/01-printdocument.md) et
[`docs/02-pagination.md`](docs/02-pagination.md).

---

## 6. Base de démonstration (`impressions`)

Table `impression`. 2 documents amorcés. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **9/9** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

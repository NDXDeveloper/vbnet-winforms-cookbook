# Composition de PDF — multi-pages, polices standard, mise en page (100 % VB.NET)

> Projet pédagogique : générer des fichiers PDF **sans aucune bibliothèque tierce
> ni binaire natif**, en VB.NET (.NET Framework 4.8.1). On écrit la structure du
> PDF octet par octet (objets, table de références croisées, trailer), avec une
> API de dessin, les polices standard et le retour à la ligne — galerie WinForms
> et archivage en base MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Structure PDF** | en-tête, objets, `xref`, `trailer`, encodage WinAnsi | `Pdf/DocumentPdf.vb` |
| **API de dessin** | texte, lignes, rectangles, repère haut-gauche | `Pdf/PagePdf.vb` |
| **Polices standard** | les polices de base PDF (Helvetica, Times, Courier) | `Pdf/PoliceStandard.vb` |
| **Mise en page** | retour à la ligne exact (Courier, chasse fixe) — testable | `Pdf/EnrouleurTexte.vb` |
| **Archivage** | stocker / relire le PDF produit en base | `Persistance/DepotDocument.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "bibliotheque" sur 127.0.0.1:3315
```

1. Ouvrir `src/NDX.Pdf.sln` ; démarrer **`NDX.Pdf.UI`** ; **F5**.
2. Page *Composer un PDF* : saisir un titre/auteur/corps, **Générer**, **Enregistrer**,
   **Ouvrir** (visionneuse par défaut), **Archiver en base**.
3. Page *Bibliothèque* : relire un document archivé et l'ouvrir.

```bash
cd src && dotnet test NDX.Pdf.sln    # 16 tests (14 logique + 2 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Pdf.Core/   → moteur PDF (document, page, polices, mise en page) + archivage
├── NDX.Pdf.UI/     → galerie WinForms
└── NDX.Pdf.Tests/  → tests MSTest
```

Le moteur est **autonome** : `DocumentPdf.Construire()` renvoie un `Byte()` qu'on
peut écrire sur disque, ouvrir dans n'importe quelle visionneuse ou stocker en base.
La logique de mise en page (`EnrouleurTexte`) est **isolée** et testée sans rien
dessiner.

---

## 5. Idées clés

- Un PDF minimal = quelques **objets numérotés**, une **table d'offsets** (`xref`)
  et un **trailer** qui pointe vers le catalogue. Détails en
  [`docs/01-anatomie-pdf.md`](docs/01-anatomie-pdf.md).
- Les **accents** passent par l'encodage `WinAnsiEncoding` des polices standard
  (texte sérialisé en Windows-1252).
- Le **retour à la ligne** est exact pour Courier (chasse fixe) :
  [`docs/02-texte-et-mise-en-page.md`](docs/02-texte-et-mise-en-page.md).

---

## 6. Base de démonstration (`bibliotheque`)

Table `document_genere` (PDF archivés). Démarre **vide** : l'application produit le
binaire. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **16/16** (dont 2 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage** ; PDF généré ouvrable dans une visionneuse.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

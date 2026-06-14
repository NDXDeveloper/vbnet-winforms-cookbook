# Éditeur de texte enrichi — `RichTextBox`, RTF et statistiques

> Projet pédagogique : un éditeur de texte **enrichi** en VB.NET (.NET Framework
> 4.8.1) — `RichTextBox` + barre de mise en forme (gras, italique, couleur),
> contenu au format **RTF**, et statistiques en direct (mots/caractères/lignes) —
> avec galerie WinForms et documents persistés en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier |
|---|---|---|
| **Mise en forme** | `RichTextBox`, `SelectionFont`, `SelectionColor` | UI `Pages/PageEditeur.vb` |
| **RTF** | sérialisation du contenu enrichi (`RichTextBox.Rtf`) | UI + `Persistance/DepotDocument.vb` |
| **Statistiques** | mots / caractères / lignes — calcul pur, testable | `Edition/StatistiquesTexte.vb` |
| **Persistance** | documents RTF en base | `Persistance/DepotDocument.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "redaction" sur 127.0.0.1:3328
```

1. Ouvrir `src/NDX.Editeur.sln` ; démarrer **`NDX.Editeur.UI`** ; **F5**.
2. Page *Éditeur RTF* : taper, sélectionner, appliquer **G** / **I** / **S** /
   *Couleur* ; voir les statistiques en bas ; **Enregistrer** / **Charger**.
3. Page *Documents* : lister les documents enregistrés.

```bash
cd src && dotnet test NDX.Editeur.sln    # 5 tests (4 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Editeur.Core/   → statistiques (pures) + persistance RTF
├── NDX.Editeur.UI/     → galerie WinForms (RichTextBox + barre de mise en forme)
└── NDX.Editeur.Tests/  → tests MSTest
```

La mise en forme s'appuie sur le `RichTextBox` (UI). La logique « comptable »
(`StatistiquesTexte`) est **séparée et pure** : on la teste sans interface. Le
contenu est stocké tel quel au format **RTF**.

---

## 5. Idées clés

- **`SelectionFont` / `SelectionColor`** appliquent un style à la sélection
  courante ; basculer un style = lire le style courant et inverser le drapeau.
- **RTF** = le format texte enrichi natif du `RichTextBox` (`.Rtf`) : on l'enregistre
  et on le recharge tel quel. Détails en
  [`docs/01-richtextbox-rtf.md`](docs/01-richtextbox-rtf.md) et
  [`docs/02-statistiques.md`](docs/02-statistiques.md).

---

## 6. Base de démonstration (`redaction`)

Table `document` (RTF). 1 document amorcé. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **5/5** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

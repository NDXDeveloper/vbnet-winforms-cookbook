# Tableur `.xlsx` — lecture/écriture sans Office (+ Excel COM optionnel)

> Projet pédagogique : lire et écrire des fichiers **Excel `.xlsx`** en VB.NET (.NET
> Framework 4.8.1) **sans installer Office** (un .xlsx est une archive ZIP de XML —
> OpenXML), avec en plus une voie **optionnelle** par automation COM d'Excel — le
> tout avec galerie WinForms et échanges base ↔ classeur en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Lecture/écriture .xlsx** | OpenXML (ZIP + XML), sans dépendance | `Tableur/ClasseurXlsx.vb` |
| **Référence de cellule** | « A1 » ↔ (colonne, ligne), base 26 bijective (pur) | `Tableur/ReferenceCellule.vb` |
| **Automation Excel (COM)** | liaison tardive, **optionnelle** (si Excel installé) | `Tableur/AutomationExcel.vb` |
| **Échanges base ↔ xlsx** | exporter / importer une table | `Persistance/DepotClasseur.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.
Microsoft Excel **n'est pas requis** : il n'est utile que pour la voie COM
*optionnelle*.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "classeur" sur 127.0.0.1:3324
```

1. Ouvrir `src/NDX.Office.sln` ; démarrer **`NDX.Office.UI`** ; **F5**.
2. Page *Tableur* : éditer la grille, **écrire** un .xlsx (sans Office), le **relire**.
3. Page *Données* : **exporter** la table en .xlsx, **importer** un .xlsx dans la table.

```bash
cd src && dotnet test NDX.Office.sln    # 19 tests : 18 verts + 1 « Inconclusive » (Excel COM absent) ; dont 1 intégration
```

---

## 4. Architecture

```
src/
├── NDX.Office.Core/   → classeur xlsx (sans Office) + référence A1 + COM Excel + échanges base
├── NDX.Office.UI/     → galerie WinForms
└── NDX.Office.Tests/  → tests MSTest
```

La voie **par défaut** (`ClasseurXlsx`) ne dépend de rien : un `.xlsx` est créé/lu
comme une **archive ZIP** de parties XML. La conversion de référence (`A1`) est
**pure** (testée). La voie **COM** (`AutomationExcel`) est isolée dans un fichier
`Option Strict Off` et n'est activée que si Excel est présent.

---

## 5. À noter (sans Excel)

- **Écriture/lecture .xlsx** : pleinement fonctionnelles et **testées** sans Office,
  via OpenXML. Détails en [`docs/01-xlsx-openxml.md`](docs/01-xlsx-openxml.md).
- **Automation Excel par COM** : se **compile** toujours ; à l'exécution, elle est
  proposée seulement si Excel est installé (sinon un message renvoie vers la voie
  « sans Office »). Le test correspondant passe en **Inconclusive** en l'absence
  d'Excel. Détails en [`docs/02-excel-com.md`](docs/02-excel-com.md).

---

## 6. Base de démonstration (`classeur`)

Table `enregistrement` (3 colonnes). 3 lignes amorcées. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **18 réussis + 1 *Inconclusive*** (le test COM, Excel absent) sur 19
  ; la voie « sans Office » est testée par aller-retour ; 1 test d'intégration **MariaDB 12.3**.
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

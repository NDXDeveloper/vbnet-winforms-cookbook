# Démarrage applicatif — séquence d'étapes, splash et exceptions globales

> Projet pédagogique : structurer le **cycle de vie** d'une application WinForms en
> VB.NET (.NET Framework 4.8.1) — écran de démarrage (splash), **séquence d'étapes**
> d'initialisation, et **capture globale des exceptions** (au lieu de planter) —
> avec galerie WinForms et journal en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Séquence de démarrage** | étapes ordonnées, arrêt/poursuite, résultats (pur) | `Demarrage/SequenceDemarrage.vb` |
| **Capture globale** | `Application.ThreadException` + `AppDomain.UnhandledException` | `Demarrage/GestionnaireExceptions.vb` |
| **Splash** | écran de démarrage sans bordure | UI `FrmChargement.vb` |
| **Journal** | étapes et exceptions enregistrées en base | `Persistance/DepotEvenement.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "demarrage" sur 127.0.0.1:3330
```

1. Ouvrir `src/NDX.Demarrage.sln` ; démarrer **`NDX.Demarrage.UI`** ; **F5**
   (un écran de démarrage s'affiche brièvement).
2. Page *Séquence de démarrage* : lancer les étapes (l'une échoue volontairement).
3. Page *Exceptions & journal* : déclencher une exception → elle est **interceptée**,
   affichée et journalisée ; consulter le journal.

```bash
cd src && dotnet test NDX.Demarrage.sln    # 7 tests (6 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Demarrage.Core/   → séquence d'étapes + capture d'exceptions + journal
├── NDX.Demarrage.UI/     → galerie WinForms + splash (installe la capture dans Main)
└── NDX.Demarrage.Tests/  → tests MSTest
```

L'orchestration des étapes (`SequenceDemarrage`) est **pure** : on la teste sans
interface. La **capture globale** s'installe dans `Program.Main`, **avant** toute
fenêtre (exigence de `SetUnhandledExceptionMode`).

---

## 5. Les idées clés

- **Séquence d'étapes** : initialiser proprement = exécuter des vérifications dans
  l'ordre, collecter les résultats, décider d'arrêter au premier échec.
- **Capture globale** : une application robuste ne plante pas sur une exception
  imprévue ; elle l'**intercepte**, la **journalise** et **informe** l'utilisateur.

Détails en [`docs/01-sequence-demarrage.md`](docs/01-sequence-demarrage.md) et
[`docs/02-exceptions-globales.md`](docs/02-exceptions-globales.md).

---

## 6. Base de démonstration (`demarrage`)

Table `evenement` (journal). Démarre **vide**. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **7/7** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage** (splash puis fenêtre principale).

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

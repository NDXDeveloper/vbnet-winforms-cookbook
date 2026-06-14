# Interopérabilité Windows — P/Invoke (inactivité, ressources GDI, premier plan)

> Projet pédagogique : appeler les fonctions natives de Windows depuis VB.NET (.NET
> Framework 4.8.1) via **P/Invoke** (`DllImport`) — durée d'inactivité, comptage des
> objets GDI/USER, fenêtre « toujours au premier plan » — avec galerie WinForms et
> historique des relevés en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Déclarations P/Invoke** | `DllImport`, `StructLayout`, types non signés | `Interop/NatifWin32.vb` |
| **Inactivité** | `GetLastInputInfo` + gestion du **repli** 32 bits (calcul pur) | `Interop/Inactivite.vb` |
| **Ressources** | `GetGuiResources` (objets GDI / USER du processus) | `Interop/RessourcesProcessus.vb` |
| **Fenêtre** | `SetWindowPos` (« toujours au premier plan ») | `Interop/FenetrePremierPlan.vb` |
| **Historique** | enregistrement des relevés en base | `Persistance/DepotSupervision.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**, **Windows**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "supervision" sur 127.0.0.1:3319
```

1. Ouvrir `src/NDX.Interop.sln` ; démarrer **`NDX.Interop.UI`** ; **F5**.
2. Page *Supervision* : voir l'inactivité et les objets GDI/USER en temps réel ;
   cocher « toujours au premier plan » ; enregistrer un relevé.
3. Page *Historique* : consulter les relevés en base.

```bash
cd src && dotnet test NDX.Interop.sln    # 7 tests (6 logique/Win32 + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Interop.Core/   → déclarations P/Invoke + inactivité + ressources + fenêtre + persistance
├── NDX.Interop.UI/     → galerie WinForms (relevés temps réel)
└── NDX.Interop.Tests/  → tests MSTest
```

Les déclarations natives sont **isolées** dans un module interne (`NatifWin32`). La
partie sensible — le calcul du delta de ticks avec **repli** — est extraite dans une
fonction **pure**, testée exhaustivement sans appel système.

---

## 5. Les deux points délicats

- **Le marshalling** : faire correspondre exactement la signature managée et la
  fonction native (`StructLayout`, `UInteger`, `ByRef`). Détails en
  [`docs/01-pinvoke-marshalling.md`](docs/01-pinvoke-marshalling.md).
- **Le repli du compteur de ticks** : `Environment.TickCount` et le tick de la
  dernière entrée reviennent à zéro toutes les ~49,7 jours ; le calcul doit le gérer.
  Détails en [`docs/02-inactivite-et-ressources.md`](docs/02-inactivite-et-ressources.md).

---

## 6. Base de démonstration (`supervision`)

Table `echantillon` (relevés). Démarre **vide**. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **7/7** (dont 1 d'intégration contre **MariaDB 12.3** ; les appels Win32 sont réels).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

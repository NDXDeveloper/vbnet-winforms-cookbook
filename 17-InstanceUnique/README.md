# Instance unique & IPC — `Mutex` et tube nommé

> Projet pédagogique : empêcher le double lancement d'une application et faire
> dialoguer les instances en VB.NET (.NET Framework 4.8.1) — `Mutex` nommé pour
> l'unicité, **tube nommé** (named pipe) pour transmettre les arguments à l'instance
> déjà ouverte — avec galerie WinForms et journal des commandes en MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Instance unique** | `Mutex` nommé (`createdNew`), `IDisposable` | `Instance/VerrouInstance.vb` |
| **IPC** | tube nommé (`NamedPipeServerStream` / `ClientStream`) | `Instance/CanalCommande.vb` |
| **Sérialisation d'arguments** | encodage/décodage d'une ligne (échappement, pur) | `Instance/Commande.vb` |
| **Journal** | enregistrement des commandes reçues | `Persistance/DepotCommande.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "commandes" sur 127.0.0.1:3322
```

1. Ouvrir `src/NDX.InstanceUnique.sln` ; démarrer **`NDX.InstanceUnique.UI`** ; **F5**.
2. Page *Instance & messages* : **Démarrer l'écoute** (instance primaire), puis
   **Simuler une 2ᵉ instance** — ses arguments arrivent via le tube nommé et sont journalisés.
3. Page *Journal* : consulter les commandes reçues.

```bash
cd src && dotnet test NDX.InstanceUnique.sln    # 9 tests (8 logique/IPC + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.InstanceUnique.Core/   → verrou Mutex + canal tube nommé + encodage + journal
├── NDX.InstanceUnique.UI/     → galerie WinForms
└── NDX.InstanceUnique.Tests/  → tests MSTest (pur, en mémoire, et IPC local)
```

L'encodage des arguments (`Commande`) est **pur** (testé par aller-retour). Le verrou
se teste **en mémoire** (deux `VerrouInstance` de même nom dans le processus). L'IPC
se teste **localement** (serveur sur une tâche, client qui envoie).

---

## 5. Les trois pièces

- **`Mutex` nommé** : la 1ʳᵉ instance le crée (`createdNew = True`) ; les suivantes
  le trouvent existant. C'est le verrou d'unicité.
- **Tube nommé** : canal local par lequel la nouvelle instance transmet ses
  arguments à celle qui écoute, avant de se retirer.
- **Encodage** : une liste d'arguments → une ligne (et retour), avec échappement.

Détails en [`docs/01-instance-unique.md`](docs/01-instance-unique.md) et
[`docs/02-tube-nomme-ipc.md`](docs/02-tube-nomme-ipc.md).

---

## 6. Base de démonstration (`commandes`)

Table `commande_recue`. Démarre **vide**. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **9/9** (dont 1 d'intégration contre **MariaDB 12.3** ; l'IPC est testé localement).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

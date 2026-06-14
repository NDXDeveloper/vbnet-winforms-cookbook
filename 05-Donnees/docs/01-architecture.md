# 01 — Architecture

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

```
        NDX.Donnees.UI               NDX.Donnees.Tests
        (WinExe, WinForms)           (MSTest)
                 │                          │
                 └────────────┬─────────────┘
                              ▼
                     NDX.Donnees.Core
                     Repository · Unit of Work · Mappeur · Resilience
                              │
                              ▼
                       MariaDB 12.3 (Docker)
```

## Couches

- **Entité** (`Produit`) : objet du domaine, propriétés publiques.
- **Mappeur** : transforme un `DataReader` en objets (réflexion).
- **Résilience** : rejoue les opérations sur fautes transitoires.
- **Unit of Work** : connexion + transaction partagées.
- **Dépôt** (`DepotProduit`) : requêtes paramétrées + mapping, autonome ou
  rattaché à une Unit of Work.

## Deux modes du dépôt

| Mode | Connexion | Transaction | Résilience |
|---|---|---|---|
| autonome (`New DepotProduit()`) | une par opération | non | oui |
| transactionnel (`New DepotProduit(uow)`) | celle de l'UoW | celle de l'UoW | non (gérée au niveau UoW) |

Le commutateur est interne (`AvecConnexion`) : les méthodes CRUD sont écrites une
seule fois et fonctionnent dans les deux modes.

## Format de projet

SDK-style, cible **`net481`**, `Option Strict On`.

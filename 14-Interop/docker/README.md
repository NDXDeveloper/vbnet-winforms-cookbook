# Base de démonstration `supervision` — MariaDB 12.3 LTS (Docker)

Conteneur MariaDB fournissant la base `supervision` (relevés de ressources) du projet 14.

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

---

## Cycle de vie du conteneur

> Commandes à lancer depuis le dossier `docker/`.

```bash
docker compose up -d --wait      # 1) démarrer
docker compose stop              # 2) arrêter (reprise : docker compose start)
docker compose down              # 3) supprimer le conteneur (données conservées)
docker compose down -v           # 4) supprimer conteneur + données
docker compose down -v --rmi all # 5) tout supprimer, y compris l'image mariadb:12.3
docker rmi mariadb:12.3          #    (ou suppression manuelle de l'image)
```

| Objectif | Commande | Conteneur | Données | Image |
|---|---|:---:|:---:|:---:|
| Démarrer | `docker compose up -d` | créé | — | — |
| Pause | `docker compose stop` | gardé | gardées | gardée |
| Supprimer le conteneur | `docker compose down` | **supprimé** | gardées | gardée |
| + données | `docker compose down -v` | **supprimé** | **supprimées** | gardée |
| Tout (+ image) | `docker compose down -v --rmi all` | **supprimé** | **supprimées** | **supprimée** |

---

## Connexion

| Paramètre | Valeur |
|---|---|
| Hôte / Port | `127.0.0.1` / **`3319`** |
| Base | `supervision` |
| Compte applicatif | `supervision_app` / `supervision_pwd` |
| Administrateur | `root` / `root_demo_pwd` |

Valeurs lues par `src/NDX.Interop.UI/App.config`. Mots de passe de **démonstration**.

---

## Schéma

| Table | Rôle |
|---|---|
| `echantillon` | relevés : objets GDI, objets USER, inactivité (ms), horodatage |

La table démarre **vide** : les relevés sont produits par l'application (page « Supervision »).

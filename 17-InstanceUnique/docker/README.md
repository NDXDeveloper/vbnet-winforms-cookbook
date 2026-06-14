# Base de démonstration `commandes` — MariaDB 12.3 LTS (Docker)

Conteneur MariaDB fournissant la base `commandes` (journal IPC) du projet 17.

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
| Hôte / Port | `127.0.0.1` / **`3322`** |
| Base | `commandes` |
| Compte applicatif | `commandes_app` / `commandes_pwd` |
| Administrateur | `root` / `root_demo_pwd` |

Valeurs lues par `src/NDX.InstanceUnique.UI/App.config`. Mots de passe de **démonstration**.

---

## Schéma

| Table | Rôle |
|---|---|
| `commande_recue` | journal : source, arguments transmis, date de réception |

La table démarre **vide** : les commandes sont produites par l'application (page « Instance & messages »).

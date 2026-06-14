# Base de démonstration `mediatheque` — MariaDB 12.3 LTS (Docker)

Conteneur MariaDB fournissant la base `mediatheque` (vignettes) du projet 15.

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
| Hôte / Port | `127.0.0.1` / **`3320`** |
| Base | `mediatheque` |
| Compte applicatif | `mediatheque_app` / `mediatheque_pwd` |
| Administrateur | `root` / `root_demo_pwd` |

Valeurs lues par `src/NDX.Images.UI/App.config`. Mots de passe de **démonstration**.

---

## Schéma

| Table | Rôle |
|---|---|
| `image` | médiathèque : nom, dimensions de l'original, vignette PNG (binaire), date |

La table démarre **vide** : les vignettes sont produites par l'application (page « Médiathèque »).

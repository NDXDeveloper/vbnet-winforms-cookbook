# Base de démonstration `entrepot` — MariaDB 12.3 LTS (Docker)

Conteneur MariaDB fournissant la base `entrepot` (ventes à exporter) du projet 04.

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

---

## Cycle de vie du conteneur

> Commandes à lancer depuis le dossier `docker/`.

```bash
docker compose up -d --wait      # 1) démarrer (attendre que la base soit prête)
docker compose stop              # 2) arrêter sans supprimer (reprise : docker compose start)
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

Vérifier : `docker compose ps` · `docker images mariadb`.

---

## Connexion

| Paramètre | Valeur |
|---|---|
| Hôte / Port | `127.0.0.1` / **`3310`** |
| Base | `entrepot` |
| Compte applicatif | `entrepot_app` / `entrepot_pwd` |
| Administrateur | `root` / `root_demo_pwd` |

Valeurs lues par `src/NDX.Export.UI/App.config`. Mots de passe de **démonstration**.

---

## Schéma

| Table | Rôle |
|---|---|
| `vente` | une table plate (date, produit, catégorie, quantité, montant) — jeu de données à exporter |

20 ventes de démonstration sont amorcées. La galerie propose deux sources :
la liste détaillée et une synthèse par catégorie (agrégats `SUM`/`COUNT`).

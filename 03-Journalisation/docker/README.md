# Base de démonstration `journal` — MariaDB 12.3 LTS (Docker)

Conteneur MariaDB fournissant la base `journal` (entrées de journal) du projet 03.

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

---

## Cycle de vie du conteneur

> Commandes à lancer depuis le dossier `docker/`.

```bash
# 1) DÉMARRER (et attendre que la base soit prête)
docker compose up -d --wait

# 2) ARRÊTER sans rien supprimer (conteneur + données conservés)
docker compose stop          # reprise : docker compose start

# 3) SUPPRIMER LE CONTENEUR (données conservées dans le volume)
docker compose down

# 4) SUPPRIMER LE CONTENEUR + LES DONNÉES
docker compose down -v

# 5) SUPPRIMER AUSSI L'IMAGE TÉLÉCHARGÉE (mariadb:12.3)
docker compose down -v --rmi all     # tout supprimer
docker rmi mariadb:12.3               # ou suppression manuelle de l'image
```

| Objectif | Commande | Conteneur | Données | Image |
|---|---|:---:|:---:|:---:|
| Démarrer | `docker compose up -d` | créé | — | — |
| Pause | `docker compose stop` | gardé | gardées | gardée |
| Supprimer le conteneur | `docker compose down` | **supprimé** | gardées | gardée |
| + données | `docker compose down -v` | **supprimé** | **supprimées** | gardée |
| Tout (+ image) | `docker compose down -v --rmi all` | **supprimé** | **supprimées** | **supprimée** |

Vérifier : `docker compose ps` · `docker volume ls` · `docker images mariadb`.

---

## Connexion

| Paramètre | Valeur |
|---|---|
| Hôte / Port | `127.0.0.1` / **`3309`** |
| Base | `journal` |
| Compte applicatif | `journal_app` / `journal_pwd` |
| Administrateur | `root` / `root_demo_pwd` |

Valeurs lues par `src/NDX.Journalisation.UI/App.config` (clés `Bdd.*`).
Mots de passe de **démonstration** uniquement.

---

## Schéma

| Table | Rôle |
|---|---|
| `entree_journal` | une entrée de journal : horodatage, niveau (numérique + libellé), catégorie, message, exception, machine |

Le niveau numérique permet de filtrer `WHERE niveau >= seuil`. Quelques entrées
de démonstration sont amorcées ; l'application en ajoute via le **puits base**.

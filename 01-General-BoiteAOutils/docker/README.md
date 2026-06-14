# Base de démonstration `etabli` — MariaDB 12.3 LTS (Docker)

Conteneur MariaDB fournissant la base `etabli` utilisée par le projet 01
(« Boîte à outils — techniques de programmation .NET »).

---

## Cycle de vie du conteneur

> Toutes ces commandes se lancent depuis le dossier `docker/` (là où se trouve
> `docker-compose.yml`).

```bash
# 1) DÉMARRER (et attendre que la base soit prête)
docker compose up -d --wait

# 2) ARRÊTER sans rien supprimer (conteneur + données conservés ; on pourra le relancer)
docker compose stop
docker compose start          # le redémarrer plus tard

# 3) SUPPRIMER LE CONTENEUR (et le réseau) — les données du volume sont CONSERVÉES
docker compose down

# 4) SUPPRIMER LE CONTENEUR + LES DONNÉES (volume) — repart de zéro au prochain up
docker compose down -v

# 5) RÉINITIALISER (recharge schéma + données de démo)
docker compose down -v && docker compose up -d --wait

# 6) SUPPRIMER AUSSI L'IMAGE TÉLÉCHARGÉE (mariadb:12.3)
docker compose down --rmi all          # supprime conteneur + image (garde le volume)
docker compose down -v --rmi all       # TOUT supprimer : conteneur + volume + image
#   (équivalent manuel de la suppression d'image, conteneur déjà arrêté :)
docker rmi mariadb:12.3
```

### Récapitulatif

| Objectif | Commande | Conteneur | Données (volume) | Image |
|---|---|:---:|:---:|:---:|
| Démarrer | `docker compose up -d` | créé | — | — |
| Mettre en pause | `docker compose stop` | gardé | gardées | gardée |
| Supprimer le conteneur | `docker compose down` | **supprimé** | gardées | gardée |
| Supprimer conteneur + données | `docker compose down -v` | **supprimé** | **supprimées** | gardée |
| Tout supprimer (+ image) | `docker compose down -v --rmi all` | **supprimé** | **supprimées** | **supprimée** |

### Vérifier l'état

```bash
docker compose ps          # le conteneur tourne-t-il ? (statut, santé)
docker volume ls           # le volume etabli-data existe-t-il encore ?
docker images mariadb      # l'image mariadb:12.3 est-elle encore présente ?
```

> Le fichier `.env` est facultatif : `docker-compose.yml` fournit des valeurs par
> défaut. Pour personnaliser, copiez `.env.example` en `.env`.

---

## Connexion

| Paramètre | Valeur |
|---|---|
| Hôte | `127.0.0.1` |
| Port | `3307` *(côté hôte ; 3306 dans le conteneur)* |
| Base | `etabli` |
| Utilisateur applicatif | `etabli_app` / `etabli_pwd` |
| Administrateur | `root` / `root_demo_pwd` |

Ces valeurs correspondent à celles lues par l'application dans
`src/BoiteAOutils.UI/App.config` (clés `Bdd.*`).

> ⚠️ Mots de passe de **démonstration** uniquement.

---

## Initialisation automatique

Au **premier** démarrage (volume vide), l'image MariaDB :

1. crée la base `etabli` et le compte `etabli_app`
   (via les variables d'environnement du `docker-compose.yml`) ;
2. exécute, dans l'ordre alphabétique, les scripts montés dans
   `/docker-entrypoint-initdb.d` :
   - `init/01-schema.sql` — création des tables ;
   - `init/02-donnees.sql` — jeu de données de démonstration.

Le compte applicatif est créé avec le plugin `mysql_native_password`, ce qui
garantit la compatibilité avec le connecteur **`MySql.Data`** côté .NET.

---

## Schéma (rappel)

```
secteur 1───* client 1───* commande 1───* ligne_commande *───1 article 1───1 fiche_article
                                                                              journal_erreur (indépendante)
```

| Table | Rôle |
|---|---|
| `secteur` | zones commerciales (référence) |
| `client` | clients (datés, actif/inactif) |
| `commande` | en-têtes de commande (datées) |
| `article` | catalogue (libellé, prix réf.) |
| `fiche_article` | caractéristiques (→ ratios calculés) |
| `ligne_commande` | détail (quantité × prix) |
| `journal_erreur` | journal applicatif des exceptions |

---

## Commandes utiles

```bash
# Ouvrir un client SQL dans le conteneur
docker exec -it etabli-mariadb mariadb -uetabli_app -petabli_pwd etabli

# Vérifier rapidement les données
docker exec -it etabli-mariadb mariadb -uetabli_app -petabli_pwd etabli \
  -e "SELECT COUNT(*) AS clients FROM client; SELECT COUNT(*) AS lignes FROM ligne_commande;"

# Journaux du conteneur
docker compose logs -f mariadb

# Sauvegarde
docker exec etabli-mariadb mariadb-dump -uroot -proot_demo_pwd etabli > sauvegarde.sql
```

---

## Dépannage

| Symptôme | Cause probable / solution |
|---|---|
| `port is already allocated` | Le port hôte 3307 est pris → changez `MARIADB_HOST_PORT` dans `.env`, et `Bdd.Port` dans `App.config`. |
| Les scripts d'init ne s'exécutent pas | Ils ne tournent **qu'avec un volume vide**. Faites `docker compose down -v` puis relancez. |
| `Access denied for user` | Vous avez modifié les identifiants après le 1er démarrage. Réinitialisez le volume (`down -v`) ou ajustez le compte manuellement. |
| L'application ne se connecte pas | Vérifiez que le conteneur est *healthy* (`docker compose ps`) et que `App.config` pointe bien sur `127.0.0.1:3307`. |
| Tag `mariadb:12.3` indisponible | Utilisez une autre étiquette LTS (ex. `mariadb:11.4`) dans `docker-compose.yml` ; le schéma reste compatible. |

---

## Pourquoi MariaDB et non MySQL ?

MariaDB est compatible au niveau du **protocole** avec le connecteur `MySql.Data` :
le code d'accès aux données est donc **identique**. Les techniques d'accès ne sont
donc pas liées à un moteur précis. Pour une compatibilité maximale en production avec
MariaDB, on pourrait basculer sur le connecteur open-source `MySqlConnector` (même
API, espace de noms différent) — non requis ici.

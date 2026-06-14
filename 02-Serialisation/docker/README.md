# Base de démonstration `coffre` — MariaDB 12.3 LTS (Docker)

Conteneur MariaDB fournissant la base `coffre` (coffre de documents sérialisés)
utilisée par le projet 02 — Sérialisation.

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

---

## Cycle de vie du conteneur

> Toutes ces commandes se lancent depuis le dossier `docker/` (là où se trouve
> `docker-compose.yml`).

```bash
# 1) DÉMARRER (et attendre que la base soit prête)
docker compose up -d --wait

# 2) ARRÊTER sans rien supprimer (conteneur + données conservés)
docker compose stop
docker compose start          # le redémarrer plus tard

# 3) SUPPRIMER LE CONTENEUR (et le réseau) — les données du volume sont CONSERVÉES
docker compose down

# 4) SUPPRIMER LE CONTENEUR + LES DONNÉES (volume) — repart de zéro au prochain up
docker compose down -v

# 5) RÉINITIALISER (recharge schéma + données de démo)
docker compose down -v && docker compose up -d --wait

# 6) SUPPRIMER AUSSI L'IMAGE TÉLÉCHARGÉE (mariadb:12.3)
docker compose down --rmi all          # conteneur + image (garde le volume)
docker compose down -v --rmi all       # TOUT supprimer : conteneur + volume + image
#   (équivalent manuel, conteneur déjà arrêté :)
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
docker volume ls           # le volume coffre-data existe-t-il encore ?
docker images mariadb      # l'image mariadb:12.3 est-elle encore présente ?
```

> Le fichier `.env` est facultatif : `docker-compose.yml` fournit des valeurs par
> défaut. Pour personnaliser, copiez `.env.example` en `.env`.

---

## Connexion

| Paramètre | Valeur |
|---|---|
| Hôte | `127.0.0.1` |
| Port | `3308` *(côté hôte ; 3306 dans le conteneur)* |
| Base | `coffre` |
| Utilisateur applicatif | `coffre_app` / `coffre_pwd` |
| Administrateur | `root` / `root_demo_pwd` |

Ces valeurs correspondent à celles lues par l'application dans
`src/NDX.Serialisation.UI/App.config` (clés `Bdd.*`).

> ⚠️ Mots de passe de **démonstration** uniquement.

---

## Initialisation automatique

Au **premier** démarrage (volume vide), l'image MariaDB :

1. crée la base `coffre` et le compte `coffre_app` (variables d'environnement) ;
2. exécute, dans l'ordre, les scripts montés dans `/docker-entrypoint-initdb.d` :
   - `init/01-schema.sql` — tables `categorie` et `document` ;
   - `init/02-donnees.sql` — catégories de démonstration.

Le compte applicatif est créé avec le plugin `mysql_native_password`, compatible
avec le connecteur **MySql.Data** côté .NET.

---

## Schéma

```
categorie 1───* document
```

| Table | Rôle |
|---|---|
| `categorie` | classement logique des documents |
| `document` | objet sérialisé (LONGBLOB) + métadonnées (type, format, taille, empreinte SHA-256) |

Les **documents** ne sont pas amorcés : ils sont créés par l'application pendant
la démonstration de persistance (sérialiser → enregistrer → relire → désérialiser).

---

## Commandes utiles

```bash
# Ouvrir un client SQL dans le conteneur
docker exec -it coffre-mariadb mariadb -ucoffre_app -pcoffre_pwd coffre

# Lister les documents enregistrés
docker exec -it coffre-mariadb mariadb -ucoffre_app -pcoffre_pwd coffre \
  -e "SELECT id, libelle, format, taille_octets, cree_le FROM document ORDER BY id;"
```

---

## Dépannage

| Symptôme | Solution |
|---|---|
| `port is already allocated` | Le port 3308 est pris → changez `MARIADB_HOST_PORT` dans `.env` et `Bdd.Port` dans `App.config`. |
| Les scripts d'init ne s'exécutent pas | Ils ne tournent **qu'avec un volume vide** : `docker compose down -v` puis relancez. |
| L'application ne se connecte pas | Vérifiez que le conteneur est *healthy* (`docker compose ps`) et que `App.config` pointe sur `127.0.0.1:3308`. |

---

## Pourquoi MariaDB ?

MariaDB est compatible au niveau du **protocole** avec le connecteur `MySql.Data`
utilisé côté .NET : le code d'accès aux données reste standard. Pour une
compatibilité maximale en production, on peut aussi employer le connecteur
open-source `MySqlConnector` (même API, espace de noms différent).

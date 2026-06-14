# Annuaire de démonstration `dc=exemple,dc=test` — OpenLDAP (Docker)

Conteneur OpenLDAP fournissant l'annuaire du projet 10 (authentification).

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

---

## Cycle de vie du conteneur

> Commandes à lancer depuis le dossier `docker/`.

```bash
docker compose up -d --wait      # 1) démarrer
docker compose stop              # 2) arrêter (reprise : docker compose start)
docker compose down              # 3) supprimer le conteneur (données conservées)
docker compose down -v           # 4) supprimer conteneur + données
docker compose down -v --rmi all # 5) tout supprimer, y compris l'image osixia/openldap
docker rmi osixia/openldap:1.5.0 #    (ou suppression manuelle de l'image)
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
| Hôte / Port | `127.0.0.1` / **`389`** (LDAP en clair) |
| Base (DN racine) | `dc=exemple,dc=test` |
| Administrateur | `cn=admin,dc=exemple,dc=test` / `admin_pwd` |
| Comptes | `alice` / `alice_pwd`, `bob` / `bob_pwd` (sous `ou=users`) |

Valeurs lues par `src/NDX.Annuaire.UI/App.config`. Mots de passe de **démonstration**.

---

## Comptes amorcés

Le fichier [`bootstrap/01-utilisateurs.ldif`](bootstrap/01-utilisateurs.ldif) crée
l'unité `ou=users` et deux comptes `inetOrgPerson` :

| uid | cn | mail |
|---|---|---|
| `alice` | Alice Martin | alice@exemple.test |
| `bob` | Bob Durand | bob@exemple.test |

> Note ACL : par défaut, un compte ne peut lire que **sa propre** entrée. Lister
> tous les comptes nécessite donc le compte **administrateur** (c'est ce que fait
> la page « Annuaire »).

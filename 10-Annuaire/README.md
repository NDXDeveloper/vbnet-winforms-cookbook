# Authentification LDAP — « bind », recherche et protection contre l'injection

> Projet pédagogique : authentifier des comptes sur un annuaire **LDAP** en VB.NET
> (.NET Framework 4.8.1) via `System.DirectoryServices.Protocols` — authentification
> par « bind », lecture d'attributs, recherche, et échappement des filtres contre
> l'injection LDAP — avec galerie WinForms et annuaire **OpenLDAP** (Docker).
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier `Core` |
|---|---|---|
| **Authentification** | `LdapConnection.Bind` (le serveur valide DN + mot de passe) | `Annuaire/ServiceAnnuaire.vb` |
| **Lecture d'attributs** | `SearchRequest` scope *Base* sur sa propre entrée (lecture « self ») | `Annuaire/ServiceAnnuaire.vb` |
| **Recherche** | filtre `uid`/`cn`, bind administrateur | `Annuaire/ServiceAnnuaire.vb` |
| **Sécurité** | échappement RFC 4515 contre l'injection LDAP | `Annuaire/EchappeurLdap.vb` |
| **Configuration** | hôte, port, base DN, gabarit de DN (`App.config`) | `Annuaire/ConfigAnnuaire.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # annuaire sur 127.0.0.1:389
```

1. Ouvrir `src/NDX.Annuaire.sln` ; démarrer **`NDX.Annuaire.UI`** ; **F5**.
2. Page *Connexion* : essayer `alice` / `alice_pwd` (puis un mauvais mot de passe).
3. Page *Annuaire* : rechercher des comptes (filtre `uid`/`cn`).

```bash
cd src && dotnet test NDX.Annuaire.sln    # 12 tests (9 logique + 3 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Annuaire.Core/   → service LDAP (auth, recherche) + échappeur + config
├── NDX.Annuaire.UI/     → galerie WinForms (connexion, annuaire)
└── NDX.Annuaire.Tests/  → tests MSTest
```

L'authentification repose sur le **bind** : on ne stocke et ne compare aucune
empreinte de mot de passe côté application — c'est l'annuaire qui tranche. La
seule logique « hors-ligne » (l'échappement de filtre) est **isolée et testée**.

---

## 5. Deux points de sécurité essentiels

- **Bind = authentification de référence.** On tente d'ouvrir une session avec le
  DN du compte et son mot de passe : si le serveur accepte, l'identité est prouvée.
- **Échapper les filtres.** Insérer une saisie brute dans un filtre LDAP ouvre la
  porte à l'**injection** (l'équivalent de l'injection SQL). On échappe `* ( ) \`
  selon la RFC 4515. Détails en
  [`docs/02-securite-bind-et-injection.md`](docs/02-securite-bind-et-injection.md).

---

## 6. Annuaire de démonstration

OpenLDAP, base `dc=exemple,dc=test`, comptes `alice` et `bob` sous `ou=users`.
Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **12/12** (dont 3 d'intégration contre **OpenLDAP**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

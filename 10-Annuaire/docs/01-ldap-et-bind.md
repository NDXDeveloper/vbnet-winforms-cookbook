# Fiche 01 — LDAP, DN et authentification par « bind »

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Un annuaire est un arbre

LDAP organise les entrées en **arbre**. Chaque entrée est identifiée par son
**DN** (Distinguished Name), chemin unique de la feuille à la racine :

```
dc=exemple,dc=test                 ← racine (base DN)
└── ou=users                       ← unité d'organisation
    ├── uid=alice                  → DN : uid=alice,ou=users,dc=exemple,dc=test
    └── uid=bob                    → DN : uid=bob,ou=users,dc=exemple,dc=test
```

Une entrée porte des **attributs** : `uid`, `cn` (nom complet), `mail`,
`userPassword`…

## S'authentifier = « bind »

Le **bind** est l'opération d'ouverture de session. On fournit un DN et un mot de
passe ; **le serveur** vérifie. C'est la bonne pratique : l'application ne stocke,
ne hache et ne compare **aucun** mot de passe.

```vbnet
Dim identite As New LdapDirectoryIdentifier(hote, port)
Using cn As New LdapConnection(identite)
    cn.SessionOptions.ProtocolVersion = 3
    cn.AuthType = AuthType.Basic
    cn.Bind(New NetworkCredential(dn, motDePasse))   ' échec => LdapException (code 49)
    ' … ici, l'identité est prouvée
End Using
```

Le DN du compte est construit à partir d'un **gabarit** configurable :

```
uid={0},ou=users,dc=exemple,dc=test     ({0} = identifiant saisi)
```

## Lire ses propres attributs

Après le bind, on lit l'entrée du compte par une recherche en portée **Base** sur
son propre DN :

```vbnet
Dim requete As New SearchRequest(dn, "(objectClass=*)", SearchScope.Base, "uid", "cn", "mail")
Dim reponse = CType(cn.SendRequest(requete), SearchResponse)
```

> Pourquoi portée *Base* sur le DN, et pas une recherche dans `ou=users` ? Parce
> qu'un compte ordinaire n'a, par défaut, le droit de lire que **sa propre**
> entrée (`by self read`). Lister **tous** les comptes nécessite un compte
> **administrateur** (voir page « Annuaire »).

## À retenir

- LDAP = arbre d'entrées identifiées par leur **DN**.
- **Bind** = authentification déléguée au serveur (aucun mot de passe géré par l'app).
- Échec de bind ⇒ `LdapException` (code 49 = identifiants invalides).
- Les **droits de lecture** dépendent de l'identité connectée (self vs admin).

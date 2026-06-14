# Recueil VB.NET — 26 techniques du développement desktop Windows

![VB.NET](https://img.shields.io/badge/langage-VB.NET-512BD4)  
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-512BD4)  
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2022-5C2D91)  
![MariaDB](https://img.shields.io/badge/MariaDB-12.3%20LTS-003545)  
![Tests](https://img.shields.io/badge/tests-286%20MSTest-2ea44f)  
![Licence](https://img.shields.io/badge/licence-MIT-green)

Recueil pédagogique de **26 projets VB.NET autonomes** (.NET Framework 4.8.1, WinForms) —
chacun illustre **une technique** du développement desktop Windows : sérialisation,
journalisation, async/await, annuaire LDAP, PDF, impression, graphiques… Base de
démonstration **MariaDB** via **Docker**, tests **MSTest**, le tout **commenté en français**.

> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE) ; chaque projet inclut également le sien).

---

## Les 26 projets

Sommaire détaillé (technique, base, port, nombre de tests) : **[`SOMMAIRE.md`](SOMMAIRE.md)**.

### Fondations & données
- [`01` Boîte à outils](./01-General-BoiteAOutils/) — chaînes, dates, conversions sûres
- [`02` Sérialisation](./02-Serialisation/) — XML, contrat, binaire sûr, JSON
- [`05` Données](./05-Donnees/) — Repository / Unit of Work / mapping par réflexion
- [`06` Compression](./06-Compression/) — GZip/Deflate, *compress-then-store*
- [`18` Réflexion](./18-Reflexion/) — inspection de `Type`, copie de propriétés, purge d'événements

### Journalisation, cycle de vie & déploiement
- [`03` Journalisation](./03-Journalisation/) — journal thread-safe + puits (fichier avec rotation, base…)
- [`07` Mise à jour](./07-MiseAJour/) — version sémantique, manifeste, service de mise à jour
- [`17` Instance unique](./17-InstanceUnique/) — mono-instance (Mutex) + IPC par tube nommé
- [`25` Démarrage](./25-Demarrage/) — séquence d'étapes, gestion d'exceptions globale, splash

### Documents & impression
- [`04` Export](./04-Export/) — CSV (RFC 4180), Excel `.xlsx` (OpenXML), PDF
- [`09` PDF](./09-Pdf/) — moteur de composition PDF 100 % VB.NET (sans dépendance native)
- [`16` Impression](./16-Impression/) — `PrintDocument` / aperçu + paginateur pur
- [`19` Office](./19-Office/) — lecture/écriture `.xlsx` **sans Excel** + pilotage COM optionnel

### Interface & graphisme WinForms
- [`08` Contrôles](./08-Controles/) — *owner-draw* : bouton bascule, visionneuse zoom/pan
- [`15` Images](./15-Images/) — GDI+ `ColorMatrix` (gris/négatif/luminosité) + vignettes
- [`20` Form de base & thèmes](./20-FormBase/) — héritage visuel de formulaires + moteur de thèmes
- [`21` Arborescence](./21-Arborescence/) — `TreeView` + données hiérarchiques (CTE récursive)
- [`22` Dessin](./22-Dessin/) — éditeur de formes vectorielles GDI+ (*hit-testing* pur)
- [`23` Éditeur](./23-Editeur/) — éditeur RTF (`RichTextBox` + barre de format)
- [`24` Contrôles II](./24-ControlesII/) — bouton à états (`IButtonControl`), onglets peints, grille
- [`26` Graphiques](./26-Graphiques/) — contrôle de tracé (barres / courbe / points)

### Système, interopérabilité & concurrence
- [`10` Annuaire](./10-Annuaire/) — authentification **LDAP** + échappement de filtre (RFC 4515)
- [`11` Raccourcis](./11-Raccourcis/) — raccourcis clavier multi-touches (*chords*)
- [`12` Shell](./12-Shell/) — raccourcis Windows `.lnk` / `.url`
- [`13` Asynchrone](./13-Asynchrone/) — `async`/`await`, `Task`, `IProgress`, `CancellationToken`
- [`14` Interop](./14-Interop/) — P/Invoke Win32 (inactivité, ressources GDI/USER, fenêtre)

---

## Prérequis communs

- **Visual Studio Community 2022**
- **.NET Framework 4.8.1** (*Developer Pack*)
- **Docker Desktop** (pour les bases de démonstration)

## Anatomie d'un projet

```
NN-Theme/
├── src/        → solution VS : Core (la technique) + UI (galerie WinForms) + Tests (MSTest)
├── docker/     → docker-compose.yml + .env.example + init/ (schéma + données) + README
├── docs/       → fiches pédagogiques détaillées
└── README.md   → présentation du projet
```

## Conventions communes

- Projets **SDK-style** ciblant **`net481`**, **`Option Strict On`**.
- Architecture **hybride** : bibliothèque **Core** (testée) + galerie **WinForms** + tests **MSTest**.
- La logique sensible est **isolée en code pur** → testée exhaustivement ; les tests
  d'intégration base passent en `Assert.Inconclusive` quand la base est absente.
- Chaque base Docker a un **nom, un port hôte et un volume distincts** (aucune collision entre projets).
- Code et documentation **en français** ; en-tête **MIT** dans chaque fichier source.

## Démarrage rapide (pour un projet donné)

```bash
cd NN-Theme/docker && docker compose up -d --wait   # démarre la base de démonstration
```

Ouvrir ensuite `NN-Theme/src/*.sln` dans Visual Studio, démarrer le projet `*.UI`, puis **F5**.
Pour les tests : `cd NN-Theme/src && dotnet test`.

> ⚠️ L'annuaire (projet 10) utilise **OpenLDAP** (port 389) au lieu de MariaDB.

## Licence

L'ensemble du dépôt est publié sous licence **MIT** (voir [`LICENSE`](LICENSE)) — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;. Chaque projet inclut en outre sa propre copie de la licence.

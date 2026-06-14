# Sommaire — Recueil VB.NET (26 projets)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Chaque projet est **autonome** (`src/` + `docker/` + `docs/` + `README.md`). Les bases de
démonstration sont des conteneurs **MariaDB 12.3 LTS** (Docker), aux ports hôte **3307 → 3331** —
exception : le projet 10 utilise **OpenLDAP** (port 389).

| # | Projet | Technique illustrée | Base (Docker) | Port | Tests |
|--:|--------|---------------------|---------------|:----:|:-----:|
| 01 | [Boîte à outils](./01-General-BoiteAOutils/) | Utilitaires généraux : chaînes, dates, conversions sûres | `etabli` | 3307 | 20 |
| 02 | [Sérialisation](./02-Serialisation/) | XML, contrat XML, binaire sûr, JSON + stockage isolé (BLOB + SHA-256) | `coffre` | 3308 | 11 |
| 03 | [Journalisation](./03-Journalisation/) | Journal thread-safe + puits mémoire/console/fichier (rotation)/base | `journal` | 3309 | 7 |
| 04 | [Export](./04-Export/) | Export CSV (RFC 4180), Excel `.xlsx` (OpenXML), PDF | `entrepot` | 3310 | 7 |
| 05 | [Données](./05-Donnees/) | Repository, Unit of Work, mapping O/R par réflexion, *retry* | `boutique` | 3311 | 5 |
| 06 | [Compression](./06-Compression/) | GZip/Deflate + *compress-then-store* (LONGBLOB + tailles + SHA-256) | `archive` | 3312 | 12 |
| 07 | [Mise à jour](./07-MiseAJour/) | Version sémantique (`IComparable`), manifeste, service de mise à jour | `deploiement` | 3313 | 23 |
| 08 | [Contrôles](./08-Controles/) | Contrôles *owner-draw* : bouton bascule, visionneuse zoom/pan | `preferences` | 3314 | 13 |
| 09 | [PDF](./09-Pdf/) | Moteur PDF 100 % VB.NET (objets + xref, polices base-14, *word-wrap*) | `bibliotheque` | 3315 | 16 |
| 10 | [Annuaire](./10-Annuaire/) | Authentification **LDAP** + échappement de filtre (RFC 4515) | **OpenLDAP** | 389 | 12 |
| 11 | [Raccourcis](./11-Raccourcis/) | Raccourcis clavier multi-touches (*chords*), machine à états | `raccourcis` | 3316 | 26 |
| 12 | [Shell](./12-Shell/) | Raccourcis Windows `.lnk` (WScript.Shell) / `.url` (INI) | `catalogue_liens` | 3317 | 10 |
| 13 | [Asynchrone](./13-Asynchrone/) | `async`/`await`, `Task`, `IProgress`, `CancellationToken` | `traitements` | 3318 | 12 |
| 14 | [Interop](./14-Interop/) | P/Invoke Win32 : inactivité, ressources GDI/USER, fenêtre topmost | `supervision` | 3319 | 7 |
| 15 | [Images](./15-Images/) | GDI+ `ColorMatrix` (gris/négatif/luminosité) + vignettes | `mediatheque` | 3320 | 8 |
| 16 | [Impression](./16-Impression/) | `PrintDocument` / aperçu avant impression + paginateur pur | `impressions` | 3321 | 9 |
| 17 | [Instance unique](./17-InstanceUnique/) | Mono-instance (Mutex nommé) + IPC par tube nommé | `commandes` | 3322 | 9 |
| 18 | [Réflexion](./18-Reflexion/) | Inspection de `Type`, copie de propriétés, purge d'événements | `metadonnees` | 3323 | 10 |
| 19 | [Office](./19-Office/) | Lecture/écriture `.xlsx` **sans Excel** (OpenXML) + pilotage COM optionnel | `classeur` | 3324 | 19 |
| 20 | [Form de base & thèmes](./20-FormBase/) | Héritage visuel de formulaires + moteur de thèmes (hex ↔ `Color`) | `themes` | 3325 | 10 |
| 21 | [Arborescence](./21-Arborescence/) | `TreeView` + données hiérarchiques (CTE récursive), plat → arbre | `arborescence` | 3326 | 5 |
| 22 | [Dessin](./22-Dessin/) | Éditeur de formes vectorielles GDI+ (*hit-testing* pur) | `atelier` | 3327 | 7 |
| 23 | [Éditeur](./23-Editeur/) | Éditeur RTF (`RichTextBox` + barre de format), statistiques de texte | `redaction` | 3328 | 5 |
| 24 | [Contrôles II](./24-ControlesII/) | Bouton à états (`IButtonControl`), onglets peints, grille personnalisée | `inventaire` | 3329 | 7 |
| 25 | [Démarrage](./25-Demarrage/) | Séquence d'étapes, gestion d'exceptions globale, écran de démarrage | `demarrage` | 3330 | 7 |
| 26 | [Graphiques](./26-Graphiques/) | Contrôle de tracé *owner-draw* (barres / courbe / points), mise à l'échelle | `mesures` | 3331 | 9 |

**Total : 286 tests MSTest** — dont **1 *Inconclusive*** volontaire (interop Excel COM, projet 19, faute d'Office installé).

---

## Comment lire un projet

1. Le **`README.md`** du projet en donne l'intention et le démarrage rapide.
2. Le dossier **`docs/`** contient les fiches pédagogiques détaillées.
3. Le cœur de la technique est dans le projet **`Core`** (`src/`), isolé et testé.
4. La galerie **`UI`** (WinForms) montre la technique en situation.

## Arrêter / démarrer toutes les bases

```bash
# Démarrer la base d'un projet :
cd NN-Theme/docker && docker compose up -d --wait

# Tout arrêter (sans perdre les données) :
docker stop $(docker ps -q)
```

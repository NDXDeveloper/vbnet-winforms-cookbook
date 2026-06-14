# Éditeur de formes — GDI+ interactif, hit-testing et sérialisation

> Projet pédagogique : un petit éditeur de **dessin vectoriel** en VB.NET (.NET
> Framework 4.8.1) — formes (rectangle, ellipse, ligne), **test de survol**
> (hit-testing), création et déplacement à la souris sur un canevas double tampon,
> et sérialisation de la scène — avec galerie WinForms et MariaDB.
>
> Copyright (c) 2026 **Nicolas DEOUX** &lt;NDXDev@gmail.com&gt; — Licence **MIT** (voir [`LICENSE`](LICENSE)).

---

## 1. Ce que démontre ce projet

| Thème | Techniques | Fichier |
|---|---|---|
| **Hit-testing** | « ce point est-il sur la forme ? » (rect / ellipse / segment) | `Dessin/Forme.vb` |
| **Scène** | collection ordonnée + sélection de la forme la plus haute | `Dessin/Scene.vb` |
| **Canevas interactif** | double tampon, création au glisser, déplacement | UI `Canevas.vb` |
| **Sérialisation** | enregistrer / recharger la scène (transaction) | `Persistance/DepotForme.vb` |

Fiches détaillées : [`docs/`](docs/).

---

## 2. Prérequis

**Visual Studio Community 2022**, **.NET Framework 4.8.1**, **Docker Desktop**.

---

## 3. Démarrage rapide

```bash
cd docker && docker compose up -d --wait    # base "atelier" sur 127.0.0.1:3327
```

1. Ouvrir `src/NDX.Dessin.sln` ; démarrer **`NDX.Dessin.UI`** ; **F5**.
2. Page *Canevas* : choisir une forme + une couleur, **glisser** pour tracer ;
   cocher *Déplacer* puis saisir une forme pour la bouger ; **Enregistrer** / **Charger**.
3. Page *Formes* : voir la scène enregistrée en base.

```bash
cd src && dotnet test NDX.Dessin.sln    # 7 tests (6 logique + 1 intégration)
```

---

## 4. Architecture

```
src/
├── NDX.Dessin.Core/   → formes (hit-testing) + scène + sérialisation
├── NDX.Dessin.UI/     → galerie WinForms + Canevas (double tampon)
└── NDX.Dessin.Tests/  → tests MSTest
```

Le **hit-testing** (et la sélection de la forme la plus haute) est de la pure
géométrie : on le teste sans interface. Le **Canevas** ne fait que router les
événements souris vers la `Scene` et redessiner.

---

## 5. L'idée clé : le hit-testing

Sélectionner une forme à la souris, c'est répondre « ce point est-il sur elle ? » :
appartenance à un rectangle, équation de l'ellipse, ou distance point↔segment pour
une ligne. La `Scene` parcourt ses formes **de la plus haute à la plus basse** pour
renvoyer celle qui est dessus. Détails en
[`docs/01-hit-testing.md`](docs/01-hit-testing.md) et
[`docs/02-canevas-double-tampon.md`](docs/02-canevas-double-tampon.md).

---

## 6. Base de démonstration (`atelier`)

Table `forme`. Démarre **vide**. Voir [`docker/README.md`](docker/README.md).

---

## 7. Vérifications effectuées

- `dotnet build` : **0 erreur, 0 avertissement**.
- `dotnet test` : **7/7** (dont 1 d'intégration contre **MariaDB 12.3**).
- Galerie WinForms : démarrage **sans plantage**.

---

## 8. Licence

**MIT** — Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt;.

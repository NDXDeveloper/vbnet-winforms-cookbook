# 06 — Images (GDI+)

> Traitement d'images GDI+ : nuances de gris, détection de format, génération.

Module : `Images/OutilsImages.vb`. Démonstration : page **Images** de la galerie.

---

## 1. Nuances de gris — `EnNuancesDeGris`

Technique : appliquer une **matrice de couleurs** (`ColorMatrix`) lors d'un
`Graphics.DrawImage`. Chaque canal de sortie reçoit la même combinaison pondérée
des canaux d'entrée, selon les coefficients de **luminance perceptuelle**
(0,30 R + 0,59 V + 0,11 B) :

```vbnet
Dim matrice As New ColorMatrix(New Single()() {
    New Single() {0.3F, 0.3F, 0.3F, 0, 0},
    New Single() {0.59F, 0.59F, 0.59F, 0, 0},
    New Single() {0.11F, 0.11F, 0.11F, 0, 0},
    New Single() {0, 0, 0, 1, 0},
    New Single() {0, 0, 0, 0, 1}})
attributs.SetColorMatrix(matrice)
g.DrawImage(original, rect, 0, 0, w, h, GraphicsUnit.Pixel, attributs)
```

C'est bien plus rapide qu'un parcours pixel par pixel : le travail est délégué à GDI+.

## 2. Image déjà en gris ? — `EstEnNuancesDeGris`

Compare les **octets** de l'image à ceux de sa version convertie en gris (égalité
binaire après encodage JPEG en mémoire). Si identiques, l'image était déjà grise.

---

## 3. Détection de format par signature — `DetecterExtension`

Technique des « **magic bytes** » : on lit les premiers octets et on les compare à
des signatures connues, **indépendamment du nom de fichier**.

| Format | Signature (hex) |
|---|---|
| PNG | `89-50-4E-47-0D-0A-1A-0A` |
| PDF | `25-50-44-46` (`%PDF`) |
| JPEG | `FF-D8-FF-E…` |
| DOC/XLS (OLE) | `D0-CF-11-E0-A1-B1-1A-E1` |
| DOCX/XLSX (ZIP) | `50-4B-03-04-14-00-06-00` |

Cas particuliers gérés : OLE composite (on départage `.doc`/`.xls` plus loin dans
le fichier) ; conteneur ZIP Office (`.docx`/`.xlsx` selon un indice textuel).

> Sécurité : valider le **type réel** d'un fichier par sa signature — et non par
> son extension — évite qu'un fichier renommé ne soit traité à tort.

---

## 4. Plan vide — `CreerPlanVide` / `CreerPlanVideBlanc`

Génère un `Bitmap` (cadre + fond, ou tout blanc) et le renvoie en **tableau
d'octets** (JPEG via `MemoryStream`) — pratique pour stockage en base ou transfert.
Dimensions par défaut : `LARGEUR_PLAN_VIDE` × `HAUTEUR_PLAN_VIDE` (paramétrables).

## 5. Encodeurs & sauvegarde

- `EncodeurPour(typeMime)` retrouve l'`ImageCodecInfo` d'un type MIME.
- `SauverEnPng` enregistre un bitmap au format PNG.

---

## Gestion des ressources

Tous les objets GDI+ (`Bitmap`, `Graphics`, `ImageAttributes`, `Pen`, `Brush`,
`MemoryStream`) sont créés dans des blocs **`Using`** : libération déterministe
des ressources non managées, point d'attention récurrent avec GDI+.

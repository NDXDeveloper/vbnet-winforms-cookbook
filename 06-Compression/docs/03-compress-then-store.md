# Fiche 03 — « Compress-then-store » : archiver compressé en base

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Idée

Plutôt que de stocker des données volumineuses telles quelles, on les **compresse
côté application** puis on enregistre le binaire compressé dans une colonne
`LONGBLOB`. On gagne de la place disque et de la bande passante, au prix d'un coût
CPU à l'écriture et à la lecture.

## Ce qu'on mémorise

| Colonne | Rôle |
|---|---|
| `contenu` | les octets **compressés** (`LONGBLOB`) |
| `algorithme` | `gzip` / `deflate` — indispensable pour décompresser |
| `taille_originale` | taille avant compression (mesure du gain) |
| `taille_compressee` | taille stockée |
| `empreinte_sha256` | SHA-256 **des données d'origine** : contrôle d'intégrité |

Le **gain de place** se calcule directement en SQL :

```sql
ROUND(100 * (1 - taille_compressee / NULLIF(taille_originale, 0)), 1) AS `Gain %`
```

## Pourquoi une empreinte SHA-256 ?

Elle est calculée sur les **données claires** avant compression. À la relecture,
on décompresse puis on peut recalculer l'empreinte : si elle correspond, on a la
**preuve** que l'aller-retour compression → stockage → décompression a restitué
des octets identiques. C'est une garantie d'intégrité indépendante du CRC interne
de GZIP.

## Le cycle complet

```vbnet
' Écriture : compresser, puis insérer le binaire + métadonnées
Dim compresse As Byte() = Compresseur.Compresser(donnees, algo)
INSERT ... VALUES (@nom, @algo, donnees.Length, compresse.Length, @compresse, sha256(donnees))

' Lecture : relire le binaire + l'algorithme, puis décompresser
Dim compresse = lecteur("contenu")
Dim algo      = lecteur("algorithme")
Return Compresseur.Decompresser(compresse, algo)   ' octets d'origine
```

## Paramètres et binaire

L'insertion passe par une **requête paramétrée** : le `LONGBLOB` est transmis via
un paramètre (`@contenu`) qui accepte directement un `Byte()`. Aucun encodage
texte (base64…) n'est nécessaire, et l'on évite toute injection SQL.

## À retenir

- **Compresser avant d'insérer** ; stocker le binaire en `LONGBLOB` paramétré.
- **Conserver l'algorithme** pour pouvoir relire.
- Mémoriser les **deux tailles** (gain) et une **empreinte** (intégrité).
- La table démarre **vide** : c'est l'application qui produit le binaire valide.

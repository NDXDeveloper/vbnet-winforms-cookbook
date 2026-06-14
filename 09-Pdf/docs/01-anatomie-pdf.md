# Fiche 01 — Anatomie d'un fichier PDF minimal

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Un PDF est un fichier texte (avec des sections binaires) composé de **quatre
parties** : un en-tête, un corps fait d'**objets numérotés**, une **table de
références croisées** (`xref`) et un **trailer**.

```
%PDF-1.4                 ← en-tête (version)
%âãÏÓ                     ← commentaire binaire (marque « fichier binaire »)

1 0 obj … endobj         ← objets numérotés (catalogue, pages, polices, contenu…)
2 0 obj … endobj
…

xref                     ← table : octet de départ de CHAQUE objet
0 N
0000000000 65535 f
0000000015 00000 n
…

trailer                  ← pointe vers le catalogue (/Root) et les infos (/Info)
<< /Size N /Root 1 0 R /Info K 0 R >>
startxref
12345                    ← octet où commence la table xref
%%EOF
```

## Les objets essentiels

| Objet | Rôle |
|---|---|
| **Catalogue** (`/Type /Catalog`) | racine du document ; pointe vers l'arbre de pages |
| **Arbre de pages** (`/Type /Pages`) | liste les pages (`/Kids`) et leur nombre (`/Count`) |
| **Police** (`/Type /Font`) | une des 14 polices standard, ici en `/WinAnsiEncoding` |
| **Page** (`/Type /Page`) | taille (`/MediaBox`), ressources, et son flux de contenu |
| **Flux de contenu** | les opérateurs de dessin (texte, traits…) |
| **Infos** (`/Info`) | métadonnées : titre, auteur, producteur |

Les objets se référencent par **« numéro génération R »** (ex. `2 0 R` = « objet 2,
génération 0 »).

## Le point délicat : les offsets

La table `xref` donne, pour chaque objet, son **décalage en octets** depuis le
début du fichier. Une seule erreur d'octet et le PDF est illisible. Dans le code,
on mémorise donc l'offset **juste avant** d'écrire chaque objet :

```vbnet
Private Shared Sub DebutObjet(sb As StringBuilder, offsets As Integer(), enc As Encoding, numero As Integer)
    offsets(numero) = enc.GetByteCount(sb.ToString())   ' position courante, en octets
    sb.Append(numero).Append(" 0 obj").Append(vbLf)
End Sub
```

On sérialise tout en **Windows-1252** (un octet par caractère pour le Latin
occidental), ce qui garde la correspondance « 1 caractère = 1 octet » et simplifie
le calcul des offsets.

## À retenir

- 4 parties : en-tête, objets, `xref`, trailer.
- Les objets se **référencent par numéro** ; le trailer désigne la racine.
- La table `xref` est une liste d'**offsets en octets** : précision indispensable.

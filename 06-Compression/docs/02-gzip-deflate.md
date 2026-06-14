# Fiche 02 — DEFLATE vs GZIP : que choisir ?

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Les deux algorithmes reposent sur le **même moteur de compression** (DEFLATE,
combinaison de LZ77 et du codage de Huffman). Ils diffèrent par leur
**enveloppe**.

| | DEFLATE (RFC 1951) | GZIP (RFC 1952) |
|---|---|---|
| Contenu | flux compressé **brut** | en-tête + DEFLATE + **CRC32** + taille |
| Surcoût | ~quelques octets | ~18 octets (en-tête + pied) |
| Contrôle d'intégrité | non | oui (CRC32 intégré) |
| Interopérabilité | interne | format de fichier `.gz` standard |
| Classe .NET | `DeflateStream` | `GZipStream` |

## Conséquences pratiques

- Pour de **très petites** données, DEFLATE est légèrement plus compact (pas
  d'en-tête). Pour des données volumineuses, l'écart devient négligeable.
- GZIP est le bon choix dès qu'on **échange des fichiers** ou qu'on veut un
  **contrôle d'intégrité** gratuit (CRC). C'est le défaut de ce projet.
- Les deux formats ne sont **pas interchangeables** : un flux écrit en GZIP doit
  être relu en GZIP. C'est pourquoi on **mémorise l'algorithme** à côté de chaque
  archive (colonne `algorithme`) pour savoir comment la décompresser.

## Dans le code

Le choix est une simple énumération, et la fabrique du flux ne change qu'une ligne :

```vbnet
Select Case algo
    Case AlgorithmeCompression.GZip    : Return New GZipStream(sousJacent, mode, leaveOpen:=True)
    Case AlgorithmeCompression.Deflate : Return New DeflateStream(sousJacent, mode, leaveOpen:=True)
End Select
```

## À retenir

- Même moteur, enveloppe différente : GZIP = DEFLATE + en-tête + CRC.
- Choisir **GZIP** par défaut (intégrité, interopérabilité) ; DEFLATE pour le
  strict minimum d'octets.
- **Conserver l'algorithme** utilisé : il faut le même pour décompresser.

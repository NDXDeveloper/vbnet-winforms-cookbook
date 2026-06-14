# Fiche 01 — Les flux de compression .NET (et le piège du *flush*)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Principe

`System.IO.Compression` fournit des **flux décorateurs** : on écrit des octets
*en clair* dans un `GZipStream`/`DeflateStream` ouvert en mode `Compress`, et il
écrit les octets *compressés* dans le flux sous-jacent (ici un `MemoryStream`).

```
octets clairs ──► GZipStream(Compress) ──► MemoryStream ──► .ToArray()
octets clairs ◄── GZipStream(Decompress) ◄── MemoryStream(données compressées)
```

## Le piège : lire le tampon trop tôt

Un flux de compression **bufferise** : ses derniers octets ne sont écrits dans le
flux sous-jacent qu'au moment où on le **ferme** (ou via `Flush`/`Dispose`). Si
l'on lit `MemoryStream.ToArray()` *avant* de fermer le `GZipStream`, le résultat
est **tronqué** et indécompressable.

Mais fermer le `GZipStream` ferme aussi, par défaut, le `MemoryStream` sous-jacent
— or on a justement besoin de le lire **après**. La solution : ouvrir le flux de
compression avec **`leaveOpen:=True`**, puis le fermer (fin du `Using`) avant de
lire le tampon.

```vbnet
Using sortie As New MemoryStream()
    Using flux As Stream = New GZipStream(sortie, CompressionMode.Compress, leaveOpen:=True)
        flux.Write(donnees, 0, donnees.Length)
    End Using                ' ferme le GZipStream (vide les tampons), garde "sortie" ouvert
    Return sortie.ToArray()  ' tampon complet et valide
End Using
```

## Décompression

À l'inverse, on enveloppe le flux d'entrée et on copie vers la sortie :

```vbnet
Using entree As New MemoryStream(donnees)
    Using flux As Stream = New GZipStream(entree, CompressionMode.Decompress, leaveOpen:=True)
        Using sortie As New MemoryStream()
            flux.CopyTo(sortie)
            Return sortie.ToArray()
        End Using
    End Using
End Using
```

## À retenir

- Le flux de compression doit être **fermé avant** de lire le tampon de sortie.
- `leaveOpen:=True` évite de fermer le flux sous-jacent qu'on veut encore lire.
- Le même code fonctionne pour `DeflateStream` : seul l'en-tête diffère (fiche 02).

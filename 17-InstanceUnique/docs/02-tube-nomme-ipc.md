# Fiche 02 — Communication inter-processus par tube nommé

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Pourquoi un tube nommé

Une fois l'unicité garantie (fiche 01), la **nouvelle** instance doit transmettre
ses arguments (« ouvre ce fichier ») à celle qui tourne déjà, puis se retirer. Un
**tube nommé** (named pipe) est un canal local simple pour cela.

## Le serveur écoute

L'instance primaire crée un serveur et attend une connexion, puis lit une ligne :

```vbnet
Using serveur As New NamedPipeServerStream(nomCanal, PipeDirection.In)
    Await serveur.WaitForConnectionAsync(jeton)
    Using lecteur As New StreamReader(serveur)
        Dim ligne = Await lecteur.ReadLineAsync()
        surReception(ligne)
    End Using
End Using
```

> Astuce UI : appelé depuis le fil d'interface, le code **après** `Await` reprend
> sur ce même fil. La callback `surReception` peut donc mettre à jour les contrôles
> **sans `Invoke`** (cf. le projet « Asynchrone »).

## Le client envoie

La seconde instance se connecte et écrit la ligne :

```vbnet
Using client As New NamedPipeClientStream(".", nomCanal, PipeDirection.Out)
    client.Connect(timeoutMs)            ' échoue si personne n'écoute
    Using ecrivain As New StreamWriter(client) With {.AutoFlush = True}
        ecrivain.WriteLine(message)
    End Using
End Using
```

Si la connexion échoue (personne n'écoute), `Envoyer` renvoie `False`.

## Une ligne, plusieurs arguments

Les arguments sont aplatis en **une seule ligne** par `Commande.Encoder`, qui
échappe le séparateur (`|`), l'antislash et les retours à la ligne ; `Decoder` fait
l'inverse. C'est une logique **pure**, testée par aller-retour (y compris avec des
arguments contenant `|`, `\` ou des sauts de ligne).

```vbnet
client : Envoyer(canal, Commande.Encoder(arguments))
serveur : args = Commande.Decoder(ligneRecue)
```

## À retenir

- Un **tube nommé** relie deux processus localement (serveur/clients).
- L'instance qui écoute lit une **ligne** ; la nouvelle instance l'**envoie** puis quitte.
- Les arguments sont **encodés/échappés** en une ligne — logique pure et testable.

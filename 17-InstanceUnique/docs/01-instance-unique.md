# Fiche 01 — Instance unique avec un `Mutex` nommé

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Le besoin

Certaines applications ne doivent tourner qu'**en un seul exemplaire** : relancer
le raccourci devrait *activer* la fenêtre déjà ouverte plutôt qu'en créer une
seconde.

## Le `Mutex` nommé

Un `Mutex` nommé est un objet de synchronisation **visible par tout le système**
(par son nom). Le constructeur indique, via `createdNew`, si c'est **nous** qui
venons de le créer :

```vbnet
Dim cree As Boolean
_mutex = New Mutex(initiallyOwned:=True, name:=nom, createdNew:=cree)
EstPremiere = cree    ' True = première instance ; False = une autre existe déjà
```

- 1ʳᵉ instance : `createdNew = True` → elle est **primaire**.
- instances suivantes : le mutex existe déjà → `createdNew = False`.

> Au démarrage d'une vraie application : si `EstPremiere = False`, on transmet les
> arguments à l'instance primaire (fiche 02) puis on **quitte**.

## Libérer proprement

Le mutex est une ressource système : la classe implémente `IDisposable` et ne
libère (`ReleaseMutex`) que si elle le **possède** :

```vbnet
Public Sub Dispose() Implements IDisposable.Dispose
    Try
        If _possede Then _mutex.ReleaseMutex()
    Catch
    End Try
    _mutex.Dispose()
End Sub
```

## Tester en mémoire

Pas besoin de lancer deux processus : dans **le même** processus, deux
`VerrouInstance` de même nom suffisent — le second voit `EstPremiere = False`. Après
libération du premier, le nom redevient disponible. Tout est donc testable hors
intégration.

## À retenir

- Un **`Mutex` nommé** = verrou d'unicité visible de tout le système.
- `createdNew` distingue l'instance **primaire** des suivantes.
- **Libérer** le mutex (`IDisposable`) ; ne `ReleaseMutex` que si on le possède.

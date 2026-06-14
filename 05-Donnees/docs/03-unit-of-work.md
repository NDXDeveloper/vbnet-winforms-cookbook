# 03 — Unit of Work (transactions)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Le patron **Unit of Work** regroupe plusieurs opérations dans **une même
transaction** : soit toutes réussissent (commit), soit aucune (rollback).

```vbnet
Using uow As New UniteDeTravail()
    uow.Commencer()
    Dim depot As New DepotProduit(uow)        ' le dépôt utilise la connexion/transaction de l'UoW
    depot.Inserer(produitA)
    depot.Inserer(produitB)
    uow.Valider()                              ' ... ou uow.Annuler()
End Using                                       ' Dispose : rollback de sécurité si non validé
```

## Garanties

- **Atomicité** : les insertions ci-dessus sont validées ensemble ou pas du tout.
- **Lecture intra-transaction** : `depot.Compter()` exécuté via la même UoW voit
  les lignes **non encore validées** (elles existent dans la transaction courante).
- **Rollback de sécurité** : si l'UoW est libérée (`Dispose`) sans `Valider`, la
  transaction est annulée — pas de demi-écriture en cas d'exception.

La page *Transaction* de la galerie le montre : compteur **avant / pendant / après**,
identique à l'« avant » après une annulation, augmenté de 3 après une validation.

## Pourquoi passer l'UoW au dépôt ?

Sans UoW, chaque appel du dépôt ouvre sa propre connexion : les opérations sont
indépendantes. Avec une UoW partagée, elles s'inscrivent dans la **même**
transaction — c'est ce qui rend l'ensemble atomique.

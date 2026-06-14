# Fiche 02 — Copier des propriétés et purger des événements

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Copier des propriétés par nom

Recopier un objet vers un autre sans écrire `dest.X = src.X` pour chaque propriété :
on parcourt par réflexion les propriétés **inscriptibles** de la destination, et,
pour chaque propriété de **même nom** et de **type compatible** dans la source, on
recopie la valeur.

```vbnet
For Each pDest In destination.GetType().GetProperties(PUBLICS_INSTANCE)
    If Not pDest.CanWrite Then Continue For
    Dim pSrc = source.GetType().GetProperty(pDest.Name, PUBLICS_INSTANCE)
    If pSrc IsNot Nothing AndAlso pSrc.CanRead _
       AndAlso pDest.PropertyType.IsAssignableFrom(pSrc.PropertyType) Then
        pDest.SetValue(destination, pSrc.GetValue(source, Nothing), Nothing)
    End If
Next
```

`IsAssignableFrom` évite les erreurs de type. Deux objets sans propriété commune →
0 copie (aucune exception).

## Purger tous les abonnés d'un événement

Un abonné qui ne se désabonne jamais **empêche le ramasse-miettes** de libérer
l'émetteur : c'est une fuite classique. Pour tout nettoyer d'un coup, on atteint le
**champ délégué** qui sous-tend l'événement et on le remet à `Nothing`.

Le nom de ce champ dépend du langage : **`<Nom>Event`** en VB, **`<Nom>`** en C#.

```vbnet
Dim champ = TrouverChamp(t, nomEvenement & "Event")   ' VB
If champ Is Nothing Then champ = TrouverChamp(t, nomEvenement)  ' C#
If GetType([Delegate]).IsAssignableFrom(champ.FieldType) Then
    champ.SetValue(cible, Nothing)   ' tous les abonnés disparaissent
End If
```

> On cherche le champ avec `BindingFlags.NonPublic Or Instance` (il est privé), en
> remontant éventuellement la hiérarchie d'héritage.

## Tester sans base

Tout est vérifiable en mémoire : copier une `Personne` (4 propriétés), constater
qu'`Article → Personne` ne copie rien (aucun nom commun), abonner deux gestionnaires
puis purger et vérifier que le compteur **ne bouge plus**.

## À retenir

- **Copier par réflexion** = mapping automatique des propriétés homonymes
  (`IsAssignableFrom` pour la sûreté de type).
- **Purger un événement** = remettre son champ délégué à `Nothing` (nom `…Event` en
  VB) — contre les fuites d'abonnements.
- Ces outils sont **purs** : testables directement, sans interface ni base.

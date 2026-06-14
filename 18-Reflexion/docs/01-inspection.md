# Fiche 01 — Inspecter un type par réflexion

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## La réflexion : interroger les types à l'exécution

La **réflexion** permet de découvrir, *au moment de l'exécution*, la structure d'un
type : ses propriétés, champs, événements, méthodes — sans rien savoir de lui à la
compilation. C'est le socle des sérialiseurs, ORM, conteneurs d'injection, etc.

## `BindingFlags` : choisir ce qu'on liste

On précise quels membres récupérer via des **`BindingFlags`** combinés :

```vbnet
Const PUBLICS_INSTANCE As BindingFlags = BindingFlags.Public Or BindingFlags.Instance

t.GetProperties(PUBLICS_INSTANCE)   ' propriétés publiques d'instance
t.GetFields(PUBLICS_INSTANCE)       ' champs publics d'instance
t.GetEvents(PUBLICS_INSTANCE)       ' événements publics d'instance
```

> Oublier `BindingFlags` (ou se tromper de combinaison) est l'erreur classique :
> on récupère alors trop (membres statiques, privés…) ou rien du tout.

## Projeter en données simples

Plutôt que de manipuler des `PropertyInfo`/`EventInfo` partout, on les projette en
un petit **descripteur** (nom, genre, type associé), facile à afficher ou à stocker :

```vbnet
t.GetProperties(PUBLICS_INSTANCE).
  Select(Function(p) New DescripteurMembre(p.Name, "Propriété", p.PropertyType.Name)).
  OrderBy(Function(d) d.Nom)
```

Comme l'inspection ne dépend que d'un `Type`, elle est **pure** : on la teste sur
des classes d'exemple (`Article` a 5 propriétés et 1 événement, etc.).

## À retenir

- La **réflexion** révèle la structure d'un type à l'exécution.
- Les **`BindingFlags`** déterminent précisément les membres listés.
- Projeter `PropertyInfo`/… en **descripteurs** simples facilite affichage et tests.

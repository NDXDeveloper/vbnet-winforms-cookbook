# 04 — Mapping objet-relationnel & résilience

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Mapping par réflexion (`Mappeur`)

`Mappeur.LireListe(Of T)` transforme les lignes d'un `IDataReader` en objets : pour
chaque colonne, il affecte la **propriété publique de même nom** (insensible à la
casse). La correspondance colonne → propriété est résolue **une seule fois**, puis
réutilisée pour chaque ligne (efficacité).

```vbnet
While lecteur.Read()
    Dim obj As New T()
    For Each paire In correspondance          ' (ordinal de colonne, propriété)
        Dim valeur = lecteur.GetValue(paire.Key)
        If Not Convert.IsDBNull(valeur) Then
            paire.Value.SetValue(obj, ConvertirVers(paire.Value.PropertyType, valeur), Nothing)
        End If
    Next
    resultat.Add(obj)
End While
```

`ConvertirVers` gère les types `Nullable` et convertit en culture invariante.
Avantage : un nouveau dépôt n'a **pas** de code de mapping à écrire ; il suffit
d'aliaser les colonnes sur les noms de propriétés (voir [`02-repository.md`](02-repository.md)).

## Résilience (`Resilience`)

Certaines erreurs de base sont **transitoires** : un interbloquage (deadlock,
code 1213) ou une attente de verrou expirée (1205) disparaissent souvent en
**réessayant**. `Resilience.Executer` rejoue l'opération avec un délai croissant :

```vbnet
Dim valeur = Resilience.Executer(Of Integer)(Function() depot.Compter())
```

- `NbTentativesMax` (défaut 3), `DelaiBaseMs` (défaut 50, multiplié par le numéro
  de tentative — *back-off* linéaire).
- Le **prédicat de transitoire** est remplaçable : par défaut il cible les codes
  MySQL/MariaDB 1213/1205 ; les tests injectent leur propre prédicat pour valider
  le comportement sans base.

> Important : on ne réessaie que ce qui est **sûr à rejouer**. Les opérations
> autonomes du dépôt le sont ; à l'intérieur d'une transaction, la reprise est
> gérée au niveau de l'Unit of Work (rejouer toute la transaction).

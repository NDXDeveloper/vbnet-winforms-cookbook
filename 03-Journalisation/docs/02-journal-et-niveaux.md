# 02 — Journal et niveaux

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Niveaux

| Niveau | Usage |
|---|---|
| `Debogage` | trace fine de diagnostic |
| `Information` | fonctionnement normal |
| `Avertissement` | anomalie non bloquante |
| `Erreur` | opération ayant échoué |
| `Critique` | défaillance grave |

`NiveauMinimal` filtre à la source : une entrée dont le niveau est inférieur au
seuil n'est ni créée ni diffusée (économie de ressources).

## Entrée de journal

`EntreeJournal` est **immuable** : horodatage, niveau, catégorie, message,
texte d'exception éventuel. `Formater()` en produit une ligne lisible :

```
[2026-02-01 08:06:00.000] [ERREUR  ] [BDD] Echec d'une requête.
    --> System.TimeoutException: délai dépassé.
```

## Utilisation

```vbnet
Dim journal As New Journal() With {.NiveauMinimal = Niveau.Information}
journal.AjouterPuits(New PuitsConsole())
journal.AjouterPuits(New PuitsFichier("app.log"))

journal.Information("Demarrage", "Application prête.")
journal.Erreur("BDD", "Requête en échec.", ex)   ' exception facultative
```

Méthodes de confort : `Debogage`, `Information`, `Avertissement`, `Erreur`,
`Critique` ; ou `Journaliser(niveau, catégorie, message, exception)`.

## Thread-safety

La diffusion est protégée par un verrou : plusieurs threads peuvent journaliser
simultanément sans corruption. Chaque puits est appelé dans un `Try/Catch` afin
qu'une destination défaillante n'impacte pas les autres.

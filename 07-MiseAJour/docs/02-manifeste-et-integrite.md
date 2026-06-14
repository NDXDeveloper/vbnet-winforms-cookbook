# Fiche 02 — Manifeste de déploiement et contrôle d'intégrité

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Le manifeste

Plutôt que de coder en dur « la dernière version est la 1.10.0 », on publie un
**manifeste** : une liste de versions disponibles, chacune décrite par une
`Publication` (version, notes, URL du paquet, empreinte attendue, caractère
obligatoire, date). L'application compare **sa** version à ce manifeste.

| Champ | Rôle |
|---|---|
| `Version` | numéro sémantique de la publication |
| `Notes` | nouveautés / corrections |
| `UrlPaquet` | où télécharger le paquet |
| `EmpreinteSha256` | empreinte attendue du paquet (intégrité) |
| `Obligatoire` | mise à jour imposée ? |
| `PublieeLe` | date de publication |

## Décider, sans surprise

`ServiceMiseAJour` est **pur** : on lui passe la version actuelle et le manifeste,
il répond. Aucune dépendance base ni réseau, donc des tests simples et rapides.

```vbnet
' La plus récente strictement supérieure à la version installée, sinon Nothing.
Public Shared Function RechercherDerniere(versionActuelle As VersionSemantique,
                                          publications As IEnumerable(Of Publication)) As Publication
    Return publications.
        Where(Function(p) p.Version > versionActuelle).
        OrderByDescending(Function(p) p.Version).
        FirstOrDefault()
End Function
```

Une mise à jour **obligatoire en attente** se repère de la même façon : existe-t-il
une publication `Obligatoire` dont la version dépasse la version installée ?

## Vérifier l'intégrité (SHA-256)

Un paquet téléchargé peut être corrompu (réseau) ou altéré (malveillance). On
compare donc l'empreinte **SHA-256** du fichier reçu à celle annoncée par le
manifeste **avant** de l'installer :

```vbnet
Public Shared Function VerifierIntegrite(paquet As Byte(), empreinteAttendue As String) As Boolean
    Return String.Equals(CalculerEmpreinte(paquet), empreinteAttendue.Trim(),
                         StringComparison.OrdinalIgnoreCase)
End Function
```

La comparaison est **insensible à la casse** (les empreintes hex s'écrivent
indifféremment en minuscules ou majuscules).

## À retenir

- Un **manifeste** externalise la liste des versions : pas de version codée en dur.
- La logique de décision est **pure** → testable hors-ligne.
- On **vérifie l'empreinte** d'un paquet avant de l'installer (intégrité +
  authenticité minimale).

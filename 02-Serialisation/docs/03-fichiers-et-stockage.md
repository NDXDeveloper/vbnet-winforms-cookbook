# 03 — Fichiers et stockage isolé

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Modules : `Serialisation/Serialiseur.vb` (fichier) et `Serialisation/StockageIsole.vb`.
Démonstration : page **Fichiers et stockage isolé**.

---

## 1. Vers / depuis un fichier

`Serialiseur` écrit la forme sérialisée directement sur le disque :

```vbnet
Serialiseur.SauverFichier(catalogue, "C:\temp\catalogue.xml", FormatSerialisation.Xml)
Dim copie As Catalogue = Serialiseur.ChargerFichier(Of Catalogue)("C:\temp\catalogue.xml", FormatSerialisation.Xml)
```

> Le **même format** doit être employé à l'écriture et à la lecture : un fichier
> écrit en JSON se relit en JSON. La page de démonstration laisse choisir le
> format ; charger avec un format incompatible lève une exception (capturée et
> affichée).

---

## 2. Le stockage isolé (`IsolatedStorage`)

Le **stockage isolé** est un espace de fichiers **privatif**, géré par .NET et
cloisonné par application et par utilisateur Windows. On y lit/écrit par un
simple **nom de fichier**, sans gérer de chemin absolu ni de droits d'accès — utile
pour des préférences, un cache ou un état applicatif.

`StockageIsole` combine ce mécanisme avec le `Serialiseur` :

```vbnet
StockageIsole.Sauver(catalogue, "catalogue.dat", FormatSerialisation.Binaire)
Dim existe As Boolean = StockageIsole.Existe("catalogue.dat")
Dim copie As Catalogue = StockageIsole.Charger(Of Catalogue)("catalogue.dat", FormatSerialisation.Binaire)
StockageIsole.Supprimer("catalogue.dat")
```

### Points clés

- `IsolatedStorageFile.GetUserStoreForAssembly()` ouvre le magasin de
  l'utilisateur courant pour l'assembly courant.
- L'emplacement physique réel (sous le profil utilisateur) est **géré par le
  framework** : l'application n'a pas à le connaître.
- La relecture lit l'intégralité du flux (`CopyTo`) avant de désérialiser, ce qui
  évite les lectures partielles.

---

## 3. Choisir le bon support

| Support | Usage typique |
|---|---|
| Tableau d'octets (`VersOctets`) | transmission réseau, mise en cache mémoire, colonne BLOB |
| Chaîne (`VersChaine`) | journalisation, aperçu, colonne texte |
| Fichier (`SauverFichier`) | export/import, échange de fichiers |
| Stockage isolé (`StockageIsole`) | préférences/état applicatif, sans gestion de chemin |
| Base de données | voir [`04-persistance-bdd.md`](04-persistance-bdd.md) |

# 04 — Chaînes et conversions

> Nettoyage de chaînes (accents, caractères spéciaux, GUID) et conversions numériques.

Modules concernés : `Chaines/OutilsChaines.vb` et `Conversions/OutilsConversions.vb`.

---

## 1. Suppression des accents — `RetirerAccents`

Technique : **normalisation Unicode en forme D** (`FormD`), qui décompose un
caractère accentué en *caractère de base* + *marque diacritique*. On supprime
ensuite les marques (catégorie `NonSpacingMark`), puis on passe en minuscules.

```vbnet
Dim normalisee As String = texte.Normalize(NormalizationForm.FormD)
For Each c As Char In normalisee
    If CharUnicodeInfo.GetUnicodeCategory(c) <> UnicodeCategory.NonSpacingMark Then
        sb.Append(c)
    End If
Next
Return sb.ToString().ToLowerInvariant()
```

`"Évènement"` → `"evenement"`, `"Ça Va"` → `"ca va"`.

---

## 2. Nettoyage par expression régulière

- `RetirerCaracteresSpeciaux` ne conserve que `[a-zA-Z0-9_.]` (RegEx **compilée**
  et réutilisée pour la performance).
- `RendreFiltreValide` retire d'abord les caractères interdits dans un nom de
  fichier (`Path.GetInvalidFileNameChars`), puis applique le nettoyage ci-dessus.

> `RendreNomFichierValide` (module `OutilsFichiers`) suit la même logique mais
> **remplace** les caractères interdits par `_` au lieu de les supprimer.

---

## 3. Détection de GUID — `ContientGuid`

Recherche d'un GUID canonique (8-4-4-4-12 hexadécimaux) par expression régulière
compilée :

```
\b[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}\b
```

---

## 4. Première lettre en majuscule — `PremiereLettreEnMajuscule`

Gère les trois cas (vide / un caractère / plusieurs) sans lever d'exception.

---

## 5. Conversions — `OutilsConversions`

### `ConvertirEnEntier`
Convertit une chaîne en entier ; chaîne vide ⇒ `0` ; échec ⇒ **`-1`** (valeur
sentinelle, plutôt qu'une exception remontée à l'appelant).

### `Formater`
Supprime les décimales inutiles : un entier est rendu sans virgule, sinon deux
décimales sont conservées. Le formatage utilise la **culture invariante** pour un
séparateur décimal stable.

```vbnet
Dim s As String = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", nombre)
If s.EndsWith("00") Then Return CInt(Math.Truncate(nombre)).ToString(CultureInfo.InvariantCulture)
Return s
```

`12.0` → `"12"`, `12.5` → `"12.50"`.

---

## Tests associés

`TestsChaines`, `TestsConversions` (projet `Tests`) couvrent tous ces cas — voir
la classe correspondante pour les valeurs attendues.

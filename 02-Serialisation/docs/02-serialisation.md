# 02 — Sérialisation et formats

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

Module : `Serialisation/Serialiseur.vb`. Démonstration : page **Sérialisation et formats**.

La **sérialisation** convertit un objet en une suite d'octets (ou de caractères)
que l'on peut stocker ou transmettre ; la **désérialisation** fait l'inverse.

---

## 1. Un point d'entrée, quatre formats

`Serialiseur` expose une API uniforme, indépendante du format :

```vbnet
Function VersOctets(Of T)(obj As T, format As FormatSerialisation) As Byte()
Function DepuisOctets(Of T)(octets As Byte(), format As FormatSerialisation) As T

Function VersChaine(Of T)(obj As T, format As FormatSerialisation) As String   ' formats texte
Function DepuisChaine(Of T)(donnees As String, format As FormatSerialisation) As T

Sub      SauverFichier(Of T)(obj As T, chemin As String, format As FormatSerialisation)
Function ChargerFichier(Of T)(chemin As String, format As FormatSerialisation) As T
```

Le tableau d'octets est la **forme canonique** ; la chaîne et le fichier en
dérivent. `EstFormatTexte(format)` indique si le format est représentable en
texte (tout sauf le binaire).

---

## 2. Les formats (`FormatSerialisation`)

| Format | Moteur | Sortie | Quand l'utiliser |
|---|---|---|---|
| `Xml` | `XmlSerializer` | XML lisible | interopérabilité, lecture humaine, fichiers de config |
| `ContratXml` | `DataContractSerializer` | XML (contrat) | contrat de données strict, graphes plus riches |
| `Binaire` | `DataContractSerializer` + écrivain binaire | binaire compact | taille réduite, échanges internes |
| `Json` | `DataContractJsonSerializer` | JSON | échanges web, APIs |

Tous sont **intégrés à .NET Framework** (aucune dépendance tierce).

### Exemple — XML

```vbnet
Dim xml As String = Serialiseur.VersChaine(catalogue, FormatSerialisation.Xml)
Dim copie As Catalogue = Serialiseur.DepuisChaine(Of Catalogue)(xml, FormatSerialisation.Xml)
```

### Exemple — binaire (compact)

```vbnet
Dim octets As Byte() = Serialiseur.VersOctets(catalogue, FormatSerialisation.Binaire)
Dim copie As Catalogue = Serialiseur.DepuisOctets(Of Catalogue)(octets, FormatSerialisation.Binaire)
```

La page de démonstration compare les **tailles** produites : le binaire est en
général sensiblement plus compact que le XML pour le même objet.

---

## 3. Rendre une classe sérialisable

Le modèle de démonstration (`Produit`, `Catalogue`) respecte les règles communes :

- **type public** et **constructeur public sans paramètre** (requis par `XmlSerializer`) ;
- **propriétés publiques** en lecture/écriture ;
- attributs `<DataContract>` / `<DataMember>` pour guider le contrat de données
  (XML de contrat et JSON). `XmlSerializer` les ignore et se fonde sur les
  propriétés publiques : une même classe fonctionne donc avec **les quatre formats**.

```vbnet
<DataContract([Namespace]:="")>
Public Class Produit
    <DataMember(Order:=0)> Public Property Reference As String
    <DataMember(Order:=1)> Public Property Designation As String
    <DataMember(Order:=2)> Public Property PrixHt As Decimal
    <DataMember(Order:=3)> Public Property Quantite As Integer
    Public Sub New()
    End Sub
End Class
```

> Pour contrôler finement le XML produit (noms d'éléments, attributs, ordre), on
> peut décorer les propriétés avec les attributs `System.Xml.Serialization`
> (`<XmlElement>`, `<XmlAttribute>`, `<XmlArray>`…). Ils n'affectent que
> `XmlSerializer`.

---

## 4. Sécurité : pourquoi pas `BinaryFormatter` ?

La sérialisation binaire historique (`BinaryFormatter`) est **obsolète** et
constitue un **risque de sécurité** majeur : désérialiser des données binaires
non fiables peut exécuter du code arbitraire (attaques par *deserialization*).
Elle est donc volontairement écartée. Le format `Binaire` de ce projet repose sur
`XmlDictionaryWriter.CreateBinaryWriter` (contrat de données), qui offre la
compacité **sans** ce risque.

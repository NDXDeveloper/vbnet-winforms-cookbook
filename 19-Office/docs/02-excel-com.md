# Fiche 02 — Automation Excel par COM (voie optionnelle)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Quand l'utiliser (et quand l'éviter)

Piloter Excel par **automation COM** permet d'exploiter le moteur d'Excel (formules,
mises en forme avancées, formats que seul Excel produit). Mais cela **exige
qu'Excel soit installé**, et c'est plus lourd que d'écrire directement le `.xlsx`.

> Règle : pour produire un simple classeur, préférer la voie **sans Office**
> (fiche 01). Réserver la voie COM aux cas qui ont vraiment besoin d'Excel.

## Détecter Excel sans le démarrer

```vbnet
Public Shared Function EstDisponible() As Boolean
    Return Type.GetTypeFromProgID("Excel.Application") IsNot Nothing
End Function
```

`GetTypeFromProgID` consulte le registre **sans** lancer Excel : idéal pour décider
si la voie COM est proposée.

## Liaison tardive plutôt qu'interop figé

On pilote Excel en **liaison tardive** (`Option Strict Off`, isolé dans un seul
fichier) : pas de référence d'interop à une version précise d'Excel.

```vbnet
Dim typeExcel = Type.GetTypeFromProgID("Excel.Application")
Dim app As Object = Activator.CreateInstance(typeExcel)
app.Visible = False
Dim classeur = app.Workbooks.Add()
classeur.Worksheets(1).Cells(1, 1).Value = "Bonjour"
classeur.SaveAs(chemin, 51)        ' 51 = xlsx
classeur.Close(False) : app.Quit()
```

## Libérer les objets COM

Les objets COM ne sont pas gérés par le ramasse-miettes .NET : on les libère
explicitement, dans un `Finally`, sinon le processus Excel **reste en mémoire**.

```vbnet
Finally
    If classeur IsNot Nothing Then Marshal.FinalReleaseComObject(classeur)
    If application IsNot Nothing Then Marshal.FinalReleaseComObject(application)
    GC.Collect() : GC.WaitForPendingFinalizers()
End Try
```

## Conséquence pour les tests

Le code **compile** toujours (liaison tardive). À l'exécution, si Excel est absent,
le test d'export est marqué **`Inconclusive`** (ni succès, ni échec) — la voie
« sans Office » reste, elle, pleinement testée.

## À retenir

- COM Excel = puissant mais **exige Excel installé** ; sinon, voie « sans Office ».
- **Détecter** via `GetTypeFromProgID` ; **piloter** en liaison tardive isolée.
- **Libérer** les objets COM (`FinalReleaseComObject`) pour ne pas laisser Excel ouvert.
- Sans Excel, le test COM est **Inconclusive** (et non en échec).

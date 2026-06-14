# Fiche 01 — Raccourcis Windows `.lnk` via COM (liaison tardive)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Un `.lnk` n'est pas un fichier texte

Contrairement au `.url`, un raccourci Windows `.lnk` est un **format binaire**
géré par le shell (interface COM `IShellLink`). On ne l'écrit pas « à la main » :
on demande au système de le créer.

## Deux options — et pourquoi on choisit la liaison tardive

| Approche | Coût |
|---|---|
| **P/Invoke `IShellLink`** | déclarer à la main les interfaces COM (`IShellLinkW`, `IPersistFile`), l'ordre exact des méthodes du vtable, les marshaling… **fragile** |
| **`WScript.Shell` en liaison tardive** | objet COM de haut niveau, présent partout, piloté par `CreateObject` — **simple et robuste** |

On retient la **liaison tardive**, isolée dans un fichier en `Option Strict Off`
(le reste de la bibliothèque reste en `Option Strict On`) :

```vbnet
Option Strict Off
...
Dim shell As Object = CreateObject("WScript.Shell")
Dim lnk As Object = shell.CreateShortcut(cheminLnk)
lnk.TargetPath = cible
lnk.Arguments = arguments
lnk.Description = description
lnk.WorkingDirectory = dossierTravail
lnk.Save()
```

Lire un raccourci existant est symétrique :

```vbnet
Dim lnk As Object = CreateObject("WScript.Shell").CreateShortcut(cheminLnk)
Return CStr(lnk.TargetPath)
```

## Le piège : l'appartement COM (STA)

`WScript.Shell` est un objet COM « à cloisonnement » : il s'utilise depuis un
thread **STA** (Single-Threaded Apartment). L'interface graphique WinForms l'est
déjà — aucun souci dans l'application. En revanche, un test MSTest s'exécute par
défaut en **MTA** : on exécute donc les opérations `.lnk` sur un thread STA dédié.

```vbnet
Dim t As New Thread(Sub() RaccourciWindows.Creer(lnk, cible))
t.SetApartmentState(ApartmentState.STA)
t.Start() : t.Join()
```

## À retenir

- Un `.lnk` est **binaire** : on passe par le shell, pas par l'écriture directe.
- La **liaison tardive** sur `WScript.Shell` évite les déclarations COM fragiles
  (un seul fichier en `Option Strict Off`).
- Les objets COM « apartment » exigent un thread **STA** (déjà le cas de l'UI ;
  à forcer dans les tests).

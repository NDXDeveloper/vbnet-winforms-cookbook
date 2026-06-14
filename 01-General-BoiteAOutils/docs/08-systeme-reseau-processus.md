# 08 — Système, réseau et processus

Modules : `Systeme/OutilsSysteme.vb`, `Reseau/OutilsReseau.vb`,
`Reseau/MoniteurReseau.vb`, `Processus/OutilsProcessus.vb`, `Interop/ApiWindows.vb`.

---

## 1. Interop Win32 (P/Invoke) — `ApiWindows`

Déclarations natives via `DllImport` :

```vbnet
<DllImport("user32.dll", SetLastError:=True)>
Public Function GetWindowThreadProcessId(hWnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer
End Function

<DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
Public Function SystemParametersInfo(uAction As Integer, uParam As Integer,
                                     lpvParam As Integer, fuWinIni As Integer) As Integer
End Function
```

Utilisées par `OutilsSysteme.DesactiverBips` (`SPI_SETBEEP`),
`DesactiverEconomiseurEcran` (`SPI_SETSCREENSAVEACTIVE`) et
`OutilsProcessus.TuerProcessusParHandleFenetre`.

## 2. Informations système — `OutilsSysteme`

- `MemoirePhysiqueDisponible` — via `PerformanceCounter("Memory","Available Bytes")`
  (dégrade proprement si les compteurs sont indisponibles).
- `NomUtilisateur`, `NomMachine`, `NomSession` — `NomSession` renvoie `""` pour une
  session console locale (utile pour détecter le bureau à distance).
- `DefinirCulture` — force `fr-FR` avec séparateur décimal `"."` (cohérence avec
  les valeurs envoyées à la base).
- `ForcerRamasseMiettes` — `GC.Collect` + `WaitForPendingFinalizers` (à réserver à
  des points précis).
- `SerialiserEnXml` — sérialise un objet via `XmlSerializer` (l'objet doit avoir un
  constructeur sans paramètre et des membres publics).

## 3. Réseau — `OutilsReseau`

- `AdresseIpLocale` — première IPv4 non locale-lien via `Dns.GetHostEntry`.
- `EstAdresseIPv4Valide` — format **et** bornes 0–255.
  > **Validation stricte** : on contrôle le format ET chaque octet ; une valeur
  > comme `999.1.1.1` est donc rejetée.
- `EnvoyerEmail` — envoi SMTP (`SmtpClient`), hôte/port lus dans `App.config`.
  En l'absence de serveur SMTP, l'échec est capturé et journalisé proprement
  (méthode de démonstration).

## 4. Moniteur réseau — `MoniteurReseau` (BackgroundWorker)

Illustration du patron `BackgroundWorker` :

- `DoWork` exécute une **boucle de test** de la base sur un **thread d'arrière-plan** ;
- les changements d'état remontent via `ReportProgress`, donc reçus sur le
  **thread d'interface** (s'il existe un `SynchronizationContext`) ;
- l'événement `EtatChange(disponible As Boolean)` est ainsi **manipulable sans
  risque depuis l'UI** ;
- la classe implémente `IDisposable` pour garantir l'arrêt propre du thread.

```vbnet
While Not _worker.CancellationPending
    Dim disponible = TesterDisponibilite()
    If Not _dernierEtat.HasValue OrElse _dernierEtat.Value <> disponible Then
        _dernierEtat = disponible
        _worker.ReportProgress(If(disponible, 1, 0))   ' → thread UI
    End If
    Thread.Sleep(_intervalleMs)
End While
```

La page **Réseau et processus** démarre/arrête le moniteur ; un libellé passe au
vert/rouge selon la disponibilité de la base (essayez d'arrêter le conteneur !).

## 5. Processus — `OutilsProcessus`

- `TuerProcessusSiPresent(nom)` — termine tous les processus d'un nom donné.
- `TuerProcessusParHandleFenetre(hWnd)` — combine l'appel natif
  `GetWindowThreadProcessId` et l'API managée `Process.GetProcessById(...).Kill()`.

> La page de démonstration **demande confirmation** avant de tuer un processus.

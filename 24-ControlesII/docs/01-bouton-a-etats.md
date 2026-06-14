# Fiche 01 — Un bouton à états (et `IButtonControl`)

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## Séparer l'état du rendu

Un bouton soigné change d'apparence selon qu'il est **au repos**, **survolé**,
**enfoncé** ou **désactivé**. Plutôt que d'éparpiller cette logique dans les
gestionnaires, on l'isole : `CalculEtat` répond « quel état ? » à partir de trois
booléens, avec une **priorité** claire.

```vbnet
Public Shared Function Determiner(active As Boolean, survol As Boolean, enfonce As Boolean) As EtatBouton
    If Not active Then Return EtatBouton.Desactive   ' priorité 1
    If enfonce Then Return EtatBouton.Enfonce         ' priorité 2
    If survol Then Return EtatBouton.Survol           ' priorité 3
    Return EtatBouton.Normal
End Function
```

C'est **pur** : on le teste sans rien dessiner (désactivé prioritaire, enfoncé avant
survol, etc.).

## Le contrôle suit les entrées et peint

```vbnet
Protected Overrides Sub OnMouseEnter(e) : _survol = True : Invalidate() : ...
Protected Overrides Sub OnMouseDown(e)  : _enfonce = True : Invalidate() : ...
Protected Overrides Sub OnPaint(e)
    Dim couleur = CouleurSelon(CalculEtat.Determiner(Enabled, _survol, _enfonce))
    ... remplir un rectangle arrondi + dessiner le texte
End Sub
```

(Double tampon activé via `SetStyle` pour éviter le scintillement.)

## `IButtonControl` : devenir bouton « par défaut »

En implémentant `IButtonControl`, le contrôle peut servir de bouton **par défaut**
(touche Entrée) ou d'**annulation** (Échap) d'un formulaire :

```vbnet
Public Property DialogResult As DialogResult Implements IButtonControl.DialogResult
Public Sub NotifyDefault(value As Boolean) Implements IButtonControl.NotifyDefault
Public Sub PerformClick() Implements IButtonControl.PerformClick
    If Enabled Then OnClick(EventArgs.Empty)
End Sub
```

Le formulaire pourra alors faire `Me.AcceptButton = monBouton`.

## À retenir

- **Isoler la décision** (l'état) du **rendu** : `CalculEtat` pur + `OnPaint`.
- Suivre survol/enfoncé via les événements souris ; `Invalidate()` pour redessiner.
- **`IButtonControl`** intègre le bouton aux boîtes de dialogue (défaut/annulation).

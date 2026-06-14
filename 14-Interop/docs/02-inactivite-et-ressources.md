# Fiche 02 — Inactivité, repli des ticks et fuites de handles

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Mesurer l'inactivité

`GetLastInputInfo` renvoie le **tick** (ms depuis le démarrage) de la dernière
entrée clavier/souris. L'inactivité = tick actuel − tick de la dernière entrée :

```vbnet
Dim ms = MillisecondesInactif(Environment.TickCount And &HFFFFFFFF, info.dwTime)
Return TimeSpan.FromMilliseconds(ms)
```

Utile pour : verrouiller une session, mettre en veille, déclencher un économiseur…

## Le piège : le repli (wraparound) du compteur 32 bits

Le compteur de ticks est un entier **32 bits non signé** : il revient à zéro toutes
les ~49,7 jours. Si la dernière entrée a eu lieu **juste avant** ce repli, une
soustraction naïve donne un nombre **négatif** énorme. La parade : faire le calcul
**modulo 2³²** :

```vbnet
Public Shared Function MillisecondesInactif(tickActuel As Long, tickDerniereEntree As Long) As Long
    Return (tickActuel - tickDerniereEntree) And &HFFFFFFFF   ' delta correct même au repli
End Function
```

Exemple : tick actuel = 50 (vient de repasser par 0), dernière entrée =
2³² − 6 = 4 294 967 290. La vraie durée est **56 ms** — et c'est bien ce que rend la
formule, alors qu'une soustraction simple donnerait −4 294 967 240.

Comme ce calcul est **pur**, on le teste exhaustivement (cas normal, égalité, repli)
sans dépendre de l'horloge système.

## Diagnostiquer les fuites de handles

`GetGuiResources` compte les objets **GDI** (stylos, pinceaux, polices, bitmaps…) et
**USER** (fenêtres, menus…) détenus par le processus. Si ce nombre **croît sans
cesse**, c'est le signe d'une fuite : des objets GDI+ créés mais jamais `Dispose`-és.
Surveiller ce compteur pendant l'utilisation est un excellent **diagnostic de
robustesse** d'une application WinForms.

> Rappel : chaque `Pen`, `Brush`, `Font`, `Bitmap`… doit être libéré (`Using`).

## À retenir

- L'inactivité se déduit de `GetLastInputInfo` ; **gérer le repli** 32 bits.
- Le calcul du delta, **pur**, est testable sans système.
- `GetGuiResources` aide à **repérer les fuites** d'objets GDI/USER.

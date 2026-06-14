# Fiche 01 — L'héritage visuel de formulaires

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev&#64;gmail.com&gt; — Licence MIT.

## L'idée : factoriser l'ossature commune

Si toutes les fenêtres d'une application partagent une même structure (un bandeau
de titre, une zone de contenu, un thème), on ne la recode pas à chaque fois : on la
met **une seule fois** dans une **fiche de base**, et les fiches concrètes en
**héritent**.

```vbnet
Public Class FormulaireBase
    Inherits Form
    Protected ReadOnly Contenu As New Panel()   ' les dérivées y placent leurs contrôles
    Public Property TitreFiche As String         ' l'en-tête est géré ici
    Public Sub AppliquerTheme(theme As Theme)    ' le thème aussi
End Class
```

## Une fiche concrète

La fiche dérivée ne s'occupe que de **son** contenu :

```vbnet
Public NotInheritable Class FrmExemple
    Inherits FormulaireBase
    Public Sub New(titre As String, theme As Theme)
        Me.TitreFiche = titre               ' en-tête fourni par la base
        Me.Contenu.Controls.Add(...)         ' Contenu est Protected -> accessible
        AppliquerTheme(theme)                ' thème fourni par la base
    End Sub
End Class
```

Elle obtient **gratuitement** l'en-tête, la zone de contenu et le thème. Tout
changement dans `FormulaireBase` profite à **toutes** les fiches.

## Points d'attention

- Le membre partagé doit être **`Protected`** (`Contenu`) pour rester accessible
  aux dérivées tout en restant masqué à l'extérieur.
- On applique le thème **après** avoir ajouté les contrôles, pour qu'ils soient
  colorés eux aussi.
- L'héritage fonctionne **entre assemblages** : la base est dans `Core`, les fiches
  concrètes dans l'UI.

## À retenir

- L'**héritage visuel** factorise l'apparence/comportement communs dans une fiche
  de base.
- Les dérivées n'ajoutent que leur contenu ; `Protected` expose juste ce qu'il faut.
- Un seul endroit à modifier pour faire évoluer toutes les fenêtres.

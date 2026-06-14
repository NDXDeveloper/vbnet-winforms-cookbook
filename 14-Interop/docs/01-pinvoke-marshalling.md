# Fiche 01 — P/Invoke : appeler le code natif et le *marshalling*

> Copyright (c) 2026 Nicolas DEOUX &lt;NDXDev@gmail.com&gt; — Licence MIT.

## Qu'est-ce que P/Invoke ?

*Platform Invocation* permet d'appeler une fonction d'une **DLL système** (écrite en
C) depuis du code managé. On déclare la signature managée et on l'annote avec
**`DllImport`** ; le moteur .NET se charge de la conversion des paramètres
(*marshalling*).

```vbnet
<DllImport("user32.dll")>
Public Function GetGuiResources(hProcess As IntPtr, uiFlags As Integer) As UInteger
End Function
```

## Faire correspondre les types

Le piège principal : la signature managée doit **correspondre exactement** à la
fonction native, sinon la pile mémoire est corrompue. Quelques règles :

| Type C | Type VB.NET |
|---|---|
| `HANDLE`, `HWND` | `IntPtr` |
| `DWORD` (non signé 32) | `UInteger` |
| `BOOL` | `Boolean` |
| `LPWSTR` | `String` (avec `CharSet`) |
| `struct *` | `ByRef <structure>` |

## Marshaller une structure

Certaines fonctions attendent un pointeur vers une structure. On la décrit avec
**`StructLayout(LayoutKind.Sequential)`** pour garantir l'ordre des champs en
mémoire, et on renseigne sa taille avant l'appel :

```vbnet
<StructLayout(LayoutKind.Sequential)>
Public Structure LASTINPUTINFO
    Public cbSize As UInteger
    Public dwTime As UInteger
End Structure

Dim info As New LASTINPUTINFO()
info.cbSize = CUInt(Marshal.SizeOf(info))   ' la fonction vérifie cette taille
GetLastInputInfo(info)                       ' passée ByRef
```

## Bonnes pratiques

- **Isoler** les déclarations natives dans un module interne (`Friend`), et exposer
  des méthodes managées propres (`Inactivite`, `RessourcesProcessus`…).
- Documenter les **constantes** (`GR_GDIOBJECTS = 0`, `HWND_TOPMOST = -1`…).
- Ne renvoyer aux appelants que des types .NET (`TimeSpan`, `Integer`, `Boolean`).

## À retenir

- `DllImport` relie une méthode managée à une fonction native.
- Le **marshalling** doit respecter exactement la signature C (types, `StructLayout`).
- On encapsule le natif derrière une **API managée propre**.

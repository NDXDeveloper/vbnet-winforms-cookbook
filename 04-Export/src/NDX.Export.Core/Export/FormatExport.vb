' ============================================================================
'  FormatExport.vb  -  Formats d'export pris en charge.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

''' <summary>Formats d'export disponibles.</summary>
Public Enum FormatExport
    ''' <summary>Texte separe (CSV), lisible et universel.</summary>
    Csv = 0
    ''' <summary>Classeur Excel .xlsx (Office Open XML), sans Excel installe.</summary>
    Excel = 1
    ''' <summary>Document PDF (texte tabule, police a chasse fixe).</summary>
    Pdf = 2
End Enum

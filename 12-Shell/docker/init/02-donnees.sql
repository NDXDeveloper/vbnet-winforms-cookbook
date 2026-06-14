-- ============================================================================
--  Catalogue de liens - Donnees initiales (projet 12 - Interop Shell)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE catalogue_liens;

INSERT INTO lien (categorie, nom, cible, description) VALUES
 ('application', 'Bloc-notes',     'C:\Windows\System32\notepad.exe',    'Éditeur de texte Windows.'),
 ('application', 'Calculatrice',   'C:\Windows\System32\calc.exe',       'Calculatrice Windows.'),
 ('application', 'Invite de commandes', 'C:\Windows\System32\cmd.exe',   'Console de commandes.'),
 ('web',         'Documentation .NET', 'https://learn.microsoft.com/dotnet/', 'Documentation officielle .NET.'),
 ('web',         'Exemple',        'https://exemple.test/',              'Lien Web de démonstration.');

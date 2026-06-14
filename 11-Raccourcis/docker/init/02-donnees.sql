-- ============================================================================
--  Raccourcis - Donnees initiales (projet 11 - Raccourcis clavier)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE raccourcis;

INSERT INTO liaison_clavier (action, raccourci) VALUES
 ('Enregistrer',           'Ctrl+S'),
 ('Tout enregistrer',      'Ctrl+K, Ctrl+S'),
 ('Commenter la ligne',    'Ctrl+K, Ctrl+C'),
 ('Palette de commandes',  'Ctrl+Maj+P'),
 ('Rechercher',            'Ctrl+F'),
 ('Remplacer',             'Ctrl+H'),
 ('Plein écran',           'F11')
ON DUPLICATE KEY UPDATE raccourci = VALUES(raccourci);

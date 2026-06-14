-- ============================================================================
--  Themes - Donnees initiales (projet 20 - FormBase / themes)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE themes;

INSERT INTO theme (nom, fond_hex, texte_hex, accent_hex) VALUES
 ('Clair',  '#FFFFFF', '#212121', '#3949AB'),
 ('Sombre', '#252526', '#DCDCDC', '#7C4DFF')
ON DUPLICATE KEY UPDATE fond_hex = VALUES(fond_hex), texte_hex = VALUES(texte_hex), accent_hex = VALUES(accent_hex);

-- ============================================================================
--  Preferences - Donnees initiales (projet 08 - Controles personnalises)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE preferences;

INSERT INTO preference (cle, valeur) VALUES
 ('bascule.notifications',     'True'),
 ('bascule.theme_sombre',      'False'),
 ('visionneuse.dernier_zoom',  '1.000')
ON DUPLICATE KEY UPDATE valeur = VALUES(valeur);

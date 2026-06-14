-- ============================================================================
--  Preferences - Schema (projet 08 - Controles personnalises)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE preferences;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS preference (
    cle     VARCHAR(100) NOT NULL COMMENT 'Clef de la preference.',
    valeur  TEXT         NULL     COMMENT 'Valeur (texte libre).',
    maj_le  DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
            COMMENT 'Derniere mise a jour.',
    PRIMARY KEY (cle)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Preferences clef/valeur des controles.';

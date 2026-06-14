-- ============================================================================
--  Raccourcis - Schema (projet 11 - Raccourcis clavier)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE raccourcis;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS liaison_clavier (
    action     VARCHAR(100) NOT NULL COMMENT 'Nom de l''action.',
    raccourci  VARCHAR(100) NOT NULL COMMENT 'Raccourci sous forme canonique (ex. Ctrl+K, Ctrl+S).',
    maj_le     DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
               COMMENT 'Derniere mise a jour.',
    PRIMARY KEY (action)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Liaisons action <-> raccourci clavier.';

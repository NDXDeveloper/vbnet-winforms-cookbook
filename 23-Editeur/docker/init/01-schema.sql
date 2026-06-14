-- ============================================================================
--  Redaction - Schema (projet 23 - Editeur RTF)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE redaction;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS document (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    titre       VARCHAR(200) NOT NULL COMMENT 'Titre du document.',
    contenu_rtf MEDIUMTEXT   NOT NULL COMMENT 'Contenu au format RTF (texte enrichi).',
    cree_le     DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de creation.',
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Documents au format RTF.';

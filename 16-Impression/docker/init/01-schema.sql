-- ============================================================================
--  Impressions - Schema (projet 16 - Impression de documents)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE impressions;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS impression (
    id        INT UNSIGNED NOT NULL AUTO_INCREMENT,
    titre     VARCHAR(200) NOT NULL COMMENT 'Titre du document.',
    contenu   MEDIUMTEXT   NOT NULL COMMENT 'Texte du document (lignes).',
    nb_pages  INT          NOT NULL DEFAULT 1 COMMENT 'Nombre de pages (estime).',
    cree_le   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de soumission.',
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Documents soumis a l''impression.';

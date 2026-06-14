-- ============================================================================
--  Demarrage - Schema (projet 25 - Demarrage applicatif)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE demarrage;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS evenement (
    id        INT UNSIGNED NOT NULL AUTO_INCREMENT,
    type      VARCHAR(30)  NOT NULL COMMENT 'etape / exception / info...',
    message   VARCHAR(500) NOT NULL COMMENT 'Detail de l''evenement.',
    horodate  DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Horodatage.',
    PRIMARY KEY (id),
    KEY ix_evenement_type (type)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Journal des evenements de demarrage et des exceptions.';

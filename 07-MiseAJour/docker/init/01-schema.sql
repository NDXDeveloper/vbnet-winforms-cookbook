-- ============================================================================
--  Deploiement - Schema (projet 07 - Mise a jour applicative)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE deploiement;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS publication (
    id               INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    version          VARCHAR(20)   NOT NULL COMMENT 'Version semantique : Majeure.Mineure.Corrective.',
    notes            TEXT          NULL     COMMENT 'Notes de version.',
    url_paquet       VARCHAR(400)  NULL     COMMENT 'Adresse de telechargement du paquet.',
    empreinte_sha256 CHAR(64)      NULL     COMMENT 'SHA-256 hex attendu du paquet.',
    obligatoire      TINYINT(1)    NOT NULL DEFAULT 0 COMMENT 'Mise a jour imposee (1) ou facultative (0).',
    publiee_le       DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de publication.',
    PRIMARY KEY (id),
    UNIQUE KEY uq_publication_version (version)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Manifeste des versions publiees.';

-- ============================================================================
--  Classeur - Schema (projet 19 - Office / tableur xlsx)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE classeur;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS enregistrement (
    id       INT UNSIGNED NOT NULL AUTO_INCREMENT,
    valeur1  VARCHAR(255) NOT NULL DEFAULT '' COMMENT 'Premiere colonne.',
    valeur2  VARCHAR(255) NOT NULL DEFAULT '' COMMENT 'Deuxieme colonne.',
    valeur3  VARCHAR(255) NOT NULL DEFAULT '' COMMENT 'Troisieme colonne.',
    cree_le  DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de creation.',
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Donnees a echanger avec des classeurs .xlsx.';

-- ============================================================================
--  Traitements - Schema (projet 13 - Asynchrone)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE traitements;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS tache (
    id           INT UNSIGNED NOT NULL AUTO_INCREMENT,
    libelle      VARCHAR(200) NOT NULL COMMENT 'Libelle de la tache.',
    charge_utile TEXT         NULL     COMMENT 'Donnees associees (facultatif).',
    etat         VARCHAR(20)  NOT NULL DEFAULT 'en_attente' COMMENT 'en_attente / traitee.',
    cree_le      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de creation.',
    traitee_le   DATETIME     NULL     COMMENT 'Date de traitement.',
    PRIMARY KEY (id),
    KEY ix_tache_etat (etat)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='File d''attente de taches.';

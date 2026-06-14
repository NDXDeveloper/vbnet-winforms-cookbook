-- ============================================================================
--  Journal - Schema de la base de demonstration            (MariaDB 12.3 LTS)
--  Projet 03 - Journalisation
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
--
--  Une seule table : les entrees de journal ecrites par le puits base de donnees.
--  Le niveau est stocke en numerique (pour filtrer >= seuil) ET en libelle lisible.
-- ============================================================================

USE journal;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS entree_journal (
    id              INT          NOT NULL AUTO_INCREMENT,
    survenu_le      DATETIME(3)  NOT NULL DEFAULT CURRENT_TIMESTAMP(3) COMMENT 'Horodatage (ms).',
    niveau          TINYINT      NOT NULL COMMENT 'Gravite numerique 0..4 (filtrage par seuil).',
    niveau_libelle  VARCHAR(10)  NOT NULL COMMENT 'Libelle lisible (DEBUG, INFO, ...).',
    categorie       VARCHAR(80)  NOT NULL DEFAULT '' COMMENT 'Categorie fonctionnelle.',
    message         TEXT         NOT NULL COMMENT 'Message journalise.',
    exception       TEXT         NULL     COMMENT 'Texte de l''exception associee.',
    machine         VARCHAR(100) NULL     COMMENT 'Machine a l''origine.',
    PRIMARY KEY (id),
    KEY ix_journal_niveau (niveau),
    KEY ix_journal_date (survenu_le)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Entrees de journal applicatif.';

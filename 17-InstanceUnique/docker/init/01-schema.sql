-- ============================================================================
--  Commandes - Schema (projet 17 - Instance unique & IPC)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE commandes;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS commande_recue (
    id        INT UNSIGNED NOT NULL AUTO_INCREMENT,
    source    VARCHAR(80)  NOT NULL COMMENT 'Origine de la commande (ex. instance-2).',
    arguments TEXT         NOT NULL COMMENT 'Arguments transmis.',
    recu_le   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de reception.',
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Journal des commandes recues par l''instance primaire.';

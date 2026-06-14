-- ============================================================================
--  Metadonnees - Schema (projet 18 - Reflexion)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE metadonnees;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS descripteur (
    id            INT UNSIGNED NOT NULL AUTO_INCREMENT,
    type_complet  VARCHAR(300) NOT NULL COMMENT 'Nom complet du type inspecte.',
    membre        VARCHAR(150) NOT NULL COMMENT 'Nom du membre.',
    genre         VARCHAR(20)  NOT NULL COMMENT 'Propriete / Champ / Evenement.',
    type_associe  VARCHAR(150) NOT NULL COMMENT 'Type associe au membre.',
    releve_le     DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date du releve.',
    PRIMARY KEY (id),
    KEY ix_descripteur_type (type_complet)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Catalogue des membres decouverts par reflexion.';

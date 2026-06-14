-- ============================================================================
--  Mesures - Schema (projet 26 - Graphiques de donnees)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE mesures;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS mesure (
    id      INT UNSIGNED NOT NULL AUTO_INCREMENT,
    libelle VARCHAR(60)  NOT NULL COMMENT 'Etiquette de l''axe (ex. mois).',
    valeur  DOUBLE       NOT NULL COMMENT 'Valeur mesuree.',
    ordre   INT          NOT NULL DEFAULT 0 COMMENT 'Ordre d''affichage.',
    PRIMARY KEY (id),
    KEY ix_mesure_ordre (ordre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Serie de mesures a tracer.';

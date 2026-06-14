-- ============================================================================
--  Themes - Schema (projet 20 - FormBase / themes)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE themes;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS theme (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom         VARCHAR(60)  NOT NULL COMMENT 'Nom du theme.',
    fond_hex    CHAR(7)      NOT NULL COMMENT 'Couleur de fond (#RRGGBB).',
    texte_hex   CHAR(7)      NOT NULL COMMENT 'Couleur de texte (#RRGGBB).',
    accent_hex  CHAR(7)      NOT NULL COMMENT 'Couleur d''accent (#RRGGBB).',
    maj_le      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                COMMENT 'Derniere mise a jour.',
    PRIMARY KEY (id),
    UNIQUE KEY uq_theme_nom (nom)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Themes visuels (couleurs en hexadecimal).';

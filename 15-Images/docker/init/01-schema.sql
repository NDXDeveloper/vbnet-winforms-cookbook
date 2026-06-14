-- ============================================================================
--  Mediatheque - Schema (projet 15 - Traitement d'images)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE mediatheque;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS image (
    id        INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nom       VARCHAR(255) NOT NULL COMMENT 'Nom du fichier image.',
    largeur   INT          NOT NULL COMMENT 'Largeur de l''original (px).',
    hauteur   INT          NOT NULL COMMENT 'Hauteur de l''original (px).',
    vignette  LONGBLOB     NOT NULL COMMENT 'Vignette PNG (image reduite).',
    cree_le   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date d''ajout.',
    PRIMARY KEY (id),
    KEY ix_image_nom (nom)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Mediatheque : metadonnees + vignettes.';

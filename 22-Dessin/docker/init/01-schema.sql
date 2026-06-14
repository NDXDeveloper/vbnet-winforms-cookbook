-- ============================================================================
--  Atelier - Schema (projet 22 - Editeur de formes)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE atelier;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS forme (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    type        TINYINT      NOT NULL COMMENT '0=Rectangle, 1=Ellipse, 2=Ligne.',
    x           INT          NOT NULL,
    y           INT          NOT NULL,
    largeur     INT          NOT NULL,
    hauteur     INT          NOT NULL,
    couleur_hex CHAR(7)      NOT NULL DEFAULT '#1565C0' COMMENT 'Couleur (#RRGGBB).',
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Formes d''une scene de dessin.';

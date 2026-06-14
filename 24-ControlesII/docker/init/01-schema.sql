-- ============================================================================
--  Inventaire - Schema (projet 24 - Controles II)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE inventaire;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS article (
    id          INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    reference   VARCHAR(40)   NOT NULL COMMENT 'Reference unique.',
    designation VARCHAR(150)  NOT NULL COMMENT 'Designation.',
    prix        DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT 'Prix unitaire.',
    stock       INT           NOT NULL DEFAULT 0 COMMENT 'Quantite en stock.',
    PRIMARY KEY (id),
    UNIQUE KEY uq_article_reference (reference)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Articles affiches dans la grille.';

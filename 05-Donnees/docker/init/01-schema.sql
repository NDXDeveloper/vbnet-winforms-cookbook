-- ============================================================================
--  Boutique - Schema (projet 05 - Acces aux donnees)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE boutique;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS produit (
    id          INT           NOT NULL AUTO_INCREMENT,
    reference   VARCHAR(40)   NOT NULL COMMENT 'Reference unique.',
    designation VARCHAR(150)  NOT NULL COMMENT 'Libelle commercial.',
    prix_ht     DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT 'Prix unitaire HT.',
    stock       INT           NOT NULL DEFAULT 0 COMMENT 'Quantite en stock.',
    cree_le     DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de creation.',
    PRIMARY KEY (id),
    UNIQUE KEY uq_produit_reference (reference)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Catalogue de produits.';

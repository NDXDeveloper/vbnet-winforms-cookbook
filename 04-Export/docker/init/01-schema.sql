-- ============================================================================
--  Entrepot - Schema de la base de demonstration (projet 04 - Export)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
--  Une table plate "vente" : un jeu de donnees ideal a exporter.
-- ============================================================================

USE entrepot;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS vente (
    id          INT           NOT NULL AUTO_INCREMENT,
    date_vente  DATE          NOT NULL COMMENT 'Date de la vente.',
    produit     VARCHAR(120)  NOT NULL COMMENT 'Produit vendu.',
    categorie   VARCHAR(60)   NOT NULL COMMENT 'Categorie du produit.',
    quantite    INT           NOT NULL DEFAULT 0 COMMENT 'Quantite vendue.',
    montant     DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT 'Montant total HT.',
    PRIMARY KEY (id),
    KEY ix_vente_categorie (categorie),
    KEY ix_vente_date (date_vente)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Ventes (jeu de donnees a exporter).';

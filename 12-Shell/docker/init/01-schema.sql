-- ============================================================================
--  Catalogue de liens - Schema (projet 12 - Interop Shell)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE catalogue_liens;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS lien (
    id          INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    categorie   VARCHAR(20)   NOT NULL COMMENT 'application (.lnk) ou web (.url).',
    nom         VARCHAR(150)  NOT NULL COMMENT 'Nom affiche / nom de fichier du raccourci.',
    cible       VARCHAR(500)  NOT NULL COMMENT 'Chemin de l''executable ou URL.',
    description VARCHAR(255)  NULL     COMMENT 'Description facultative.',
    cree_le     DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de creation.',
    PRIMARY KEY (id),
    KEY ix_lien_categorie (categorie)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Catalogue de liens / raccourcis.';

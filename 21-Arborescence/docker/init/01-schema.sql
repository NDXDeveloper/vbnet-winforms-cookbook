-- ============================================================================
--  Arborescence - Schema (projet 21 - Donnees hierarchiques)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE arborescence;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS noeud (
    id         INT UNSIGNED NOT NULL AUTO_INCREMENT,
    parent_id  INT UNSIGNED NULL     COMMENT 'Parent (NULL = racine) - liste d''adjacence.',
    libelle    VARCHAR(150) NOT NULL COMMENT 'Libelle du noeud.',
    categorie  VARCHAR(40)  NOT NULL DEFAULT '' COMMENT 'Categorie (dossier, fichier...).',
    PRIMARY KEY (id),
    KEY ix_noeud_parent (parent_id),
    CONSTRAINT fk_noeud_parent FOREIGN KEY (parent_id) REFERENCES noeud (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Arbre stocke a plat (parent_id).';

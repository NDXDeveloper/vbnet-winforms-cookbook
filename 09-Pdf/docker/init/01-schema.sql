-- ============================================================================
--  Bibliotheque - Schema (projet 09 - Composition de PDF)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE bibliotheque;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS document_genere (
    id               INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    titre            VARCHAR(255)  NOT NULL COMMENT 'Titre du document.',
    auteur           VARCHAR(150)  NOT NULL COMMENT 'Auteur du document.',
    nb_pages         INT UNSIGNED  NOT NULL DEFAULT 1 COMMENT 'Nombre de pages.',
    taille_octets    INT UNSIGNED  NOT NULL COMMENT 'Taille du PDF en octets.',
    contenu          LONGBLOB      NOT NULL COMMENT 'Binaire PDF complet.',
    empreinte_sha256 CHAR(64)      NOT NULL COMMENT 'SHA-256 hex du binaire.',
    cree_le          DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de creation.',
    PRIMARY KEY (id),
    KEY ix_document_titre (titre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Bibliotheque des documents PDF generes.';

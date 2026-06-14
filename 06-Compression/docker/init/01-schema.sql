-- ============================================================================
--  Archive - Schema (projet 06 - Compression)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE archive;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS archive (
    id                INT UNSIGNED  NOT NULL AUTO_INCREMENT,
    nom               VARCHAR(255)  NOT NULL COMMENT 'Nom logique de l''archive.',
    algorithme        VARCHAR(10)   NOT NULL COMMENT 'Algorithme : gzip ou deflate.',
    taille_originale  INT UNSIGNED  NOT NULL COMMENT 'Taille en octets avant compression.',
    taille_compressee INT UNSIGNED  NOT NULL COMMENT 'Taille en octets apres compression.',
    contenu           LONGBLOB      NOT NULL COMMENT 'Donnees compressees (binaire).',
    empreinte_sha256  CHAR(64)      NOT NULL COMMENT 'SHA-256 hex des donnees ORIGINALES (controle d''integrite).',
    cree_le           DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de creation.',
    PRIMARY KEY (id),
    KEY ix_archive_nom (nom)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Archives compressees (compress-then-store).';

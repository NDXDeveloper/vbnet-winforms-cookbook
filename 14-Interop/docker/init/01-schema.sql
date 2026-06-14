-- ============================================================================
--  Supervision - Schema (projet 14 - Interop Win32)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE supervision;
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS echantillon (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    objets_gdi  INT          NOT NULL COMMENT 'Nombre d''objets GDI au moment du releve.',
    objets_user INT          NOT NULL COMMENT 'Nombre d''objets USER au moment du releve.',
    inactif_ms  BIGINT       NOT NULL COMMENT 'Duree d''inactivite en millisecondes.',
    cree_le     DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Horodatage du releve.',
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Releves de ressources et d''inactivite.';

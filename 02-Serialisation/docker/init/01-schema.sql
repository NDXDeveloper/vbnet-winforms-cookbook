-- ============================================================================
--  Coffre - Schema de la base de demonstration            (MariaDB 12.3 LTS)
-- ----------------------------------------------------------------------------
--  Projet 02 - Serialisation
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
--
--  Domaine : un "coffre de documents". Chaque document conserve la forme
--  serialisee d'un objet (XML, contrat XML, binaire ou JSON), accompagnee de
--  metadonnees : type de l'objet, format, taille et empreinte SHA-256 (controle
--  d'integrite). Les categories servent a classer les documents.
--
--  Le payload est stocke en LONGBLOB : ce type accepte indifferemment du texte
--  (XML/JSON, encode en UTF-8) et du binaire compact.
--
--  Ce script est execute automatiquement par l'image MariaDB au premier
--  demarrage (volume vide). Le compte applicatif est cree par l'image a partir
--  des variables MARIADB_USER / MARIADB_PASSWORD.
-- ============================================================================

USE coffre;

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------------------------------------------------------
--  categorie : classement logique des documents.
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS categorie (
    id       INT          NOT NULL AUTO_INCREMENT,
    code     VARCHAR(20)  NOT NULL COMMENT 'Code court unique (ex : CATALOGUE).',
    libelle  VARCHAR(120) NOT NULL COMMENT 'Libelle affichable.',
    PRIMARY KEY (id),
    UNIQUE KEY uq_categorie_code (code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Categories de documents.';

-- ----------------------------------------------------------------------------
--  document : un objet serialise + ses metadonnees.
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS document (
    id                INT           NOT NULL AUTO_INCREMENT,
    libelle           VARCHAR(160)  NOT NULL COMMENT 'Nom lisible du document.',
    type_clr          VARCHAR(255)  NOT NULL COMMENT 'Type .NET de l''objet serialise.',
    format            ENUM('xml','contrat_xml','binaire','json')
                                    NOT NULL COMMENT 'Format de serialisation employe.',
    contenu           LONGBLOB      NOT NULL COMMENT 'Charge utile serialisee (texte UTF-8 ou binaire).',
    taille_octets     INT           NOT NULL DEFAULT 0 COMMENT 'Taille du payload en octets.',
    empreinte_sha256  CHAR(64)      NULL     COMMENT 'Empreinte SHA-256 du payload (controle d''integrite).',
    fk_categorie      INT           NULL     COMMENT 'Categorie de rattachement.',
    cree_le           DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date d''enregistrement.',
    PRIMARY KEY (id),
    KEY ix_document_categorie (fk_categorie),
    KEY ix_document_format (format),
    CONSTRAINT fk_document_categorie FOREIGN KEY (fk_categorie)
        REFERENCES categorie (id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Documents serialises et leurs metadonnees.';

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================================
--  Etabli - Schema de la base de demonstration            (MariaDB 12.3 LTS)
-- ----------------------------------------------------------------------------
--  Projet 01 - Boite a outils
--
--  Domaine : gestion de commandes d'atelier. Le schema met en jeu une hierarchie
--  relationnelle (secteurs -> clients -> commandes -> lignes), un catalogue
--  d'articles, des fiches techniques (colonnes calculees) et un journal
--  d'incidents. Il sert de support aux techniques d'acces aux donnees du projet.
--
--  Ce script est execute automatiquement par l'image MariaDB au premier
--  demarrage (volume vide). Le compte applicatif 'etabli_app' est, lui, cree
--  par l'image a partir des variables MARIADB_USER / MARIADB_PASSWORD.
-- ============================================================================

USE etabli;

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------------------------------------------------------
--  secteur : zones commerciales.
--  Sert de table de reference et de cle d'agregation pour les statistiques.
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS secteur (
    id       INT          NOT NULL AUTO_INCREMENT,
    code     VARCHAR(10)  NOT NULL COMMENT 'Code court unique (ex : NORD, SUD).',
    libelle  VARCHAR(100) NOT NULL COMMENT 'Libelle affichable du secteur.',
    PRIMARY KEY (id),
    UNIQUE KEY uq_secteur_code (code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Secteurs commerciaux (table de reference).';

-- ----------------------------------------------------------------------------
--  client : clients rattaches a un secteur.
--  'cree_le' permet d'illustrer les filtres "depuis une date" des stats.
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS client (
    id              INT          NOT NULL AUTO_INCREMENT,
    raison_sociale  VARCHAR(150) NOT NULL COMMENT 'Nom commercial du client.',
    email           VARCHAR(150) NULL     COMMENT 'Adresse e-mail de contact.',
    actif           TINYINT(1)   NOT NULL DEFAULT 1 COMMENT '1 = client actif, 0 = archive.',
    fk_secteur      INT          NULL     COMMENT 'Secteur de rattachement.',
    cree_le         DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de creation de la fiche.',
    PRIMARY KEY (id),
    KEY ix_client_secteur (fk_secteur),
    CONSTRAINT fk_client_secteur FOREIGN KEY (fk_secteur)
        REFERENCES secteur (id) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Clients de l''atelier.';

-- ----------------------------------------------------------------------------
--  article : catalogue produit.
--  Porte le libelle et le prix de reference utilises dans les lignes.
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS article (
    id           INT           NOT NULL AUTO_INCREMENT,
    designation  VARCHAR(150)  NOT NULL COMMENT 'Designation commerciale.',
    unite        VARCHAR(10)   NOT NULL DEFAULT 'U' COMMENT 'Unite de vente (U, kg, m...).',
    prix_ref     DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT 'Prix de reference HT.',
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Catalogue des articles.';

-- ----------------------------------------------------------------------------
--  fiche_article : caracteristiques techniques d'un article.
--  Les colonnes numeriques permettent d'illustrer le CALCUL DE RATIOS
--  directement dans la requete SQL (cf. AccesDonnees.IndicateursFiche).
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS fiche_article (
    id                 INT           NOT NULL AUTO_INCREMENT,
    fk_article         INT           NOT NULL COMMENT 'Article decrit par cette fiche (relation 1-1).',
    poids_kg           DECIMAL(10,3) NOT NULL DEFAULT 0 COMMENT 'Poids unitaire en kg.',
    surface_m2         DECIMAL(10,3) NOT NULL DEFAULT 0 COMMENT 'Surface a traiter en m2.',
    rendement          DECIMAL(10,3) NOT NULL DEFAULT 0 COMMENT 'Rendement fabrication (h/tonne).',
    poids_accessoires  DECIMAL(10,3) NOT NULL DEFAULT 0 COMMENT 'Poids des accessoires en kg.',
    poids_chutes       DECIMAL(10,3) NOT NULL DEFAULT 0 COMMENT 'Poids des chutes en kg.',
    heures_montage     DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT 'Heures de montage estimees.',
    PRIMARY KEY (id),
    UNIQUE KEY uq_fiche_article (fk_article),
    CONSTRAINT fk_fiche_article FOREIGN KEY (fk_article)
        REFERENCES article (id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Fiches techniques des articles.';

-- ----------------------------------------------------------------------------
--  commande : entete de commande passee par un client.
--  'cree_le' alimente les filtres temporels des statistiques.
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS commande (
    id         INT          NOT NULL AUTO_INCREMENT,
    reference  VARCHAR(30)  NOT NULL COMMENT 'Reference lisible (ex : CMD-2026-0001).',
    fk_client  INT          NOT NULL COMMENT 'Client donneur d''ordre.',
    statut     ENUM('brouillon','confirmee','expediee','annulee')
                            NOT NULL DEFAULT 'brouillon' COMMENT 'Cycle de vie de la commande.',
    cree_le    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Date de creation.',
    PRIMARY KEY (id),
    UNIQUE KEY uq_commande_reference (reference),
    KEY ix_commande_client (fk_client),
    CONSTRAINT fk_commande_client FOREIGN KEY (fk_client)
        REFERENCES client (id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Entetes de commande.';

-- ----------------------------------------------------------------------------
--  ligne_commande : lignes de detail.
--  quantite x prix_unitaire => illustre les agregats SUM()/COUNT().
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS ligne_commande (
    id             INT           NOT NULL AUTO_INCREMENT,
    fk_commande    INT           NOT NULL COMMENT 'Commande de rattachement.',
    fk_article     INT           NOT NULL COMMENT 'Article commande.',
    quantite       DECIMAL(10,3) NOT NULL DEFAULT 0 COMMENT 'Quantite commandee.',
    prix_unitaire  DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT 'Prix unitaire applique (peut differer du prix_ref).',
    PRIMARY KEY (id),
    KEY ix_ligne_commande (fk_commande),
    KEY ix_ligne_article (fk_article),
    CONSTRAINT fk_ligne_commande FOREIGN KEY (fk_commande)
        REFERENCES commande (id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_ligne_article FOREIGN KEY (fk_article)
        REFERENCES article (id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Lignes de detail des commandes.';

-- ----------------------------------------------------------------------------
--  journal_erreur : journal applicatif des exceptions.
--  Cible de la technique "INSERT puis recuperation de LAST_INSERT_ID()".
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS journal_erreur (
    id          INT       NOT NULL AUTO_INCREMENT,
    message     TEXT      NOT NULL COMMENT 'Rapport d''exception complet.',
    survenu_le  DATETIME  NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Horodatage de l''incident.',
    par_qui     VARCHAR(100) NULL COMMENT 'Compte utilisateur / machine a l''origine.',
    PRIMARY KEY (id),
    KEY ix_journal_date (survenu_le)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Journal applicatif des exceptions.';

SET FOREIGN_KEY_CHECKS = 1;

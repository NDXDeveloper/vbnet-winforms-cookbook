-- ============================================================================
--  Etabli - Jeu de donnees de demonstration               (MariaDB 12.3 LTS)
-- ----------------------------------------------------------------------------
--  Donnees fictives, deterministes et datees (de septembre 2025 a juin 2026)
--  afin que les statistiques filtrees "depuis une date" renvoient des
--  resultats parlants. Aucune donnee reelle d'entreprise.
-- ============================================================================

USE etabli;

-- --- Secteurs ----------------------------------------------------------------
INSERT INTO secteur (id, code, libelle) VALUES
    (1, 'NORD', 'Secteur Nord'),
    (2, 'SUD',  'Secteur Sud'),
    (3, 'EST',  'Secteur Est');

-- --- Clients (le client 9 est volontairement inactif) ------------------------
INSERT INTO client (id, raison_sociale, email, actif, fk_secteur, cree_le) VALUES
    (1,  'Charpentes du Bearn',   'contact@charpentes-bearn.fr', 1, 1, '2025-09-12 09:30:00'),
    (2,  'Metallerie Pyreneenne', 'accueil@metal-pyr.fr',        1, 1, '2025-10-03 14:05:00'),
    (3,  'Ateliers Gave SARL',    'devis@ateliers-gave.fr',      1, 2, '2025-11-20 11:20:00'),
    (4,  'Construction Aspe',     'info@construction-aspe.fr',   1, 2, '2026-01-15 08:45:00'),
    (5,  'Ossatures Pyrenees',       'contact@ossatures-pyr.fr',  1, 3, '2026-02-01 16:10:00'),
    (6,  'Soudure Express',       'atelier@soudure-express.fr',  1, 3, '2026-02-18 10:00:00'),
    (7,  'Bati Sud Ouest',        'commande@bati-so.fr',         1, 2, '2026-03-05 13:30:00'),
    (8,  'Inox et Compagnie',     'contact@inox-co.fr',          1, 1, '2026-03-22 09:15:00'),
    (9,  'Charpente Moderne',     'hello@charpente-moderne.fr',  0, 3, '2025-08-01 09:00:00'),
    (10, 'Forge Atlantique',      'forge@atlantique.fr',         1, 2, '2026-04-10 15:40:00');

-- --- Articles (catalogue) ----------------------------------------------------
INSERT INTO article (id, designation, unite, prix_ref) VALUES
    (1,  'Poutre IPE 200',        'U',  145.00),
    (2,  'Tube carre 40x40',      'm',   12.50),
    (3,  'Tole acier 2mm',        'm',   23.90),
    (4,  'Boulon HR M16',         'U',    1.20),
    (5,  'Platine 200x200',       'U',   18.75),
    (6,  'Corniere 50x50',        'm',    9.40),
    (7,  'Profil UPN 100',        'm',   21.30),
    (8,  'Goujon soudable',       'U',    0.85),
    (9,  'Peinture antirouille',  'kg',  14.60),
    (10, 'Disque a tronconner',   'U',    3.20),
    (11, 'Electrode inox',        'kg',  28.00),
    (12, 'Plaque alveolaire',     'U',   56.40),
    (13, 'Visserie inox M8',      'U',    0.45),
    (14, 'Equerre renforcee',     'U',    7.90),
    (15, 'Profil HEA 160',        'm',   38.10);

-- --- Fiches techniques (1 par article) ---------------------------------------
INSERT INTO fiche_article
    (fk_article, poids_kg, surface_m2, rendement, poids_accessoires, poids_chutes, heures_montage) VALUES
    (1,  224.000, 3.200, 12.500, 8.000, 11.000, 4.50),
    (2,    4.400, 0.160,  8.000, 0.200,  0.300, 0.50),
    (3,   15.700, 1.000,  6.000, 0.000,  1.200, 0.80),
    (4,    0.080, 0.000,  0.000, 0.000,  0.000, 0.00),
    (5,    6.300, 0.040,  9.000, 0.500,  0.400, 0.30),
    (6,    3.060, 0.100,  7.500, 0.100,  0.200, 0.40),
    (7,   10.600, 0.340, 11.000, 0.600,  0.700, 1.20),
    (8,    0.050, 0.000,  0.000, 0.000,  0.000, 0.10),
    (9,    1.000, 0.000,  0.000, 0.000,  0.000, 0.00),
    (10,   0.150, 0.000,  0.000, 0.000,  0.000, 0.00),
    (11,   1.000, 0.000,  0.000, 0.000,  0.000, 0.00),
    (12,   8.500, 2.000,  5.000, 0.300,  0.500, 1.00),
    (13,   0.020, 0.000,  0.000, 0.000,  0.000, 0.00),
    (14,   0.900, 0.020,  6.000, 0.050,  0.060, 0.20),
    (15,  30.400, 0.900, 13.000, 1.500,  2.000, 2.50);

-- --- Commandes ---------------------------------------------------------------
INSERT INTO commande (id, reference, fk_client, statut, cree_le) VALUES
    (1,  'CMD-2025-0001', 1,  'expediee',  '2025-09-20 10:00:00'),
    (2,  'CMD-2025-0002', 1,  'expediee',  '2025-10-10 11:30:00'),
    (3,  'CMD-2025-0003', 2,  'confirmee', '2025-10-15 09:45:00'),
    (4,  'CMD-2025-0004', 3,  'expediee',  '2025-12-01 14:20:00'),
    (5,  'CMD-2026-0001', 4,  'confirmee', '2026-01-20 08:30:00'),
    (6,  'CMD-2026-0002', 5,  'brouillon', '2026-02-05 16:00:00'),
    (7,  'CMD-2026-0003', 2,  'expediee',  '2026-02-12 10:15:00'),
    (8,  'CMD-2026-0004', 6,  'confirmee', '2026-02-25 13:00:00'),
    (9,  'CMD-2026-0005', 7,  'confirmee', '2026-03-10 09:00:00'),
    (10, 'CMD-2026-0006', 8,  'brouillon', '2026-03-25 15:30:00'),
    (11, 'CMD-2026-0007', 1,  'confirmee', '2026-04-02 11:00:00'),
    (12, 'CMD-2026-0008', 10, 'expediee',  '2026-04-15 14:45:00'),
    (13, 'CMD-2026-0009', 3,  'confirmee', '2026-05-01 08:50:00'),
    (14, 'CMD-2026-0010', 5,  'brouillon', '2026-05-12 10:30:00'),
    (15, 'CMD-2026-0011', 7,  'annulee',   '2026-05-20 09:20:00'),
    (16, 'CMD-2026-0012', 2,  'confirmee', '2026-06-01 16:40:00'),
    (17, 'CMD-2026-0013', 4,  'brouillon', '2026-06-08 09:10:00'),
    (18, 'CMD-2026-0014', 8,  'confirmee', '2026-06-10 13:25:00');

-- --- Lignes de commande ------------------------------------------------------
--  (quantite x prix_unitaire ; le prix_unitaire peut differer du prix_ref)
INSERT INTO ligne_commande (fk_commande, fk_article, quantite, prix_unitaire) VALUES
    (1,  1,  12.000, 145.00), (1,  4, 120.000,   1.15), (1,  9,  8.000,  14.60),
    (2,  2,  60.000,  12.50), (2,  6,  40.000,   9.20), (2, 10, 15.000,   3.20),
    (3,  3,  25.000,  23.90), (3,  5,  30.000,  18.50), (3, 13, 200.000,  0.42),
    (4,  7,  18.000,  21.30), (4,  1,   6.000, 142.00), (4,  4, 80.000,   1.20),
    (5, 15,  10.000,  38.10), (5, 12,   5.000,  56.40), (5, 11,  4.000,  28.00),
    (6,  2,  35.000,  12.30), (6, 14,  50.000,   7.90),
    (7,  1,   8.000, 145.00), (7,  3,  12.000,  23.50), (7,  9,  6.000,  14.60), (7,  4, 60.000, 1.18),
    (8,  6,  22.000,   9.40), (8,  8, 150.000,   0.85),
    (9,  5,  16.000,  18.75), (9,  7,  10.000,  21.00), (9, 10, 20.000,   3.10),
    (10, 12,  3.000,  56.40), (10, 13, 120.000,  0.45),
    (11, 1,  15.000, 144.50), (11, 2,  45.000,  12.50), (11, 4, 100.000,  1.10),
    (12, 15,  6.000,  38.10), (12, 11,  2.000,  27.50),
    (13, 3,  30.000,  23.90), (13, 6,  18.000,   9.30), (13, 14, 40.000,  7.80),
    (14, 7,   9.000,  21.30), (14, 5,  12.000,  18.75),
    (15, 2,  20.000,  12.50),
    (16, 1,  10.000, 145.00), (16, 9,  5.000,   14.40), (16, 12, 4.000,  56.00),
    (17, 4,  90.000,   1.20), (17, 8, 100.000,   0.82),
    (18, 15,  7.000,  38.00), (18, 3,  14.000,  23.90), (18, 10, 10.000,  3.20);

-- --- Journal d'erreur (exemples) ---------------------------------------------
INSERT INTO journal_erreur (message, survenu_le, par_qui) VALUES
    ('[2026-01-05 10:12:00] Exemple : timeout de connexion lors d''un import.', '2026-01-05 10:12:00', 'demo@etabli'),
    ('[2026-03-18 15:48:00] Exemple : violation de contrainte d''unicite sur commande.reference.', '2026-03-18 15:48:00', 'demo@etabli');

-- ============================================================================
--  Boutique - Jeu de donnees de demonstration (produits fictifs).
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE boutique;

INSERT INTO produit (reference, designation, prix_ht, stock) VALUES
    ('OUT-001', 'Cle a molette 250 mm',        12.90,  25),
    ('OUT-002', 'Tournevis cruciforme PH2',     4.50, 120),
    ('OUT-003', 'Niveau a bulle 40 cm',        18.75,  12),
    ('OUT-004', 'Marteau arrache-clou',        12.95,  18),
    ('OUT-005', 'Scie egoine 500 mm',           8.95,  22),
    ('OUT-006', 'Perceuse visseuse 18V',       74.99,  15),
    ('QUI-001', 'Boite de vis 4x40 (x200)',     2.00,  80),
    ('QUI-002', 'Boulons HR M16 (x100)',       12.00,  14),
    ('QUI-003', 'Cheville a frapper (x100)',    3.00,  60),
    ('QUI-004', 'Equerre de fixation',          2.99, 120),
    ('PEI-001', 'Peinture antirouille 1L',     14.60,  40),
    ('PEI-002', 'Rouleau + bac',                7.00,  35),
    ('ELE-001', 'Disjoncteur 16A',              9.00,  60),
    ('ELE-002', 'Cable 3G2.5 (au metre)',       1.30, 500),
    ('ELE-003', 'Prise murale',                 2.10, 220);

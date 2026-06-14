-- ============================================================================
--  Entrepot - Jeu de donnees de demonstration (ventes fictives).
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE entrepot;

INSERT INTO vente (date_vente, produit, categorie, quantite, montant) VALUES
    ('2026-01-05', 'Cle a molette',          'Outillage',     25, 322.50),
    ('2026-01-08', 'Tournevis cruciforme',   'Outillage',    120, 540.00),
    ('2026-01-12', 'Niveau a bulle 40 cm',   'Outillage',     12, 225.00),
    ('2026-01-15', 'Boite de vis 4x40',      'Quincaillerie', 80, 160.00),
    ('2026-01-18', 'Boulons HR M16 (x100)',  'Quincaillerie', 14, 168.00),
    ('2026-01-22', 'Cheville a frapper',     'Quincaillerie',300, 90.00),
    ('2026-01-26', 'Peinture antirouille 1L','Peinture',      40, 584.00),
    ('2026-02-02', 'Rouleau + bac',          'Peinture',      35, 245.00),
    ('2026-02-05', 'Ruban de masquage',      'Peinture',     150, 187.50),
    ('2026-02-09', 'Disjoncteur 16A',        'Electricite',   60, 540.00),
    ('2026-02-13', 'Cable 3G2.5 (m)',        'Electricite',  500, 650.00),
    ('2026-02-17', 'Prise murale',           'Electricite',  220, 462.00),
    ('2026-02-21', 'Marteau arrache-clou',   'Outillage',     18, 233.10),
    ('2026-02-24', 'Scie egoine',            'Outillage',     22, 196.90),
    ('2026-03-01', 'Equerre de fixation',    'Quincaillerie',120, 358.80),
    ('2026-03-05', 'Vernis incolore 0.5L',   'Peinture',      28, 251.72),
    ('2026-03-09', 'Gaine ICTA 20 (m)',      'Electricite',  400, 240.00),
    ('2026-03-13', 'Perceuse visseuse',      'Outillage',     15, 1124.85),
    ('2026-03-17', 'Lot de forets (x10)',    'Outillage',     65, 422.50),
    ('2026-03-20', 'Interrupteur va-et-vient','Electricite',  90, 198.00);

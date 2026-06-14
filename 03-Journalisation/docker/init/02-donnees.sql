-- ============================================================================
--  Journal - Quelques entrees de demonstration.
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
--  (L'application ajoute ses propres entrees via le puits base.)
-- ============================================================================

USE journal;

INSERT INTO entree_journal (survenu_le, niveau, niveau_libelle, categorie, message, exception, machine) VALUES
    ('2026-02-01 08:00:00.000', 1, 'INFO',    'Demarrage', 'Application demarree.', NULL, 'demo'),
    ('2026-02-01 08:00:01.250', 0, 'DEBUG',   'Config',    'Configuration chargee (3 cles).', NULL, 'demo'),
    ('2026-02-01 08:05:12.500', 2, 'AVERT',   'Reseau',    'Latence elevee detectee (820 ms).', NULL, 'demo'),
    ('2026-02-01 08:06:00.000', 3, 'ERREUR',  'BDD',       'Echec d''une requete.', 'System.TimeoutException: delai depasse.', 'demo');

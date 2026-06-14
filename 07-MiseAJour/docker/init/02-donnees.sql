-- ============================================================================
--  Deploiement - Donnees initiales (projet 07 - Mise a jour applicative)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE deploiement;

INSERT INTO publication (version, notes, url_paquet, empreinte_sha256, obligatoire, publiee_le) VALUES
 ('1.0.0', 'Version initiale.',                              'https://exemple.invalide/paquets/app-1.0.0.zip', NULL, 0, '2025-09-01 09:00:00'),
 ('1.1.0', 'Améliorations de performance.',                  'https://exemple.invalide/paquets/app-1.1.0.zip', NULL, 0, '2025-10-12 09:00:00'),
 ('1.2.0', 'Nouvelle page de préférences.',                  'https://exemple.invalide/paquets/app-1.2.0.zip', NULL, 0, '2025-11-20 09:00:00'),
 ('1.3.0', 'Correctif de sécurité important.',               'https://exemple.invalide/paquets/app-1.3.0.zip', NULL, 1, '2026-01-15 09:00:00'),
 ('1.4.1', 'Corrections diverses.',                          'https://exemple.invalide/paquets/app-1.4.1.zip', NULL, 0, '2026-03-03 09:00:00'),
 ('1.10.0','Refonte de l''interface et nouvelles options.',  'https://exemple.invalide/paquets/app-1.10.0.zip',NULL, 0, '2026-05-28 09:00:00');

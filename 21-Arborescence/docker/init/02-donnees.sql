-- ============================================================================
--  Arborescence - Donnees initiales (projet 21 - Donnees hierarchiques)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE arborescence;

INSERT INTO noeud (id, parent_id, libelle, categorie) VALUES
 (1, NULL, 'Documents',  'dossier'),
 (2, 1,    'Projets',    'dossier'),
 (3, 1,    'Archives',   'dossier'),
 (4, 2,    'Projet A',   'dossier'),
 (5, 2,    'Projet B',   'dossier'),
 (6, 4,    'cahier.txt', 'fichier'),
 (7, 3,    'ancien.zip', 'fichier');

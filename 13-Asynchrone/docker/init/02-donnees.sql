-- ============================================================================
--  Traitements - Donnees initiales (projet 13 - Asynchrone)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================

USE traitements;

INSERT INTO tache (libelle, etat) VALUES
 ('Importer le fichier A', 'en_attente'),
 ('Recalculer les totaux', 'en_attente'),
 ('Générer le rapport',    'en_attente'),
 ('Envoyer les notifications', 'en_attente'),
 ('Nettoyer le cache',     'en_attente');

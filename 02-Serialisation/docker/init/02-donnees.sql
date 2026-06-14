-- ============================================================================
--  Coffre - Jeu de donnees de demonstration               (MariaDB 12.3 LTS)
-- ----------------------------------------------------------------------------
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
--
--  On amorce uniquement les categories. Les documents (objets serialises) sont
--  CREES PAR L'APPLICATION lors de la demonstration de persistance : c'est le
--  coeur de l'exercice (serialiser un objet puis l'enregistrer, le relire et le
--  deserialiser).
-- ============================================================================

USE coffre;

INSERT INTO categorie (id, code, libelle) VALUES
    (1, 'CATALOGUE', 'Catalogues produits'),
    (2, 'COMMANDE',  'Commandes clients'),
    (3, 'CONFIG',    'Fichiers de configuration'),
    (4, 'DIVERS',    'Documents divers');

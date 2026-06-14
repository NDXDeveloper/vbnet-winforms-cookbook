-- ============================================================================
--  Redaction - Donnees initiales (projet 23 - Editeur RTF)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
--  NB : les antislashs du RTF sont doubles (echappement de chaine SQL).
-- ============================================================================

USE redaction;

INSERT INTO document (titre, contenu_rtf) VALUES
 ('Bienvenue', '{\\rtf1\\ansi\\deff0 Bienvenue dans l''editeur. Tapez, mettez en forme, enregistrez.}');

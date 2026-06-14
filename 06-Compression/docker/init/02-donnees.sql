-- ============================================================================
--  Archive - Donnees initiales (projet 06 - Compression)
--  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT.
-- ============================================================================
--
--  La table demarre VOLONTAIREMENT VIDE.
--
--  Le principe « compress-then-store » veut que ce soit l'application qui
--  compresse les donnees (cote .NET, en GZip ou Deflate) puis les enregistre :
--  le contenu binaire valide ne peut donc pas etre amorce de maniere fiable en
--  pur SQL. Utilisez la page « Archives (base) » de la galerie pour creer,
--  lister et recharger des archives.
-- ============================================================================

USE archive;
-- (aucune insertion : population via l'application)

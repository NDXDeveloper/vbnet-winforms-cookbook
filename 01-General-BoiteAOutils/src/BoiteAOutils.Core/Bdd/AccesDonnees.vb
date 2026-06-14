Imports System.Data
Imports System.Reflection
Imports System.Text
Imports MySql.Data.MySqlClient

''' <summary>
''' Couche d'acces aux donnees : execution des commandes SQL et requetes
''' metier de demonstration.
''' </summary>
''' <remarks>
''' <para>
''' Trois methodes d'execution centrales (<c>ExecuteNonQuery</c>,
''' <c>ExecuteScalar</c>, <c>GetDTfromCommand</c>) embarquent une logique de
''' "seconde chance" : sur une erreur
''' transitoire (interbloquage InnoDB, ou <c>DataReader</c> reste ouvert sur la
''' connexion), la commande est rejouee apres regeneration de la connexion ou
''' courte temporisation, dans la limite d'un nombre d'essais.
''' </para>
''' <para>
''' Les requetes metier (indicateurs de fiche, lignes, montant global,
''' statistiques clients, verification d'articles) illustrent surtout l'emploi
''' systematique de <b>requetes parametrees</b> (<c>@parametre</c>), seule
''' protection fiable contre l'injection SQL.
''' </para>
''' </remarks>
Public Module AccesDonnees

    ' Nombre maximal de tentatives de "seconde chance".
    Private Const MAX_TENTATIVES As Integer = 3

#Region "Execution generique (avec seconde chance)"

    ''' <summary>
    ''' Execute une commande ne renvoyant pas de jeu de resultats (INSERT/UPDATE/DELETE).
    ''' </summary>
    ''' <returns>Le nombre de lignes affectees, ou -1 en cas d'echec definitif.</returns>
    Public Function ExecuteNonQuery(ByRef cmd As MySqlCommand,
                                    ByRef oMy As ConnexionMySql,
                                    ByVal methode As MethodBase,
                                    ByVal pileEnvironnement As String,
                                    Optional ByVal estUnReessai As Boolean = False,
                                    Optional ByVal numeroTentative As Integer = 0) As Integer
        Dim retour As Integer = -1
        EnregistrerAction(oMy, methode, pileEnvironnement)
        If cmd.Connection Is Nothing Then
            GestionExceptions.TraiterErreur(ConstruireRapportConnexionNulle(methode, oMy))
            Return retour
        End If

        Try
            retour = cmd.ExecuteNonQuery()
        Catch ex As Exception
            If DoitReessayer(ex, oMy, numeroTentative) Then
                RegenererEtReouvrir(cmd, oMy)
                Return ExecuteNonQuery(cmd, oMy, methode, pileEnvironnement, True, numeroTentative + 1)
            End If
            If oMy IsNot Nothing AndAlso oMy.Tr IsNot Nothing AndAlso numeroTentative > MAX_TENTATIVES Then
                oMy.Rollback()
                Return -1
            End If
            SignalerEchec(ex, oMy)
        End Try
        Return retour
    End Function

    ''' <summary>
    ''' Execute une commande renvoyant une valeur scalaire (premier champ de la premiere ligne).
    ''' </summary>
    ''' <returns>La valeur lue, ou -1 si aucune/echec.</returns>
    Public Function ExecuteScalar(ByRef cmd As MySqlCommand,
                                  ByRef oMy As ConnexionMySql,
                                  ByVal methode As MethodBase,
                                  ByVal pileEnvironnement As String,
                                  Optional ByVal estUnReessai As Boolean = False,
                                  Optional ByVal numeroTentative As Integer = 0) As Object
        Dim retour As Object = Nothing
        EnregistrerAction(oMy, methode, pileEnvironnement)
        If cmd.Connection Is Nothing Then
            GestionExceptions.TraiterErreur(ConstruireRapportConnexionNulle(methode, oMy))
            Return -1
        End If

        Try
            retour = cmd.ExecuteScalar()
        Catch ex As Exception
            If DoitReessayer(ex, oMy, numeroTentative) Then
                RegenererEtReouvrir(cmd, oMy)
                Return ExecuteScalar(cmd, oMy, methode, pileEnvironnement, True, numeroTentative + 1)
            End If
            If oMy IsNot Nothing AndAlso oMy.Tr IsNot Nothing AndAlso numeroTentative > MAX_TENTATIVES Then
                oMy.Rollback()
                Return -1
            End If
            SignalerEchec(ex, oMy)
        End Try

        If retour Is Nothing Then retour = -1
        Return retour
    End Function

    ''' <summary>
    ''' Execute une commande de selection et renvoie le resultat dans un <see cref="DataTable"/>.
    ''' </summary>
    Public Function GetDTfromCommand(ByRef cmd As MySqlCommand,
                                     ByVal duplicationUnique As Boolean,
                                     ByRef oMy As ConnexionMySql,
                                     ByVal methode As MethodBase,
                                     ByVal pileEnvironnement As String,
                                     Optional ByVal estUnReessai As Boolean = False,
                                     Optional ByVal numeroTentative As Integer = 0) As DataTable
        Dim retour As New DataTable()
        EnregistrerAction(oMy, methode, pileEnvironnement)
        If cmd.Connection Is Nothing Then
            GestionExceptions.TraiterErreur(ConstruireRapportConnexionNulle(methode, oMy))
            Return retour
        End If

        Try
            Using da As New MySqlDataAdapter(cmd)
                da.Fill(retour)
            End Using
        Catch ex As Exception
            ' "duplicationUnique" : autorise une seule passe, sans seconde chance.
            If Not duplicationUnique AndAlso DoitReessayer(ex, oMy, numeroTentative) Then
                RegenererEtReouvrir(cmd, oMy)
                Return GetDTfromCommand(cmd, duplicationUnique, oMy, methode, pileEnvironnement, True, numeroTentative + 1)
            End If
            If oMy IsNot Nothing AndAlso oMy.Tr IsNot Nothing AndAlso numeroTentative > MAX_TENTATIVES Then
                oMy.Rollback()
                Return Nothing
            End If
            SignalerEchec(ex, oMy)
        End Try
        Return retour
    End Function

#End Region

#Region "Outils internes de la seconde chance"

    ''' <summary>Renseigne l'historique d'actions de la connexion (cf. ConnexionMySql.Action).</summary>
    Private Sub EnregistrerAction(ByVal oMy As ConnexionMySql, ByVal methode As MethodBase, ByVal pile As String)
        If oMy Is Nothing OrElse methode Is Nothing Then Return
        oMy.Action = methode.DeclaringType.Name & "." & methode.Name & "() " &
                     DateTime.Now.ToString("HH:mm:ss.fff") & Environment.NewLine & pile
    End Sub

    ''' <summary>
    ''' Determine si une exception justifie un nouvel essai : interbloquage
    ''' InnoDB ou <c>DataReader</c> reste ouvert sur la connexion.
    ''' </summary>
    Private Function DoitReessayer(ByVal ex As Exception, ByVal oMy As ConnexionMySql, ByVal numeroTentative As Integer) As Boolean
        If oMy Is Nothing Then Return False
        If numeroTentative >= MAX_TENTATIVES Then Return False
        Dim m As String = If(ex.Message, "")
        Return m.Contains("Deadlock found when trying to get lock") OrElse
               m.Contains("There is already an open DataReader")
    End Function

    ''' <summary>Regenere la connexion, la rouvre et reaffecte la commande.</summary>
    Private Sub RegenererEtReouvrir(ByRef cmd As MySqlCommand, ByVal oMy As ConnexionMySql)
        Journal.Ecrire("Seconde chance : regeneration de la connexion ID " & oMy.Id, Journal.Niveau.Avertissement)
        oMy.RegenererConnexion()
        oMy.Open(MethodBase.GetCurrentMethod())
        cmd.Connection = oMy.Conn
        cmd.Transaction = oMy.Tr
    End Sub

    ''' <summary>Construit le rapport complet et le transmet au gestionnaire d'exceptions.</summary>
    Private Sub SignalerEchec(ByVal ex As Exception, ByVal oMy As ConnexionMySql)
        Dim exc As String = GestionExceptions.PreparerException(
            ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace)
        ' On annexe l'historique des dernieres actions de la connexion.
        If oMy IsNot Nothing Then
            exc &= Environment.NewLine & "######## Historique des actions ########"
            exc &= Environment.NewLine & "Action     : " & oMy.Action
            exc &= Environment.NewLine & "Action n-1 : " & oMy.ActionNM1
            exc &= Environment.NewLine & "Action n-2 : " & oMy.ActionNM2
            exc &= Environment.NewLine & "Action n-3 : " & oMy.ActionNM3
            exc &= Environment.NewLine & "Action n-4 : " & oMy.ActionNM4
            exc &= Environment.NewLine & "Action n-5 : " & oMy.ActionNM5
        End If
        GestionExceptions.TraiterException(exc)
    End Sub

    ''' <summary>Rapport specifique au cas "connexion de la commande a Nothing".</summary>
    Private Function ConstruireRapportConnexionNulle(ByVal methode As MethodBase, ByVal oMy As ConnexionMySql) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("[ " & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") & " ]")
        sb.AppendLine("[ cmd.Connection est Nothing ]")
        sb.AppendLine("[ Base : " & ConfigBdd.NomBase & " ]")
        If methode IsNot Nothing AndAlso methode.DeclaringType IsNot Nothing Then
            sb.AppendLine("Methode : " & methode.DeclaringType.Name & "." & methode.Name & "()")
        End If
        Return sb.ToString()
    End Function

#End Region

#Region "Service de connexion / diagnostic"

    ''' <summary>
    ''' Verifie que la base est joignable (ouverture + SELECT VERSION()).
    ''' </summary>
    ''' <param name="message">Message de diagnostic en sortie (version serveur ou erreur).</param>
    ''' <returns>True si la connexion fonctionne.</returns>
    Public Function TesterConnexion(ByRef message As String) As Boolean
        Try
            Using oMy As New ConnexionMySql(False, MethodBase.GetCurrentMethod())
                oMy.Open(MethodBase.GetCurrentMethod())
                Using cmd As New MySqlCommand("SELECT VERSION();", oMy.Conn, oMy.Tr)
                    Dim version As Object = cmd.ExecuteScalar()
                    message = "Connexion OK - serveur : " & Convert.ToString(version)
                    Return True
                End Using
            End Using
        Catch ex As Exception
            message = "Echec de connexion : " & ex.Message
            Return False
        End Try
    End Function

#End Region

#Region "Requetes metier de demonstration (requetes parametrees)"

    ''' <summary>
    ''' Indicateurs calcules d'une fiche article (poids, surface, heures de
    ''' fabrication, ratios accessoires/chutes...). Les calculs sont realises
    ''' cote serveur, dans la requete.
    ''' </summary>
    Public Function IndicateursFiche(ByVal idFiche As Integer) As DataTable
        Const requete As String =
            "SELECT " &
            "  f.poids_kg                                       AS `Poids (kg)`, " &
            "  f.surface_m2                                     AS `Surface (m2)`, " &
            "  ROUND(f.rendement * f.poids_kg / 1000, 2)        AS `Heures fab.`, " &
            "  f.rendement                                      AS `Rendement`, " &
            "  ROUND(f.poids_accessoires / NULLIF(f.poids_kg,0) * 100, 1) AS `% Accessoires`, " &
            "  ROUND(f.poids_chutes * 100 / NULLIF(f.poids_kg,0), 1)      AS `% Chutes`, " &
            "  f.heures_montage                                 AS `Heures montage` " &
            "FROM fiche_article f " &
            "WHERE f.id = @idFiche;"
        Return ExecuterRequetePtable(requete,
            Sub(cmd) cmd.Parameters.AddWithValue("@idFiche", idFiche))
    End Function

    ''' <summary>
    ''' Lignes de commande portant sur l'article decrit par une fiche.
    ''' Jointures sur plusieurs tables.
    ''' </summary>
    Public Function LignesParFiche(ByVal idFiche As Integer) As DataTable
        Const requete As String =
            "SELECT a.designation AS `Article`, lc.quantite AS `Quantite`, lc.prix_unitaire AS `Prix U.` " &
            "FROM ligne_commande lc " &
            "JOIN article a       ON a.id = lc.fk_article " &
            "JOIN fiche_article f ON f.fk_article = a.id " &
            "WHERE f.id = @idFiche;"
        Return ExecuterRequetePtable(requete,
            Sub(cmd) cmd.Parameters.AddWithValue("@idFiche", idFiche))
    End Function

    ''' <summary>
    ''' Nombre de lignes et montant total (somme de quantite x prix_unitaire)
    ''' pour l'article d'une fiche.
    ''' </summary>
    Public Function MontantGlobalParFiche(ByVal idFiche As Integer) As DataTable
        Const requete As String =
            "SELECT COUNT(*) AS `Nb lignes`, " &
            "  COALESCE(SUM(lc.quantite * lc.prix_unitaire), 0) AS `Montant global` " &
            "FROM ligne_commande lc " &
            "JOIN fiche_article f ON f.fk_article = lc.fk_article " &
            "WHERE f.id = @idFiche;"
        Return ExecuterRequetePtable(requete,
            Sub(cmd) cmd.Parameters.AddWithValue("@idFiche", idFiche))
    End Function

    ''' <summary>
    ''' Statistiques d'activite par client actif depuis une date donnee
    ''' (nb de commandes, nb de lignes, montant). Sous-requetes correlees et
    ''' jointure sur le secteur.
    ''' </summary>
    Public Function StatistiquesClients(ByVal depuis As DateTime) As DataTable
        Const requete As String =
            "SELECT " &
            "  c.raison_sociale AS `Client`, " &
            "  s.libelle        AS `Secteur`, " &
            "  (SELECT COUNT(*) FROM commande co " &
            "     WHERE co.fk_client = c.id AND co.cree_le >= @depuis) AS `Nb commandes`, " &
            "  (SELECT COUNT(*) FROM ligne_commande lc " &
            "     JOIN commande co ON co.id = lc.fk_commande " &
            "     WHERE co.fk_client = c.id AND co.cree_le >= @depuis) AS `Nb lignes`, " &
            "  (SELECT COALESCE(SUM(lc.quantite * lc.prix_unitaire), 0) FROM ligne_commande lc " &
            "     JOIN commande co ON co.id = lc.fk_commande " &
            "     WHERE co.fk_client = c.id AND co.cree_le >= @depuis) AS `Montant` " &
            "FROM client c " &
            "LEFT JOIN secteur s ON s.id = c.fk_secteur " &
            "WHERE c.actif = 1 " &
            "GROUP BY c.id " &
            "ORDER BY `Nb commandes` DESC, `Montant` DESC;"
        Return ExecuterRequetePtable(requete,
            Sub(cmd) cmd.Parameters.AddWithValue("@depuis", depuis))
    End Function

    ''' <summary>
    ''' Verifie que tous les identifiants d'articles fournis existent en base.
    ''' Les identifiants absents sont ajoutes a <paramref name="listeProblemes"/>.
    ''' </summary>
    ''' <returns>True si tous les articles existent.</returns>
    Public Function VerifierArticles(ByVal listeIds As List(Of Integer),
                                     ByRef listeProblemes As List(Of Integer)) As Boolean
        If listeProblemes Is Nothing Then listeProblemes = New List(Of Integer)()
        If listeIds Is Nothing OrElse listeIds.Count = 0 Then Return True

        ' La liste IN(...) est construite a partir d'ENTIERS valides : aucune
        ' donnee de type chaine n'y entre, donc pas de risque d'injection.
        Dim listeIn As String = String.Join(",", listeIds)
        Dim requete As String = "SELECT id FROM article WHERE id IN (" & listeIn & ");"

        Dim trouvees As New HashSet(Of Integer)()
        Try
            Using oMy As New ConnexionMySql(False, MethodBase.GetCurrentMethod())
                oMy.Open(MethodBase.GetCurrentMethod())
                Using cmd As New MySqlCommand(requete, oMy.Conn, oMy.Tr)
                    Dim dt As DataTable = GetDTfromCommand(cmd, False, oMy, MethodBase.GetCurrentMethod(), Environment.StackTrace)
                    If dt IsNot Nothing Then
                        For Each ligne As DataRow In dt.Rows
                            If Not IsDBNull(ligne("id")) Then trouvees.Add(Convert.ToInt32(ligne("id")))
                        Next
                    End If
                End Using
            End Using
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return False
        End Try

        For Each id As Integer In listeIds
            If Not trouvees.Contains(id) AndAlso Not listeProblemes.Contains(id) Then
                listeProblemes.Add(id)
            End If
        Next
        Return listeProblemes.Count = 0
    End Function

    ''' <summary>
    ''' Enregistre un rapport d'erreur dans la table <c>journal_erreur</c> et
    ''' renvoie l'identifiant cree (technique INSERT + LAST_INSERT_ID()).
    ''' </summary>
    ''' <remarks>
    ''' Methode <b>volontairement defensive</b> : en cas d'echec, elle ne
    ''' rappelle jamais <see cref="GestionExceptions.TraiterException"/> (qui
    ''' rappellerait LogErreur), afin d'eviter toute recursion.
    ''' </remarks>
    ''' <returns>L'identifiant insere, ou -1 en cas d'echec.</returns>
    Public Function LogErreur(ByVal message As String, ByVal parQui As String) As Integer
        Try
            Using oMy As New ConnexionMySql(False, MethodBase.GetCurrentMethod())
                oMy.Open(MethodBase.GetCurrentMethod())
                Const requete As String =
                    "INSERT INTO journal_erreur (message, survenu_le, par_qui) " &
                    "VALUES (@message, @date, @parQui); SELECT LAST_INSERT_ID();"
                Using cmd As New MySqlCommand(requete, oMy.Conn, oMy.Tr)
                    cmd.Parameters.AddWithValue("@message", message)
                    cmd.Parameters.AddWithValue("@date", DateTime.Now)
                    cmd.Parameters.AddWithValue("@parQui", parQui)
                    Dim idCree As Object = cmd.ExecuteScalar()
                    Return If(idCree Is Nothing OrElse IsDBNull(idCree), -1, Convert.ToInt32(idCree))
                End Using
            End Using
        Catch ex As Exception
            ' Pas de TraiterException ici : on se contente d'une trace locale.
            Debug.WriteLine("[AccesDonnees.LogErreur] echec : " & ex.Message)
            Return -1
        End Try
    End Function

    ''' <summary>
    ''' Fabrique commune aux requetes de selection : ouvre une connexion
    ''' ephemere, prepare la commande, applique les parametres puis delegue a
    ''' <see cref="GetDTfromCommand"/>.
    ''' </summary>
    Private Function ExecuterRequetePtable(ByVal requete As String,
                                           ByVal appliquerParametres As Action(Of MySqlCommand)) As DataTable
        Dim resultat As New DataTable()
        Try
            Using oMy As New ConnexionMySql(False, MethodBase.GetCurrentMethod())
                oMy.Open(MethodBase.GetCurrentMethod())
                Using cmd As New MySqlCommand(requete, oMy.Conn, oMy.Tr)
                    appliquerParametres?.Invoke(cmd)
                    resultat = GetDTfromCommand(cmd, False, oMy, MethodBase.GetCurrentMethod(), Environment.StackTrace)
                End Using
            End Using
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Try
        Return resultat
    End Function

#End Region

End Module

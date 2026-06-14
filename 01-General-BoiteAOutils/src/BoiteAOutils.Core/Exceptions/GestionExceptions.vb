Imports System.Diagnostics
Imports System.Reflection
Imports System.Text
Imports MySql.Data.MySqlClient

''' <summary>
''' Preparation et traitement centralise des exceptions.
''' </summary>
''' <remarks>
''' <para>
''' Gestion centralisee des exceptions : partout dans le code, un bloc
''' <c>Catch</c> appelle <see cref="PreparerException"/> pour construire un
''' rapport detaille, puis <see cref="TraiterException"/> pour le journaliser
''' (fichier, sortie de debogage et table <c>journal_erreur</c>).
''' </para>
''' <para>
''' Point essentiel : une garde de reentrance protege
''' <see cref="TraiterException"/>. En effet, la journalisation en base ouvre
''' elle-meme une connexion qui peut echouer (conteneur arrete) ; sans garde,
''' on obtiendrait une recursion infinie exception -> log -> exception.
''' </para>
''' </remarks>
Public Module GestionExceptions

    ' Empeche qu'un echec survenu PENDANT le traitement d'une exception ne
    ' relance le traitement (recursion). Marque par thread.
    <ThreadStatic>
    Private _enTraitement As Boolean

    ''' <summary>
    ''' Indique si la journalisation des exceptions en base de donnees est active.
    ''' Desactivable pour les demonstrations hors-ligne.
    ''' </summary>
    Public Property JournaliserEnBase As Boolean = True

    ''' <summary>
    ''' Construit un rapport d'exception detaille (message, code MySQL, pile
    ''' d'appels, exception interne, premiere frame source...).
    ''' </summary>
    ''' <param name="ex">Exception capturee.</param>
    ''' <param name="assemblyExecutante">Assembly d'ou provient l'appel (pour tracage).</param>
    ''' <param name="methode">Methode appelante (<see cref="MethodBase.GetCurrentMethod"/>).</param>
    ''' <param name="pileEnvironnement">Pile d'appels (<see cref="Environment.StackTrace"/>).</param>
    ''' <returns>Un rapport texte multiligne pret a etre journalise.</returns>
    Public Function PreparerException(ByVal ex As Exception,
                                      ByVal assemblyExecutante As Assembly,
                                      ByVal methode As MethodBase,
                                      ByVal pileEnvironnement As String) As String
        Dim sb As New StringBuilder()
        Try
            ' --- Code d'erreur specifique MySQL/MariaDB, le cas echeant ---------
            If TypeOf ex Is MySqlException Then
                Dim codeMySql As Integer = CInt(DirectCast(ex, MySqlException).Number)
                sb.AppendLine("Code erreur MySQL/MariaDB : " & codeMySql.ToString())
            End If

            ' --- Remonte jusqu'a l'exception "racine" (innermost) ---------------
            Dim exceptionRacine As Exception = ex
            While exceptionRacine.InnerException IsNot Nothing
                exceptionRacine = exceptionRacine.InnerException
            End While

            ' --- En-tete : horodatage, base, assembly, methode ------------------
            sb.AppendLine("[ " & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") & " ]")
            sb.AppendLine("[ Base : " & ConfigBdd.NomBase & " ]")
            If methode IsNot Nothing AndAlso methode.DeclaringType IsNot Nothing Then
                sb.AppendLine("[ " & assemblyExecutante.GetName().Name & " ] " &
                              methode.DeclaringType.Name & ".vb " & methode.Name & "()")
            End If
            sb.AppendLine("=========================")
            sb.AppendLine("Message : " & ex.Message)
            sb.AppendLine("=========================")
            sb.AppendLine("HResult : " & ex.HResult.ToString())
            sb.AppendLine("Source  : " & If(ex.Source, ""))
            If ex.TargetSite IsNot Nothing Then
                sb.AppendLine("TargetSite : " & ex.TargetSite.Name)
            End If

            ' --- Piles d'appels -------------------------------------------------
            sb.AppendLine("=========================")
            sb.AppendLine("Pile d'environnement :")
            sb.AppendLine(pileEnvironnement)
            sb.AppendLine("=========================")
            sb.AppendLine("Pile d'execution :")
            sb.AppendLine(If(ex.StackTrace, ""))

            ' --- Exception interne / racine ------------------------------------
            If ex.InnerException IsNot Nothing Then
                sb.AppendLine("=========================")
                sb.AppendLine("Exception interne : " & ex.InnerException.Message)
            End If
            If Not ReferenceEquals(exceptionRacine, ex) Then
                sb.AppendLine("=========================")
                sb.AppendLine("Exception racine : " & exceptionRacine.Message)
            End If

            ' --- Premiere frame disposant d'informations sources ---------------
            Try
                Dim st As New StackTrace(ex, True)
                Dim frames As StackFrame() = st.GetFrames()
                If frames IsNot Nothing AndAlso frames.Length > 0 AndAlso frames(0) IsNot Nothing Then
                    Dim f As StackFrame = frames(0)
                    Dim fichier As String = If(f.GetFileName(), "''")
                    Dim ligne As Integer = f.GetFileLineNumber()
                    Dim nomMethode As String = If(f.GetMethod() IsNot Nothing, f.GetMethod().Name & "()", "''")
                    sb.AppendLine("=========================")
                    sb.AppendLine("Localisation : ligne " & ligne.ToString() & " dans " & fichier & " " & nomMethode)
                End If
            Catch
                ' Les informations de frame ne sont pas toujours disponibles (Release sans PDB).
            End Try

        Catch interne As Exception
            ' Le rapport ne doit jamais lever d'exception lui-meme.
            Return "[ Rapport incomplet : " & interne.Message & " ]" & Environment.NewLine & sb.ToString()
        End Try
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Surcharge pratique : prepare un rapport a partir de la seule exception,
    ''' en deduisant automatiquement assembly et methode appelante.
    ''' </summary>
    Public Function PreparerException(ByVal ex As Exception) As String
        Return PreparerException(ex, Assembly.GetCallingAssembly(),
                                 New StackTrace().GetFrame(1)?.GetMethod(),
                                 Environment.StackTrace)
    End Function

    ''' <summary>
    ''' Traite un rapport d'exception : journalisation fichier/sortie debogage,
    ''' puis enregistrement en base (table <c>journal_erreur</c>) si active.
    ''' </summary>
    ''' <param name="rapport">Rapport produit par <see cref="PreparerException"/>.</param>
    ''' <param name="arret">
    ''' Indique si l'erreur est consideree comme bloquante (simple information
    ''' portee dans la trace ; aucune action d'arret n'est realisee ici).
    ''' </param>
    Public Sub TraiterException(ByVal rapport As String, Optional ByVal arret As Boolean = True)
        ' Garde de reentrance : si on est deja en train de traiter une exception
        ' (typiquement un echec de journalisation en base), on se contente d'une
        ' trace minimale, sans relancer tout le mecanisme.
        If _enTraitement Then
            Debug.WriteLine("[GestionExceptions] (reentrance) " & rapport)
            Return
        End If

        _enTraitement = True
        Try
            Journal.Ecrire((If(arret, "[ARRET] ", "[CONTINUE] ")) & rapport, Journal.Niveau.Erreur)

            If JournaliserEnBase Then
                Try
                    ' LogErreur est volontairement defensif : il n'appelle jamais
                    ' TraiterException en cas d'echec (cf. AccesDonnees).
                    AccesDonnees.LogErreur(rapport, OutilsSysteme.NomUtilisateur())
                Catch
                    ' Journalisation en base impossible : on ne fait rien de plus.
                End Try
            End If
        Finally
            _enTraitement = False
        End Try
    End Sub

    ''' <summary>Alias de <see cref="TraiterException"/> (compatibilite de nommage).</summary>
    Public Sub TraiterErreur(ByVal rapport As String, Optional ByVal arret As Boolean = True)
        TraiterException(rapport, arret)
    End Sub

End Module

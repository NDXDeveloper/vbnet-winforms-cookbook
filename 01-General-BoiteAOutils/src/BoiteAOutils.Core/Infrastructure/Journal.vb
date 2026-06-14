Imports System.IO
Imports System.Threading

''' <summary>
''' Journal applicatif minimaliste et thread-safe.
''' </summary>
''' <remarks>
''' <para>
''' Conserve les traces a la fois :
''' </para>
''' <list type="bullet">
''' <item><description>dans la fenetre de sortie de Visual Studio (<see cref="Debug"/>) ;</description></item>
''' <item><description>dans un fichier texte horodate (dossier temporaire) ;</description></item>
''' <item><description>dans une memoire tampon circulaire exploitee par la galerie WinForms.</description></item>
''' </list>
''' <para>
''' L'evenement <see cref="LigneAjoutee"/> permet a l'interface de s'abonner
''' pour afficher en direct le cycle de vie des connexions et des requetes :
''' c'est tout l'interet pedagogique de ce journal.
''' </para>
''' </remarks>
Public Module Journal

    ''' <summary>Niveaux de gravite, du plus bavard au plus critique.</summary>
    Public Enum Niveau
        ''' <summary>Trace technique fine (ouverture/fermeture de connexion...).</summary>
        Debogage = 0
        ''' <summary>Information de fonctionnement normal.</summary>
        Information = 1
        ''' <summary>Anomalie non bloquante.</summary>
        Avertissement = 2
        ''' <summary>Erreur ou exception.</summary>
        Erreur = 3
    End Enum

    ' Verrou protegeant l'ecriture concurrente (les BackgroundWorker journalisent
    ' depuis des threads differents du thread d'interface).
    Private ReadOnly _verrou As New Object()

    ' Tampon circulaire : on ne conserve que les N dernieres lignes en memoire.
    Private Const TAILLE_TAMPON As Integer = 500
    Private ReadOnly _tampon As New List(Of String)()

    ''' <summary>Niveau minimal reellement enregistre (filtre les traces trop fines).</summary>
    Public Property NiveauMinimal As Niveau = Niveau.Debogage

    ''' <summary>Chemin du fichier journal courant.</summary>
    Public ReadOnly Property CheminFichier As String
        Get
            Return Path.Combine(Path.GetTempPath(), "BoiteAOutils.log")
        End Get
    End Property

    ''' <summary>
    ''' Declenche a chaque nouvelle ligne effectivement journalisee.
    ''' La galerie WinForms s'y abonne pour rafraichir son panneau de log.
    ''' </summary>
    Public Event LigneAjoutee(ByVal ligneFormatee As String, ByVal niveauLigne As Niveau)

    ''' <summary>
    ''' Enregistre un message au niveau indique.
    ''' </summary>
    ''' <param name="message">Texte a journaliser.</param>
    ''' <param name="niveauMessage">Gravite du message.</param>
    Public Sub Ecrire(ByVal message As String, Optional ByVal niveauMessage As Niveau = Niveau.Information)
        If niveauMessage < NiveauMinimal Then Return

        Dim ligne As String = String.Format(
            "[{0:yyyy-MM-dd HH:mm:ss.fff}] [{1}] [T{2}] {3}",
            DateTime.Now,
            niveauMessage.ToString().ToUpperInvariant(),
            Thread.CurrentThread.ManagedThreadId,
            message)

        SyncLock _verrou
            ' 1) Fenetre de sortie de Visual Studio.
            Debug.WriteLine(ligne)

            ' 2) Tampon memoire circulaire.
            _tampon.Add(ligne)
            If _tampon.Count > TAILLE_TAMPON Then
                _tampon.RemoveAt(0)
            End If

            ' 3) Fichier texte (best effort : une erreur d'ecriture ne doit jamais
            '    faire planter l'application appelante).
            Try
                File.AppendAllText(CheminFichier, ligne & Environment.NewLine)
            Catch
                ' Volontairement ignore.
            End Try
        End SyncLock

        RaiseEvent LigneAjoutee(ligne, niveauMessage)
    End Sub

    ''' <summary>Retourne une copie des dernieres lignes conservees en memoire.</summary>
    Public Function DernieresLignes() As String()
        SyncLock _verrou
            Return _tampon.ToArray()
        End SyncLock
    End Function

    ''' <summary>Vide la memoire tampon (le fichier journal n'est pas efface).</summary>
    Public Sub Vider()
        SyncLock _verrou
            _tampon.Clear()
        End SyncLock
    End Sub

End Module

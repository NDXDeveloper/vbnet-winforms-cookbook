Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions

''' <summary>
''' Outils de manipulation des fichiers et dossiers.
''' </summary>
''' <remarks>
''' Chemins, creation de dossiers, droits d'ecriture, recherche recursive
''' d'images et fusion de tableaux d'octets.
''' </remarks>
Public Module OutilsFichiers

    ''' <summary>
    ''' Garantit qu'un chemin de dossier se termine par un separateur
    ''' (<c>\</c> sous Windows). Pratique avant de concatener un nom de fichier.
    ''' </summary>
    Public Function AjouterSeparateurFinal(ByVal dossier As String) As String
        If String.IsNullOrEmpty(dossier) Then Return String.Empty
        If Not dossier.EndsWith(Path.DirectorySeparatorChar) Then
            Return dossier & Path.DirectorySeparatorChar
        End If
        Return dossier
    End Function

    ''' <summary>Cree un dossier s'il n'existe pas. Retourne True si le dossier existe au final.</summary>
    Public Function CreerDossierSiAbsent(ByVal dossier As String) As Boolean
        Try
            If Not Directory.Exists(dossier) Then
                Directory.CreateDirectory(dossier)
            End If
            Return Directory.Exists(dossier)
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Transforme une chaine en nom de fichier valide en remplacant les
    ''' caracteres interdits par "_", puis en otant les caracteres speciaux.
    ''' </summary>
    Public Function RendreNomFichierValide(ByVal nom As String) As String
        If nom Is Nothing Then Return String.Empty
        Dim caracteresInterdits As String = Regex.Escape(New String(Path.GetInvalidFileNameChars()))
        Dim motif As String = String.Format("([{0}]*\.+$)|([{0}]+)", caracteresInterdits)
        Return OutilsChaines.RetirerCaracteresSpeciaux(Regex.Replace(nom, motif, "_"))
    End Function

    ''' <summary>
    ''' Verifie si un dossier est accessible en ecriture, en y creant un fichier
    ''' temporaire auto-supprime (<see cref="FileOptions.DeleteOnClose"/>).
    ''' </summary>
    ''' <param name="leverSiEchec">Si True, propage l'exception au lieu de renvoyer False.</param>
    Public Function DossierAccessibleEnEcriture(ByVal chemin As String,
                                                Optional ByVal leverSiEchec As Boolean = False) As Boolean
        Try
            Using fs As FileStream = File.Create(
                Path.Combine(chemin, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose)
            End Using
            Return True
        Catch
            If leverSiEchec Then Throw
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Verifie si l'application peut ecrire dans un fichier en l'ouvrant en
    ''' ecriture. Le message d'erreur eventuel est retourne via
    ''' <paramref name="messageErreur"/>.
    ''' </summary>
    Public Function PeutEcrireDansFichier(ByVal chemin As String, ByRef messageErreur As String) As Boolean
        messageErreur = String.Empty
        Try
            Using fs As FileStream = File.Open(chemin, FileMode.OpenOrCreate, FileAccess.Write)
                Return True
            End Using
        Catch ex As Exception
            messageErreur = ex.Message
            Return False
        End Try
    End Function

    ''' <summary>Retourne la taille d'un fichier en octets.</summary>
    Public Function TailleFichier(ByVal chemin As String) As Long
        Return New FileInfo(chemin).Length
    End Function

    ''' <summary>Concatene deux tableaux d'octets en un seul (copie binaire rapide).</summary>
    Public Function Combiner(ByVal premier As Byte(), ByVal second As Byte()) As Byte()
        Dim resultat(premier.Length + second.Length - 1) As Byte
        Buffer.BlockCopy(premier, 0, resultat, 0, premier.Length)
        Buffer.BlockCopy(second, 0, resultat, premier.Length, second.Length)
        Return resultat
    End Function

    ''' <summary>
    ''' Indique si un fichier est une image valide, en tentant de la charger.
    ''' GDI+ leve <see cref="OutOfMemoryException"/> pour un format non reconnu.
    ''' </summary>
    Public Function EstImageValide(ByVal chemin As String) As Boolean
        Try
            Using img As Image = Image.FromFile(chemin)
            End Using
            Return True
        Catch generatedExceptionName As OutOfMemoryException
            ' Format d'image non valide / non supporte par GDI+.
            Return False
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Recherche recursivement toutes les images valides d'un dossier et de ses
    ''' sous-dossiers, et retourne la liste de leurs chemins.
    ''' </summary>
    Public Function RechercherImages(ByVal dossier As String) As List(Of String)
        Dim fichiers As New List(Of String)()
        Try
            For Each f As String In Directory.GetFiles(dossier)
                If EstImageValide(f) Then fichiers.Add(f)
            Next
            ' Recursion : meme traitement sur chaque sous-dossier.
            For Each sousDossier As String In Directory.GetDirectories(dossier)
                fichiers.AddRange(RechercherImages(sousDossier))
            Next
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
        End Try
        Return fichiers
    End Function

    ''' <summary>Cree un fichier temporaire et retourne son chemin.</summary>
    Public Function NomFichierTemporaire() As String
        Try
            Return Path.GetTempFileName()
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return String.Empty
        End Try
    End Function

End Module

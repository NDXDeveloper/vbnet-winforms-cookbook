' ============================================================================
'  PuitsFichier.vb
'  Puits fichier avec rotation par taille.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.IO
Imports System.Text

''' <summary>
''' Puits fichier : ecrit les entrees dans un fichier texte. Quand le fichier
''' depasse une taille maximale, il est <b>roule</b> : <c>journal.log</c> devient
''' <c>journal.1.log</c>, l'ancien <c>.1</c> devient <c>.2</c>, etc., jusqu'a un
''' nombre d'archives donne (les plus anciennes sont supprimees).
''' </summary>
Public NotInheritable Class PuitsFichier
    Implements IPuits

    Private ReadOnly _chemin As String
    Private ReadOnly _tailleMax As Long
    Private ReadOnly _nbArchives As Integer
    Private ReadOnly _verrou As New Object()
    Private ReadOnly _encodage As New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)

    ''' <summary>Chemin du fichier journal courant.</summary>
    Public ReadOnly Property Chemin As String
        Get
            Return _chemin
        End Get
    End Property

    ''' <param name="chemin">Chemin du fichier journal.</param>
    ''' <param name="tailleMaxOctets">Taille declenchant la rotation (1 Mo par defaut).</param>
    ''' <param name="nbArchives">Nombre de fichiers archives conserves.</param>
    Public Sub New(ByVal chemin As String, Optional ByVal tailleMaxOctets As Long = 1048576, Optional ByVal nbArchives As Integer = 3)
        _chemin = chemin
        _tailleMax = Math.Max(1024, tailleMaxOctets)
        _nbArchives = Math.Max(0, nbArchives)
        Dim dossier As String = Path.GetDirectoryName(chemin)
        If Not String.IsNullOrEmpty(dossier) AndAlso Not Directory.Exists(dossier) Then
            Directory.CreateDirectory(dossier)
        End If
    End Sub

    Public Sub Ecrire(ByVal entree As EntreeJournal) Implements IPuits.Ecrire
        SyncLock _verrou
            RoterSiNecessaire()
            File.AppendAllText(_chemin, entree.Formater() & Environment.NewLine, _encodage)
        End SyncLock
    End Sub

    ' Renomme en cascade lorsque le fichier courant atteint la taille maximale.
    Private Sub RoterSiNecessaire()
        If Not File.Exists(_chemin) Then Return
        If New FileInfo(_chemin).Length < _tailleMax Then Return

        ' Supprime l'archive la plus ancienne, puis decale les autres.
        Dim archiveLaPlusAncienne As String = CheminArchive(_nbArchives)
        If File.Exists(archiveLaPlusAncienne) Then File.Delete(archiveLaPlusAncienne)

        For i As Integer = _nbArchives - 1 To 1 Step -1
            Dim src As String = CheminArchive(i)
            If File.Exists(src) Then File.Move(src, CheminArchive(i + 1))
        Next

        If _nbArchives >= 1 Then
            File.Move(_chemin, CheminArchive(1))
        Else
            File.Delete(_chemin) ' aucune archive demandee : on repart a vide
        End If
    End Sub

    ' "journal.log" -> "journal.1.log"
    Private Function CheminArchive(ByVal index As Integer) As String
        Dim dossier As String = Path.GetDirectoryName(_chemin)
        Dim nom As String = Path.GetFileNameWithoutExtension(_chemin)
        Dim ext As String = Path.GetExtension(_chemin)
        Return Path.Combine(If(dossier, ""), nom & "." & index.ToString() & ext)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Aucune ressource persistante (ouverture/fermeture a chaque ecriture).
    End Sub

End Class

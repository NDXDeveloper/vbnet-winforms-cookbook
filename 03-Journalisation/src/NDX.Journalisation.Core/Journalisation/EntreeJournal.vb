' ============================================================================
'  EntreeJournal.vb
'  Represente une entree de journal immuable.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Globalization

''' <summary>Une entree de journal : horodatage, niveau, categorie, message, exception eventuelle.</summary>
Public NotInheritable Class EntreeJournal

    ''' <summary>Date et heure de creation de l'entree.</summary>
    Public ReadOnly Property Horodatage As DateTime
    ''' <summary>Gravite de l'entree.</summary>
    Public ReadOnly Property Niveau As Niveau
    ''' <summary>Categorie fonctionnelle (ex. "Demarrage", "BDD", "Reseau").</summary>
    Public ReadOnly Property Categorie As String
    ''' <summary>Message libre.</summary>
    Public ReadOnly Property Message As String
    ''' <summary>Texte de l'exception associee, ou Nothing.</summary>
    Public ReadOnly Property Exception As String

    Public Sub New(ByVal niveau As Niveau, ByVal categorie As String,
                   ByVal message As String, Optional ByVal exception As String = Nothing)
        Me.Horodatage = DateTime.Now
        Me.Niveau = niveau
        Me.Categorie = If(categorie, "")
        Me.Message = If(message, "")
        Me.Exception = exception
    End Sub

    ''' <summary>Libelle court du niveau (ex. "INFO", "ERREUR").</summary>
    Public Shared Function LibelleNiveau(ByVal n As Niveau) As String
        Select Case n
            Case Niveau.Debogage : Return "DEBUG"
            Case Niveau.Information : Return "INFO"
            Case Niveau.Avertissement : Return "AVERT"
            Case Niveau.Erreur : Return "ERREUR"
            Case Niveau.Critique : Return "CRITIQUE"
            Case Else : Return n.ToString().ToUpperInvariant()
        End Select
    End Function

    ''' <summary>Rend l'entree sous forme d'une ligne de texte lisible.</summary>
    Public Function Formater() As String
        Dim ligne As String = String.Format(CultureInfo.InvariantCulture,
            "[{0:yyyy-MM-dd HH:mm:ss.fff}] [{1,-8}] [{2}] {3}",
            Horodatage, LibelleNiveau(Niveau), Categorie, Message)
        If Not String.IsNullOrEmpty(Exception) Then
            ligne &= Environment.NewLine & "    --> " & Exception
        End If
        Return ligne
    End Function

End Class

' ============================================================================
'  SequenceDemarrage.vb  -  Execution ordonnee des etapes de demarrage.
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Collections.Generic

''' <summary>
''' Exécute une suite d'étapes de démarrage dans l'ordre, en collectant un
''' <see cref="ResultatEtape"/> par étape. Une étape qui échoue (False ou exception)
''' arrête la séquence si demandé. Logique pure : testable sans interface.
''' </summary>
Public NotInheritable Class SequenceDemarrage

    Private Sub New()
    End Sub

    ''' <summary>Exécute les étapes ; renvoie un résultat par étape exécutée.</summary>
    Public Shared Function Executer(ByVal etapes As IEnumerable(Of EtapeDemarrage),
                                    Optional ByVal arreterAuPremierEchec As Boolean = True) As List(Of ResultatEtape)
        Dim resultats As New List(Of ResultatEtape)()
        If etapes Is Nothing Then Return resultats
        For Each etape As EtapeDemarrage In etapes
            Dim resultat As ResultatEtape
            Try
                Dim ok As Boolean = etape.Verification.Invoke()
                resultat = New ResultatEtape(etape.Nom, ok, If(ok, "Terminé.", "Échec de la vérification."))
            Catch ex As Exception
                resultat = New ResultatEtape(etape.Nom, False, ex.Message)
            End Try
            resultats.Add(resultat)
            If Not resultat.Reussi AndAlso arreterAuPremierEchec Then Exit For
        Next
        Return resultats
    End Function

    ''' <summary>Vrai si toutes les étapes exécutées ont réussi.</summary>
    Public Shared Function ToutEstReussi(ByVal resultats As IEnumerable(Of ResultatEtape)) As Boolean
        If resultats Is Nothing Then Return False
        For Each r As ResultatEtape In resultats
            If Not r.Reussi Then Return False
        Next
        Return True
    End Function

End Class

' ============================================================================
'  PageTransaction.vb  -  Demonstration de l'Unit of Work (commit / rollback).
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com> - Licence MIT (voir LICENSE).
' ============================================================================

Imports System.Drawing
Imports System.Windows.Forms
Imports NDX.Donnees

Public NotInheritable Class PageTransaction
    Inherits PageBase

    Private ReadOnly _console As TextBox

    Public Sub New()
        MyBase.New("Transaction (Unit of Work)",
                   "Insérer 3 produits dans une même transaction, puis valider ou annuler. On compte avant / pendant / après.")
        _console = Console()
        Construire()
    End Sub

    Private Sub Construire()
        Dim barre As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .AutoSize = True}
        barre.Controls.Add(Bouton("Insérer 3 produits puis VALIDER", AddressOf SurValider))
        barre.Controls.Add(Bouton("Insérer 3 produits puis ANNULER", AddressOf SurAnnuler))
        barre.Controls.Add(Bouton("Effacer", AddressOf SurEffacer))
        Contenu.Controls.Add(_console)
        Contenu.Controls.Add(barre)
    End Sub

    Private Sub Tracer(ByVal texte As String)
        _console.AppendText(texte & Environment.NewLine)
    End Sub

    Private Sub Demo(ByVal valider As Boolean)
        Try
            Dim avant As Long = New DepotProduit().Compter()
            Tracer("=== " & If(valider, "VALIDATION", "ANNULATION") & " ===")
            Tracer("Avant : " & avant.ToString() & " produit(s).")

            Using uow As New UniteDeTravail()
                uow.Commencer()
                Dim depot As New DepotProduit(uow)
                Dim prefixe As String = "TX-" & Guid.NewGuid().ToString("N").Substring(0, 6)
                For i As Integer = 1 To 3
                    depot.Inserer(New Produit() With {
                        .Reference = prefixe & "-" & i.ToString(),
                        .Designation = "Produit transactionnel " & i.ToString(),
                        .PrixHt = CDec(i),
                        .Stock = i})
                Next
                ' Compté via la MEME connexion/transaction : voit les insertions non encore validees.
                Tracer("Pendant (même transaction) : " & depot.Compter().ToString() & " produit(s).")
                If valider Then uow.Valider() Else uow.Annuler()
            End Using

            Dim apres As Long = New DepotProduit().Compter()
            Tracer("Après : " & apres.ToString() & " produit(s).")
            Tracer(If(valider, "-> Les 3 insertions sont persistées.", "-> Rien n'a été persisté (rollback)."))
            Tracer("")
        Catch ex As Exception
            Tracer("Erreur : " & ex.Message)
        End Try
    End Sub

    Private Sub SurValider(ByVal s As Object, ByVal e As EventArgs)
        Demo(True)
    End Sub
    Private Sub SurAnnuler(ByVal s As Object, ByVal e As EventArgs)
        Demo(False)
    End Sub
    Private Sub SurEffacer(ByVal s As Object, ByVal e As EventArgs)
        _console.Clear()
    End Sub

End Class

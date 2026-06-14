' ============================================================================
'  ModeleDemo.vb
'  Objets metier de demonstration, concus pour etre serialisables par les
'  quatre formats (XML, contrat XML, binaire, JSON).
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

Imports System.Collections.Generic
Imports System.Runtime.Serialization

''' <summary>
''' Un produit du catalogue.
''' </summary>
''' <remarks>
''' Les regles communes aux serialiseurs sont respectees : type public,
''' constructeur public sans parametre, proprietes publiques en lecture/ecriture.
''' Les attributs <see cref="DataContractAttribute"/> / <see cref="DataMemberAttribute"/>
''' guident le contrat de donnees (XML/JSON) ; ils sont ignores par XmlSerializer,
''' qui se fonde sur les proprietes publiques.
''' </remarks>
<DataContract(Name:="Produit", [Namespace]:="")>
Public Class Produit

    <DataMember(Order:=0)>
    Public Property Reference As String

    <DataMember(Order:=1)>
    Public Property Designation As String

    <DataMember(Order:=2)>
    Public Property PrixHt As Decimal

    <DataMember(Order:=3)>
    Public Property Quantite As Integer

    ''' <summary>Constructeur sans parametre requis par la serialisation.</summary>
    Public Sub New()
    End Sub

    ''' <summary>Constructeur de confort.</summary>
    Public Sub New(ByVal reference As String, ByVal designation As String,
                   ByVal prixHt As Decimal, ByVal quantite As Integer)
        Me.Reference = reference
        Me.Designation = designation
        Me.PrixHt = prixHt
        Me.Quantite = quantite
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0} - {1} ({2:0.00} EUR x {3})", Reference, Designation, PrixHt, Quantite)
    End Function

End Class

''' <summary>
''' Un catalogue de produits (objet racine de demonstration : il contient des
''' types imbriques et une collection, cas representatif de serialisation).
''' </summary>
<DataContract(Name:="Catalogue", [Namespace]:="")>
Public Class Catalogue

    <DataMember(Order:=0)>
    Public Property Nom As String

    <DataMember(Order:=1)>
    Public Property DateGeneration As Date

    <DataMember(Order:=2)>
    Public Property Produits As List(Of Produit)

    Public Sub New()
        Produits = New List(Of Produit)()
    End Sub

    ''' <summary>Construit un catalogue d'exemple, utile pour les demonstrations et les tests.</summary>
    Public Shared Function Exemple() As Catalogue
        Return New Catalogue With {
            .Nom = "Catalogue de demonstration",
            .DateGeneration = New Date(2026, 1, 15, 9, 30, 0),
            .Produits = New List(Of Produit) From {
                New Produit("PR-001", "Cle a molette", 12.9D, 25),
                New Produit("PR-002", "Tournevis cruciforme", 4.5D, 120),
                New Produit("PR-003", "Niveau a bulle 40 cm", 18.75D, 12),
                New Produit("PR-004", "Metre ruban 5 m", 6.2D, 60)
            }
        }
    End Function

End Class

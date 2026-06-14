Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Reflection

''' <summary>
''' Outils de creation, conversion et analyse d'images (GDI+).
''' </summary>
''' <remarks>
''' Conversion en nuances de gris (matrice de couleurs), detection du format par
''' signature binaire, et generation de "plans vides".
''' </remarks>
Public Module OutilsImages

    ''' <summary>Largeur par defaut d'un plan vide genere.</summary>
    Public Const LARGEUR_PLAN_VIDE As Integer = 1920
    ''' <summary>Hauteur par defaut d'un plan vide genere.</summary>
    Public Const HAUTEUR_PLAN_VIDE As Integer = 1358

#Region "Nuances de gris"
    ''' <summary>
    ''' Convertit une image en nuances de gris a l'aide d'une matrice de couleurs
    ''' appliquant les coefficients de luminance perceptuelle (0.30 / 0.59 / 0.11).
    ''' </summary>
    Public Function EnNuancesDeGris(ByVal original As Image) As Bitmap
        Dim resultat As New Bitmap(original.Width, original.Height)
        Using g As Graphics = Graphics.FromImage(resultat)
            ' Matrice 5x5 : chaque canal de sortie recoit la meme combinaison
            ' ponderee des canaux d'entree -> teinte de gris.
            Dim matrice As New ColorMatrix(New Single()() {
                New Single() {0.3F, 0.3F, 0.3F, 0, 0},
                New Single() {0.59F, 0.59F, 0.59F, 0, 0},
                New Single() {0.11F, 0.11F, 0.11F, 0, 0},
                New Single() {0, 0, 0, 1, 0},
                New Single() {0, 0, 0, 0, 1}
            })
            Using attributs As New ImageAttributes()
                attributs.SetColorMatrix(matrice)
                g.DrawImage(original,
                            New Rectangle(0, 0, original.Width, original.Height),
                            0, 0, original.Width, original.Height,
                            GraphicsUnit.Pixel, attributs)
            End Using
        End Using
        Return resultat
    End Function

    ''' <summary>
    ''' Determine si une image est deja en nuances de gris, en comparant ses
    ''' octets a ceux de sa version convertie en gris (egalite binaire).
    ''' </summary>
    Public Function EstEnNuancesDeGris(ByVal original As Image) As Boolean
        If original Is Nothing Then Return False
        Try
            Using copie As Bitmap = New Bitmap(original.Width, original.Height)
                Using g As Graphics = Graphics.FromImage(copie)
                    g.DrawImage(original, New Rectangle(0, 0, original.Width, original.Height))
                End Using
                Using fluxA As New MemoryStream()
                    copie.Save(fluxA, ImageFormat.Jpeg)
                    Dim a As Byte() = fluxA.ToArray()
                    Using gris As Bitmap = EnNuancesDeGris(original)
                        Using fluxB As New MemoryStream()
                            gris.Save(fluxB, ImageFormat.Jpeg)
                            Return a.SequenceEqual(fluxB.ToArray())
                        End Using
                    End Using
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function
#End Region

#Region "Detection d'extension par signature (magic bytes)"
    ' Familles de formats reconnaissables a leurs premiers octets.
    Private Enum Format
        Inconnu = 0
        DocOuXls
        Pdf
        Jpg
        Png
        DocxOuXlsx
    End Enum

    ' Signatures binaires (en notation hexadecimale "XX-XX-...").
    Private ReadOnly _signatures As New Dictionary(Of Format, String) From {
        {Format.DocOuXls, "D0-CF-11-E0-A1-B1-1A-E1"},
        {Format.Pdf, "25-50-44-46"},
        {Format.Jpg, "FF-D8-FF-E"},
        {Format.Png, "89-50-4E-47-0D-0A-1A-0A"},
        {Format.DocxOuXlsx, "50-4B-03-04-14-00-06-00"}
    }

    ''' <summary>
    ''' Deduit l'extension d'un fichier a partir de la signature de ses premiers
    ''' octets, independamment de son nom. Utile pour valider un type reel.
    ''' </summary>
    Public Function DetecterExtension(ByVal octets As Byte()) As String
        If octets Is Nothing OrElse octets.Length < 8 Then Return String.Empty

        Dim octetsSignature(7) As Byte
        Array.Copy(octets, octetsSignature, octetsSignature.Length)
        Dim signature As String = BitConverter.ToString(octetsSignature)

        Dim format As Format = _signatures.FirstOrDefault(Function(p) signature.Contains(p.Value)).Key

        Select Case format
            Case Format.Pdf
                Return ".pdf"
            Case Format.Jpg
                Return ".jpg"
            Case Format.Png
                Return ".png"
            Case Format.DocOuXls
                ' Format composite OLE : on departage .doc / .xls plus loin dans le fichier.
                If octets.Length < 516 Then Return ".xls"
                Dim sousSignature(3) As Byte
                Array.Copy(octets, 512, sousSignature, 0, sousSignature.Length)
                If BitConverter.ToString(sousSignature) = "EC-A5-C1-00" Then Return ".doc"
                Return ".xls"
            Case Format.DocxOuXlsx
                ' Conteneur ZIP (Office Open XML) : on cherche un indice textuel.
                Dim contenu As String = System.Text.Encoding.UTF8.GetString(octets)
                If contenu.Contains("word") Then Return ".docx"
                If contenu.Contains("xl") Then Return ".xlsx"
                Return String.Empty
            Case Else
                Return String.Empty
        End Select
    End Function
#End Region

#Region "Generation de plans vides"
    ''' <summary>
    ''' Cree une image avec un cadre et un fond legerement teinte, retournee sous
    ''' forme de tableau d'octets (JPEG).
    ''' </summary>
    Public Function CreerPlanVide(Optional ByVal largeur As Integer = LARGEUR_PLAN_VIDE,
                                  Optional ByVal hauteur As Integer = HAUTEUR_PLAN_VIDE) As Byte()
        Try
            Using bmp As New Bitmap(largeur, hauteur)
                Using gfx As Graphics = Graphics.FromImage(bmp)
                    Using fond As New SolidBrush(Color.White)
                        gfx.FillRectangle(fond, 0, 0, largeur, hauteur)
                    End Using
                    Dim cadre As New Rectangle(10, 10, bmp.Width - 20, bmp.Height - 20)
                    Using stylo As New Pen(Color.FromArgb(255, 53, 54, 82), 5)
                        gfx.DrawRectangle(stylo, cadre)
                    End Using
                    Using voile As New SolidBrush(Color.FromArgb(25, 255, 255, 255))
                        gfx.FillRectangle(voile, cadre)
                    End Using
                End Using
                Return EnTableauOctets(bmp)
            End Using
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return Nothing
        End Try
    End Function

    ''' <summary>Cree une image entierement blanche, retournee sous forme de tableau d'octets (JPEG).</summary>
    Public Function CreerPlanVideBlanc(Optional ByVal largeur As Integer = LARGEUR_PLAN_VIDE,
                                       Optional ByVal hauteur As Integer = HAUTEUR_PLAN_VIDE) As Byte()
        Try
            Using bmp As New Bitmap(largeur, hauteur)
                Using gfx As Graphics = Graphics.FromImage(bmp)
                    Using fond As New SolidBrush(Color.White)
                        gfx.FillRectangle(fond, 0, 0, largeur, hauteur)
                    End Using
                End Using
                Return EnTableauOctets(bmp)
            End Using
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return Nothing
        End Try
    End Function

    Private Function EnTableauOctets(ByVal bmp As Bitmap) As Byte()
        Using flux As New MemoryStream()
            bmp.Save(flux, ImageFormat.Jpeg)
            Return flux.ToArray()
        End Using
    End Function
#End Region

#Region "Encodeurs / sauvegarde"
    ''' <summary>Retourne l'encodeur GDI+ correspondant a un type MIME (ex. "image/png").</summary>
    Public Function EncodeurPour(ByVal typeMime As String) As ImageCodecInfo
        Return ImageCodecInfo.GetImageEncoders().FirstOrDefault(Function(e) e.MimeType = typeMime)
    End Function

    ''' <summary>Sauvegarde un bitmap au format PNG a l'emplacement indique.</summary>
    Public Function SauverEnPng(ByVal img As Bitmap, ByVal destination As String) As Boolean
        If String.IsNullOrWhiteSpace(destination) Then Return False
        Try
            Dim chemin As String = If(destination.ToLowerInvariant().EndsWith(".png"), destination, destination & ".png")
            img.Save(chemin, ImageFormat.Png)
            Return File.Exists(chemin)
        Catch ex As Exception
            GestionExceptions.TraiterException(GestionExceptions.PreparerException(
                ex, Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), Environment.StackTrace))
            Return False
        End Try
    End Function
#End Region

End Module

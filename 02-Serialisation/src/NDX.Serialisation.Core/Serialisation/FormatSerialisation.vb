' ============================================================================
'  FormatSerialisation.vb
'  Enumeration des formats de serialisation pris en charge.
'
'  Copyright (c) 2026 Nicolas DEOUX <NDXDev@gmail.com>
'  Distribue sous licence MIT - voir le fichier LICENSE a la racine du projet.
' ============================================================================

''' <summary>
''' Formats de serialisation pris en charge par le <see cref="Serialiseur"/>.
''' </summary>
Public Enum FormatSerialisation

    ''' <summary>
    ''' XML lisible via <see cref="System.Xml.Serialization.XmlSerializer"/>.
    ''' Ideal pour l'interoperabilite et la lecture humaine ; ne serialise que les
    ''' membres publics et exige un constructeur public sans parametre.
    ''' </summary>
    Xml = 0

    ''' <summary>
    ''' XML via <see cref="System.Runtime.Serialization.DataContractSerializer"/>.
    ''' Plus strict sur le contrat de donnees, gere davantage de graphes d'objets.
    ''' </summary>
    ContratXml = 1

    ''' <summary>
    ''' Binaire compact et SUR, base sur le contrat de donnees (writer/reader
    ''' binaire XML). Plus leger que le XML texte, sans les risques de securite
    ''' de la serialisation binaire historique.
    ''' </summary>
    Binaire = 2

    ''' <summary>
    ''' JSON via <see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/>.
    ''' Format texte compact, courant pour les echanges web.
    ''' </summary>
    Json = 3

End Enum

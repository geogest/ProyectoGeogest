using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Xml;

public class XMLHelper
{
    // Constantes
    // Constantes del metodo que dan acceso a los nodos
    // necesarios para realizar la verificacion del documento DTE
    const string XPATH_MODULUS = "//sii:DTE/sig:Signature/sig:KeyInfo/sig:KeyValue/sig:RSAKeyValue/sig:Modulus";
    const string XPATH_EXPONENT = "//sii:DTE/sig:Signature/sig:KeyInfo/sig:KeyValue/sig:RSAKeyValue/sig:Exponent";

    public static bool VerificarDTE(string pathXml)
    {
        //abrir documento DTE firmado sin modificar el orden del documento
        XmlDocument documento = new XmlDocument();
        documento.PreserveWhitespace = true;
        documento.Load(pathXml);

        //Crear namespaces del horto
        XmlNamespaceManager ns = new XmlNamespaceManager(documento.NameTable);
        ns.AddNamespace("sii", documento.DocumentElement.NamespaceURI);
        ns.AddNamespace("sig", "http://www.w3.org/2000/09/xmldsig#");

        //Recuperar valores MODULO y EXPONENTE de la firma para realizar la verificacion
        string Modulus = documento.SelectSingleNode(XPATH_MODULUS, ns).InnerText;
        string Exponent = documento.SelectSingleNode(XPATH_EXPONENT, ns).InnerText;

        //Representacion de los datos:
        //Crear la informacion de la clave publica con el formato necesario para realizar la verificacion
        string PublicKeyXml = string.Empty;
        PublicKeyXml += "<RSAKeyValue>";
        PublicKeyXml += "<Modulus>{0}</Modulus>";
        PublicKeyXml += "<Exponent>{1}</Exponent>";
        PublicKeyXml += "</RSAKeyValue>";

        //Parsear el string PublicKeyXml segun el formato exigido por la clase
        PublicKeyXml = string.Format(PublicKeyXml, Modulus, Exponent);

        //Recuperar la clave publica utilizando RSACryptoServiceProvider
        RSACryptoServiceProvider publicKey = new RSACryptoServiceProvider();
        publicKey.FromXmlString(PublicKeyXml);

        //Crear objeto SignedXML y pasar el documento firmado
        SignedXml signedXML = new SignedXml(documento);

        //Recuperar el nodo Signature del documento del documento xml y asignarlo al objeto nodelist.
        // NOTE: El documento dte(XML) debe estar firmado
        XmlNodeList nodeList = documento.GetElementsByTagName("Signature");

        //Cargue el nodo SIGNATURE en el objeto signedXML 
        signedXML.LoadXml((XmlElement)nodeList[0]);

        //Verificar la firma del documento
        bool IsRight = signedXML.CheckSignature(publicKey);

        return IsRight;
    }

    public static bool VerificarDTEXMLString(string pathXml)
    {
        //abrir documento DTE firmado sin modificar el orden del documento
        XmlDocument documento = new XmlDocument();
        documento.PreserveWhitespace = true;
        documento.LoadXml(pathXml);

        //Crear namespaces del horto
        XmlNamespaceManager ns = new XmlNamespaceManager(documento.NameTable);
        ns.AddNamespace("sii", documento.DocumentElement.NamespaceURI);
        ns.AddNamespace("sig", "http://www.w3.org/2000/09/xmldsig#");

        //Recuperar valores MODULO y EXPONENTE de la firma para realizar la verificacion
        string Modulus = documento.SelectSingleNode(XPATH_MODULUS, ns).InnerText;
        string Exponent = documento.SelectSingleNode(XPATH_EXPONENT, ns).InnerText;

        //Representacion de los datos:
        //Crear la informacion de la clave publica con el formato necesario para realizar la verificacion
        string PublicKeyXml = string.Empty;
        PublicKeyXml += "<RSAKeyValue>";
        PublicKeyXml += "<Modulus>{0}</Modulus>";
        PublicKeyXml += "<Exponent>{1}</Exponent>";
        PublicKeyXml += "</RSAKeyValue>";

        //Parsear el string PublicKeyXml segun el formato exigido por la clase
        PublicKeyXml = string.Format(PublicKeyXml, Modulus, Exponent);

        //Recuperar la clave publica utilizando RSACryptoServiceProvider
        RSACryptoServiceProvider publicKey = new RSACryptoServiceProvider();
        publicKey.FromXmlString(PublicKeyXml);

        //Crear objeto SignedXML y pasar el documento firmado
        SignedXml signedXML = new SignedXml(documento);

        //Recuperar el nodo Signature del documento del documento xml y asignarlo al objeto nodelist.
        // NOTE: El documento dte(XML) debe estar firmado
        XmlNodeList nodeList = documento.GetElementsByTagName("Signature");

        //Cargue el nodo SIGNATURE en el objeto signedXML 
        signedXML.LoadXml((XmlElement)nodeList[0]);

        //Verificar la firma del documento
        bool IsRight = signedXML.CheckSignature(publicKey);

        return IsRight;
    }

    public static XmlNode CreateNodeWithAttribute(string NodeName, string AttributeName, string AttributeValue, XmlDocument XMLDoc)
    {
        XmlNode Node = XMLDoc.CreateElement(NodeName);
        XmlAttribute Attribute = XMLDoc.CreateAttribute(AttributeName);
        Attribute.Value = AttributeValue;
        Node.Attributes.Append(Attribute);
        return Node;
    }

    public static XmlNode CreateNodeWithAttributeNS(string NodeName, string AttributeName, string AttributeValue, XmlDocument XMLDoc)
    {
        XmlNode Node = XMLDoc.CreateElement(NodeName, XMLDoc.DocumentElement.NamespaceURI);
        XmlAttribute Attribute = XMLDoc.CreateAttribute(AttributeName);
        Attribute.Value = AttributeValue;
        Node.Attributes.Append(Attribute);
        return Node;
    }

    public static XmlNode CreateNodeWithInnerText(string NodeName, string InnerText, XmlDocument XMLDoc)
    {
        XmlNode returnValue = XMLDoc.CreateElement(NodeName);
        returnValue.InnerText = InnerText;
        return returnValue;
    }

    public static void FirmarDocumentoXml(ref XmlDocument xmldocument, X509Certificate2 certificado, string referenciaUri)
    {
        ////
        //// Cree el objeto SignedXml donde xmldocument
        //// representa el documento DTE preparado para
        //// ser firmado. Recuerde que debe ser abierto 
        //// con la propiedad PreserveWhiteSpace = true
        SignedXml signedXml = new SignedXml(xmldocument);

        ////
        //// Agregue la clave privada al objeto signedXml
        signedXml.SigningKey = certificado.PrivateKey;

        ////
        //// Recupere el objeto signature desde signedXml
        Signature XMLSignature = signedXml.Signature;

        ////
        //// Cree la refrerencia al documento DTE
        //// recuerde que la referencia tiene el 
        //// formato '#reference'
        //// ejemplo '#DTE001'
        Reference reference = new Reference();
        reference.Uri = referenciaUri;

        ////
        //// Agregue la referencia al objeto signature
        XMLSignature.SignedInfo.AddReference(reference);
        KeyInfo keyInfo = new KeyInfo();
        keyInfo.AddClause(new RSAKeyValue((RSA)certificado.PrivateKey));

        ////
        //// Agregar información del certificado x509
        keyInfo.AddClause(new KeyInfoX509Data(certificado));
        XMLSignature.KeyInfo = keyInfo;

        ////
        //// Calcule la firma y recupere la representacion
        //// de la firma en un objeto xmlElement
        signedXml.ComputeSignature();
        XmlElement xmlDigitalSignature = signedXml.GetXml();

        ////
        //// Inserte la firma en el documento DTE
        xmldocument.DocumentElement.AppendChild(xmldocument.ImportNode(xmlDigitalSignature, true));

    }

    public static void FirmarDocumentoXml(ref XmlDocument xmldocument, X509Certificate2 certificado, string referenciaUri, ref XmlNode NodeToAppend)
    {
        ////
        //// Cree el objeto SignedXml donde xmldocument
        //// representa el documento DTE preparado para
        //// ser firmado. Recuerde que debe ser abierto 
        //// con la propiedad PreserveWhiteSpace = true
        SignedXml signedXml = new SignedXml(xmldocument);

        ////
        //// Agregue la clave privada al objeto signedXml
        signedXml.SigningKey = certificado.PrivateKey;

        ////
        //// Recupere el objeto signature desde signedXml
        Signature XMLSignature = signedXml.Signature;

        ////
        //// Cree la refrerencia al documento DTE
        //// recuerde que la referencia tiene el 
        //// formato '#reference'
        //// ejemplo '#DTE001'
        Reference reference = new Reference();
        reference.Uri = referenciaUri;

        ////
        //// Agregue la referencia al objeto signature
        XMLSignature.SignedInfo.AddReference(reference);
        KeyInfo keyInfo = new KeyInfo();
        keyInfo.AddClause(new RSAKeyValue((RSA)certificado.PrivateKey));

        ////
        //// Agregar información del certificado x509
        keyInfo.AddClause(new KeyInfoX509Data(certificado));
        XMLSignature.KeyInfo = keyInfo;

        ////
        //// Calcule la firma y recupere la representacion
        //// de la firma en un objeto xmlElement
        signedXml.ComputeSignature();
        XmlElement xmlDigitalSignature = signedXml.GetXml();

        ////
        //// Inserte la firma en el documento DTE
        NodeToAppend.AppendChild(xmldocument.ImportNode(xmlDigitalSignature, true));
        //xmldocument.DocumentElement.AppendChild(xmldocument.ImportNode(xmlDigitalSignature, true));

    }

    public static void FirmarDocumentoXml(ref XmlDocument xmldocument, X509Certificate2 certificado, string referenciaUri, ref XmlNode NodeToAppend, string OuterXMLtoSign)
    {
        ////
        //// Cree el objeto SignedXml donde xmldocument
        //// representa el documento DTE preparado para
        //// ser firmado. Recuerde que debe ser abierto 
        //// con la propiedad PreserveWhiteSpace = true
        XmlDocument ExternXMLDoc = new XmlDocument();
        ExternXMLDoc.LoadXml(OuterXMLtoSign);
        SignedXml signedXml = new SignedXml(ExternXMLDoc);

        ////
        //// Agregue la clave privada al objeto signedXml
        signedXml.SigningKey = certificado.PrivateKey;

        ////
        //// Recupere el objeto signature desde signedXml
        Signature XMLSignature = signedXml.Signature;

        ////
        //// Cree la refrerencia al documento DTE
        //// recuerde que la referencia tiene el 
        //// formato '#reference'
        //// ejemplo '#DTE001'
        Reference reference = new Reference();
        reference.Uri = referenciaUri;

        ////
        //// Agregue la referencia al objeto signature
        XMLSignature.SignedInfo.AddReference(reference);
        KeyInfo keyInfo = new KeyInfo();
        keyInfo.AddClause(new RSAKeyValue((RSA)certificado.PrivateKey));

        ////
        //// Agregar información del certificado x509
        keyInfo.AddClause(new KeyInfoX509Data(certificado));
        XMLSignature.KeyInfo = keyInfo;

        ////
        //// Calcule la firma y recupere la representacion
        //// de la firma en un objeto xmlElement
        signedXml.ComputeSignature();
        XmlElement xmlDigitalSignature = signedXml.GetXml();

        ////
        //// Inserte la firma en el documento DTE
        NodeToAppend.AppendChild(xmldocument.ImportNode(xmlDigitalSignature, true));
        //xmldocument.DocumentElement.AppendChild(xmldocument.ImportNode(xmlDigitalSignature, true));

    }

    public static string GenerarTimbreDD(string DD, string pk, bool PurgeSIINameSpace = true)
    {
        //// //////////////////////////////////////////////////////////////////
        //// Generar timbre sobre los datos del tag DD utilizando la clave 
        //// privada suministrada por el SII en el archivo CAF
        //// //////////////////////////////////////////////////////////////////

        DD = DD.Replace(" xmlns=\"\"", "");
        if(PurgeSIINameSpace)
            DD = DD.Replace(" xmlns=\"http://www.sii.cl/SiiDte\"", "");
        DD = DD.Replace(System.Environment.NewLine, "");
        DD = DD.Replace("\n", "");

        ////
        //// Calcule el hash de los datos a firmar DD
        //// transformando la cadena DD a arreglo de bytes, luego con
        //// el objeto 'SHA1CryptoServiceProvider' creamos el Hash del
        //// arreglo de bytes que representa los datos del DD

        //fucks up for caracteres especiales, lets have a try with alternative encoding
        //ASCIIEncoding ByteConverter = new ASCIIEncoding();

        //wew lad
        Encoding ByteConverter = Encoding.GetEncoding("ISO-8859-1");

        byte[] bytesStrDD = ByteConverter.GetBytes(DD);
        byte[] HashValue = new SHA1CryptoServiceProvider().ComputeHash(bytesStrDD);

        ////
        //// Cree el objeto Rsa para poder firmar el hashValue creado
        //// en el punto anterior. La clase FuncionesComunes.crearRsaDesdePEM()
        //// Transforma la llave rivada del CAF en formato PEM a el objeto
        //// Rsa necesario para la firma.
        RSACryptoServiceProvider rsa = ParseExtensions.crearRsaDesdePEM(pk);

        ////
        //// Firme el HashValue ( arreglo de bytes representativo de DD )
        //// utilizando el formato de firma SHA1, lo cual regresará un nuevo 
        //// arreglo de bytes.
        byte[] bytesSing = rsa.SignHash(HashValue, "SHA1");

        ////
        //// Recupere la representación en base 64 de la firma, es decir de
        //// el arreglo de bytes 
        string FRMT1 = Convert.ToBase64String(bytesSing);

        return FRMT1;
    }

    

}

public class ISO8859StringWriter : StringWriter
{
    public override Encoding Encoding
    {
        get { return new Iso88591Encoding(); }
    }
}


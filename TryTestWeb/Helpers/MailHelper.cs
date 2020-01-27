using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using TryTestWeb;
using System.Net.Mime;
using System.IO;
using System.Data.Entity.Migrations;
using System.Xml;
using System.Diagnostics;


public class MailHelper
{
    public static string BodyCorreoIntercambio
    {
        get
        {
            StringBuilder bodyCorreoIntercambio = new StringBuilder();
            bodyCorreoIntercambio.AppendLine("FacturaLabs | Facturacion Electronica | Proceso de Intercambio");
            //bodyCorreoIntercambio.AppendLine("E-Factu | Facturacion Electronica | Proceso de Intercambio");
            bodyCorreoIntercambio.AppendLine("======================================");
            bodyCorreoIntercambio.AppendLine(string.Empty);
            bodyCorreoIntercambio.AppendLine("Adjunto puede encontrar los archivos XML correspondientes al proceso de intercambio");
            bodyCorreoIntercambio.AppendLine("entre contribuyentes autorizados, generado para usted por el emisor de referencia.");
            bodyCorreoIntercambio.AppendLine(string.Empty);
            bodyCorreoIntercambio.AppendLine("----------------------------------------------------------------------------");
            bodyCorreoIntercambio.AppendLine(string.Empty);
            bodyCorreoIntercambio.AppendLine("Este es un correo generado automaticamente, no conteste este correo ni realice consultas a esta direccion");
            return bodyCorreoIntercambio.ToString();
        }
    }

    public const string DireccionCorreoIntercambio = "intercambio@e-factu.cl";
    public const string SubjectCorreoIntercambio = "FacturaLabs / PROCESO INTERCAMBIO DTE";
    //public const string SubjectCorreoIntercambio = "e-factu / PROCESO INTERCAMBIO DTE";



   
  

    public static bool SendMail_Generic(List<string> TO, string FROM, string SUBJECT, string TITLE_TEXT, string MSG_BODY, bool IS_BODY_HTML)
    {
        //Verificar remitentes y destinatarios
        bool DestinosSonEMAIL = TO.All(r => r.IsValidEMail());
        bool OrigenEsEMAIL = FROM.IsValidEMail();
        if (OrigenEsEMAIL == false || DestinosSonEMAIL == false)
            return false;

        //Verificar que subject no sea vacio
        if (string.IsNullOrWhiteSpace(SUBJECT))
            return false;

        //Si BODY y TITLE son vacios, porque se estaria enviando un correo?
        if (string.IsNullOrWhiteSpace(TITLE_TEXT) && string.IsNullOrWhiteSpace(MSG_BODY))
            return false;

        string RutaTemplateMailPasswordRecovery = ParseExtensions.Get_AppData_Path("MailBody.html");
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(RutaTemplateMailPasswordRecovery);

        var node = doc.GetElementbyId("mainMessage");
        node.InnerHtml = MSG_BODY;

        node = doc.GetElementbyId("mainTitle");
        node.InnerHtml = TITLE_TEXT;

        //aca se envia el correo
        MailMessage mail = new MailMessage();
        mail.Subject = SUBJECT;
        mail.From = new MailAddress(FROM);
        foreach (string Destinatarios in TO)
        {
            mail.To.Add(Destinatarios);
        }
        mail.Body = doc.DocumentNode.OuterHtml;
        mail.IsBodyHtml = IS_BODY_HTML;

        SmtpClient smtp = new SmtpClient();
        NetworkCredential netCre = new NetworkCredential(FROM, "mierda42.5");

        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.EnableSsl = true;
        smtp.Host = "smtp.zoho.com";
        smtp.Port = 587;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = netCre;

        try
        {
            smtp.Send(mail);
            return true;
        }
        catch (Exception ex)
        {
            // Handle exception here 
            #if DEBUG
            Debug.Assert(false, "Mail failed");
            #endif
            return false;
        }
    }

    public static bool SendMail_ProblemaServicioSII()
    {
        return SendMail_Generic(new List<string> { "soporte@e-factu.cl" }, "soporte@e-factu.cl", "Problema Servicio SII", "Problema Servicio SII", DateTime.Now + ": Existe al parecer problemas comunicandose con los servicios del SII", false);
    }

    public static bool SendMail_Generic(List<string> TO, string FROM, string SUBJECT, string TITLE_TEXT, string MSG_BODY, bool IS_BODY_HTML, out string MensajeEnviadoOutput)
    {
        //variable de salida mensaje out
        MensajeEnviadoOutput = string.Empty;

        //Verificar remitentes y destinatarios
        bool DestinosSonEMAIL = TO.All(r => r.IsValidEMail());
        bool OrigenEsEMAIL = FROM.IsValidEMail();
        if (OrigenEsEMAIL == false || DestinosSonEMAIL == false)
            return false;

        //Verificar que subject no sea vacio
        if (string.IsNullOrWhiteSpace(SUBJECT))
            return false;

        //Si BODY y TITLE son vacios, porque se estaria enviando un correo?
        if (string.IsNullOrWhiteSpace(TITLE_TEXT) && string.IsNullOrWhiteSpace(MSG_BODY))
            return false;

        string RutaTemplateMailPasswordRecovery = ParseExtensions.Get_AppData_Path("MailBody.html");
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(RutaTemplateMailPasswordRecovery);

        var node = doc.GetElementbyId("mainMessage");
        node.InnerHtml = MSG_BODY;

        node = doc.GetElementbyId("mainTitle");
        node.InnerHtml = TITLE_TEXT;

        //aca se envia el correo
        MailMessage mail = new MailMessage();
        mail.Subject = SUBJECT;
        mail.From = new MailAddress(FROM);
        foreach (string Destinatarios in TO)
        {
            mail.To.Add(Destinatarios);
        }
        mail.Body = doc.DocumentNode.OuterHtml;
        mail.IsBodyHtml = IS_BODY_HTML;

        SmtpClient smtp = new SmtpClient();
        NetworkCredential netCre = new NetworkCredential(FROM, "mierda42.5");

        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.EnableSsl = true;
        smtp.Host = "smtp.zoho.com";
        smtp.Port = 587;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = netCre;

        try
        {
            smtp.Send(mail);
            MensajeEnviadoOutput = doc.DocumentNode.OuterHtml;
            return true;
        }
        catch (Exception ex)
        {
            // Handle exception here
            #if DEBUG
            Debug.Assert(false, "Mail failed");
            #endif
            return false;
        }
    }

   

    public static bool SendMailSoporte(string txtNombre, string txtEMail, string txtTelefono, string txtMsg, string txtNombreEmpresa)
    {

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("NOMBRE CONTACTO: "+txtNombre);
        sb.AppendLine(string.Empty);
        sb.AppendLine("NOMBRE EMPRESA: " + txtNombreEmpresa);
        sb.AppendLine(string.Empty);
        sb.AppendLine("EMAIL: " + txtEMail);
        sb.AppendLine();
        sb.AppendLine("TELEFONO: "+txtTelefono);
        sb.AppendLine();
        sb.AppendLine("MENSAJE:");
        sb.AppendLine(txtMsg);


        MailMessage mail = new MailMessage();
        mail.Subject = "WEBPAGE FORM cotizacion contacto";
        mail.From = new MailAddress("venta@e-factu.cl");
        mail.To.Add("venta@e-factu.cl");
        mail.Body = sb.ToString();
        mail.IsBodyHtml = false;

        SmtpClient smtp = new SmtpClient();
        NetworkCredential netCre = new NetworkCredential("venta@e-factu.cl", "mierda42.5");

        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.EnableSsl = true;
        smtp.Host = "smtp.zoho.com";
        smtp.Port = 587;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = netCre;

        try
        {
            smtp.Send(mail);
            return true;
        }
        catch (Exception ex)
        {
            // Handle exception here 
            return false;
        }
    }

    public static string GetMensajeRecuperarContraseña(string callbackUrl)
    {
        string RutaTemplateMailPasswordRecovery = ParseExtensions.Get_AppData_Path("MailBody.html");
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(RutaTemplateMailPasswordRecovery);
        var node = doc.GetElementbyId("mainMessage");
        node.InnerHtml = "Por favor reestablezca su contraseña haciendo click en el siguiente <a href=\"" + callbackUrl + "\">link</a>";

        return doc.DocumentNode.OuterHtml;
    }
}

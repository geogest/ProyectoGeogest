using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class ReportesImpagosLog
{
    public int ReportesImpagosLogID { get; set; }
    public int QuickEmisorModelID { get; set; }

    public string MensajeCorreoEnviado { get; set; }
    public string DireccionCorreoEnviada { get; set; }

    public ReportesImpagosLog()
    {

    }

    public ReportesImpagosLog(int QuickEmisorModelID, string MensajeCorreoEnviado, string DireccionCorreoEnviada)
    {
        this.QuickEmisorModelID = QuickEmisorModelID;
        this.MensajeCorreoEnviado = MensajeCorreoEnviado;
        this.DireccionCorreoEnviada = DireccionCorreoEnviada;
    }
}

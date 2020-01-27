using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class ImpuestosAdRelacionModel
    {
      public int ImpuestosAdRelacionModelID { get; set; }
      public int CodigoUnionImpuesto { get; set; }
      public int ImpuestosAdicionalesModelID { get; set; }
      public int ClienteContableModelID { get; set; }
      public int CodigoImpuesto { get; set; }
      public bool HaSidoConvertidoAVoucher { get; set; } = false;

    public ImpuestosAdRelacionModel()
        {

        }
    }

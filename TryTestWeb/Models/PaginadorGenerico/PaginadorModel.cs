using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class PaginadorModel : BasePaginadorModel
    {
        public List<VoucherModel> VoucherList { get; set; }
        public List<string[]> ResultStringArray { get; set; }
        public List<CatorceTerViewModel> LstCatorceTer {get;set;}
    }

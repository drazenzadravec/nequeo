//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nequeo.DataAccess.NequeoCompany.Edm
{
    using System;
    using System.Collections.Generic;
    
    public partial class InvoiceDetail
    {
        public int InvoiceDetailsID { get; set; }
        public int InvoiceID { get; set; }
        public double Hours { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public decimal GST { get; set; }
        public string Comments { get; set; }
    }
}
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
    
    public partial class BA
    {
        public int BASID { get; set; }
        public int CompanyID { get; set; }
        public System.DateTime BasDate { get; set; }
        public string DocumentID { get; set; }
        public string Reference { get; set; }
        public string EFTCode { get; set; }
        public decimal TotalSales { get; set; }
        public Nullable<decimal> ExportSales { get; set; }
        public Nullable<decimal> OtherGSTFreeSales { get; set; }
        public Nullable<decimal> CapitalPurchases { get; set; }
        public decimal NonCapitalPurchases { get; set; }
        public decimal GSTOnSales { get; set; }
        public decimal GSTOnPurchases { get; set; }
        public string PaymentOrRefund { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public decimal TotalWages { get; set; }
        public decimal PAYGWithheld { get; set; }
    }
}
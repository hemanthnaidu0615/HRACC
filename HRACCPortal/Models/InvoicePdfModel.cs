using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class InvoicePdfModel 
    {

        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyZip { get; set; }
        public string CompanyFax { get; set; }

        public int InvoiceIdPK { get; set; }
        public string InvoiceNumber { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Term { get; set; }
        public string MonthStartDate { get; set; }
        public string MonthEndDate { get; set; }
        public string InvoiceDate { get; set; }
        public string DueDate { get; set; }
        public int? ConsultantIdFK { get; set; }
        public int? ConsultantPositionIdFK { get; set; }
        public string RegularHours { get; set; }
        public string OvertimeHours { get; set; }
        public decimal? InvoiceAmount { get; set; }

        public string CustomerName { get; set; }
        public string CustomerContactPhone { get; set; }
        public string CustomerContactEmail { get; set; }
        public string CustomerContactAddress1 { get; set; }
        public string CustomerContactAddress2 { get; set; }
        public string CustomerContactCity { get; set; }
        public string CustomerContactState { get; set; }
        public string CustomerContactZip { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        public string PositionNumber { get; set; }
        public string PositionTitle { get; set; }
        public string PositionFamily { get; set; }
        public string PositionScopeVariant { get; set; }
        public string PurchaseOrderNo { get; set; }

        public decimal TotalAmount { get; set; }
        public int? Rate { get; set; }

        public string InvoiceNo { get; set; }
        public string DateAdded { get; set; }
        public string ConsultatntName { get; set; }

        public List<InvoicePdfModel> invDetails { get; set; }
    }
}
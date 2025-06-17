using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.ObjectModel
{
    public class InvoiceObjectModel
    {
        public string IndianTimeNow = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))).ToString();
        public DateTime todaydate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).Date;

        public int InvoiceIdPK { get; set; }
        public string InvoiceNumber { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string InvoiceDate { get; set; }
        public string DueDate { get; set; }
        public int? ConsultantIdFK { get; set; }
        public int? ConsultantPositionIdFK { get; set; }
        public string RegularHours { get; set; }
        public string OvertimeHours { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }

        public int? CustomerId { get; set; }
        public List<InvoiceObjectModel> InvoiceList { get; set; }
    }
}
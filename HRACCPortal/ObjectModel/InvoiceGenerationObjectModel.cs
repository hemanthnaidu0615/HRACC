using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.ObjectModel
{
    public class InvoiceGenerationObjectModel
    {
        public string IndianTimeNow = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))).ToString();
        public DateTime todaydate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).Date;

        public int InvoiceGenerationId { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceMonth { get; set; }
        public string InvoiceDueDate { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        
        public List<InvoiceGenerationObjectModel> InvoiceGenerationList { get; set;}
    }
}
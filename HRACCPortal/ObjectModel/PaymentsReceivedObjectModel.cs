using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.ObjectModel
{
    public class PaymentsReceivedObjectModel
    {
        public string IndianTimeNow = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))).ToString();
        public DateTime todaydate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).Date;

        public int PaymentsReceivedId { get; set; }
        public int CustomerIdFK { get; set; }

        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceAmount { get; set; }
        public string InvoiceDueDate { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }

        public string Status { get;set; }
        public List<PaymentsReceivedObjectModel> PaymentsReceivedList { get; set; }

    }
}
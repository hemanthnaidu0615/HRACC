using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.ObjectModel
{
    public class BalanceSheetObjectModel
    {
        public string IndianTimeNow = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))).ToString();
        public DateTime todaydate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).Date;

        public int BalanceSheetId { get; set; }
        public int CustomerIdFK { get; set; }
        public string CustomerName { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceAmount { get; set; }
        public string PaymentReceived { get; set; }
        public string Balance { get; set; }
        public bool? Status { get; set; }
        public string InactiveDate { get; set; }
        public string InactiveReason { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public List<BalanceSheetObjectModel> BalanceSheetList { get; set; }
    }
}
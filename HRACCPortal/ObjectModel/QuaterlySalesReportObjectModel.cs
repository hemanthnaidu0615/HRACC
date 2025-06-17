using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.ObjectModel
{
    public class QuaterlySalesReportObjectModel
    {
        public string IndianTimeNow = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))).ToString();
        public DateTime todaydate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).Date;

        public int QuaterlySalesReportIdPK { get; set; }
        public int CustomerIdFK { get; set; }
        public string CustomerName { get; set; }
        public string Quarter { get; set; }
        public string Month { get; set; }
        public string ConsultantName { get; set; }
        public string InvoiceAmount { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public List<QuaterlySalesReportObjectModel> QuaterlySalesReportList { get; set; }


    }

}
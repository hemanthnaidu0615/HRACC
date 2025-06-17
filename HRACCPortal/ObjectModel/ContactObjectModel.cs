using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.ObjectModel
{
    public class ContactObjectModel
    
        {

        public string IndianTimeNow = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))).ToString();
        public DateTime todaydate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).Date;


        public int ContactIdPK { get; set; }
            public string ContactName { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public string ContactAddress1 { get; set; }
            public string ContactAddress2 { get; set; }
            public string ContactCity { get; set; }
            public string ContactState { get; set; }
        public string ContactType { get; set; }
        public string ContactZip { get; set; }
            public string ContactFEID { get; set; }
            public string DateAdded { get; set; }
            public string DateUpdated { get; set; }
            public string AddedBy { get; set; }
            public string UpdatedBy { get; set; }
            public bool isActive { get; set; }
        
    }
}

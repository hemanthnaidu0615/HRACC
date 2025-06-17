using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.ObjectModel
{
    public class WorkerObjectModel
    {
        public string IndianTimeNow = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))).ToString();
        public DateTime todaydate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).Date;

        public int WorkerIdPK { get; set; }
        public string WorkerName { get; set; }
        public string WorkerPhone { get; set; }
        public string WorkerEmail { get; set; }
        public string WorkerTitle { get; set; }
        public string WorkerSalary { get; set; }
        public string WorkerAddress1 { get; set; }
        public string WorkerAddress2 { get; set; }
        public string WorkerCity { get; set; }
        public string WorkerState { get; set; }
        public string WorkerZip { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool isActive { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRACCPortal.ObjectModel
{
    public class PositionObjectModel
    {
        public string IndianTimeNow = (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))).ToString();
        public DateTime todaydate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).Date;

        //position
        public int PositionIdPK { get; set; }
        public int CustomerIdFK { get; set; }
        public string CustomerName { get; set; }
        public string PositionNumber { get; set; }
        public string PositionTitle { get; set; }
        public string PositionFamily { get; set; }
        public string PositionScopeVariant { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PurchaseRequisitionNo { get; set; }
        public bool? Status { get; set; }
        public string InactiveDate { get; set; }
        public string InactiveReason { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }

  
        public List<PositionObjectModel> PositionList { get; set; }

        //position rate
        public int PositionRateIdPK { get; set; }
        public int PositionIdFK { get; set; }
        public string FiscalYearStart { get; set; }

        public string FiscalYearEnd { get; set; }
        public string FiscalYearAbbrv { get; set; }
        public int? Rate { get; set; }
        public int? OvertimeRate { get; set; }
        public List<PositionObjectModel> PositionRateList { get; set; }

    }
}
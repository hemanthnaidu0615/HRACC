using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class ApproveTimesheetsViewModel
    {
        public List<TimesheetViewModel> PendingTimesheets { get; set; }
        public List<TimesheetViewModel> ApprovedTimesheets { get; set; }
        public List<TimesheetViewModel> RejectedTimesheets { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
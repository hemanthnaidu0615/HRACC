using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class TimesheetDetail
    {
       
            public int DetailId { get; set; }
            public int TimesheetId { get; set; }
            public string WorkDate { get; set; }
            public string Description { get; set; }
            public decimal Hours { get; set; }
            public virtual Timesheet Timesheet { get; set; }
            public decimal OTHours { get; set; }

    }
}
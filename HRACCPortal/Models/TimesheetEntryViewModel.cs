using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class TimesheetEntryViewModel
    {
        
            public DateTime WorkDate { get; set; }
            public string Description { get; set; }
            public decimal Hours { get; set; }
        
    }
}
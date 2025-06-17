using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class TimesheetViewModel
    {

            public int TimesheetId { get; set; }
            public int ConsultantId { get; set; }
            
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal TotalHours { get; set; }
            public string Approval { get; set; }
            public string CustomerName { get; set; } // ✅ Customer Name
            public string EmployerName { get; set; }
            public DateTime CreatedAt { get; set; }
            
        public List<TimesheetEntryViewModel> Entries { get; set; } = new List<TimesheetEntryViewModel>();
        
    }
}
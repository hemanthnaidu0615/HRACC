using HRACCPortal.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class Timesheet
    {
        public int TimesheetId { get; set; }
        public int ConsultantId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalHours { get; set; }
        public virtual ICollection<TimesheetDetail> TimesheetDetails { get; set; }
        public string EmailStatus { get; set; }
    }
}
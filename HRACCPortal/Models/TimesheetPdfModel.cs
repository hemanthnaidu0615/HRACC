using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class TimesheetPdfModel
    {
        // Company Information
        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyZip { get; set; }
        public string CompanyFax { get; set; }

        // Consultant Information
        public string ConsultantName { get; set; }
        public string PositionTitle { get; set; }

        // Timesheet Information
        public int TimesheetId { get; set; }
        public string MonthStartDate { get; set; }
        public string MonthEndDate { get; set; }
        public decimal TotalHours { get; set; }

        // List of Timesheet Entries
        public List<TimesheetEntryModel> TimesheetEntries { get; set; }
    }

    public class TimesheetEntryModel
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal Hours { get; set; }
    }
}
using HRACCPortal.Edmx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    [Table("EmployeeEmployer")]
    public class EmployeeEmployerModel
    {

        [Key]
        public int EmployeeEmployerId { get; set; } // Primary Key

        [ForeignKey("Employee")] // Foreign Key to Employee table
        public int EmployeeIdFK { get; set; }

        [ForeignKey("Employer")] // Foreign Key to Employer table
        public int EmployerIdFK { get; set; }

        public string DateAdded { get; set; } // Date when the record was added

        // Navigation properties (optional, for relationships)
        public virtual Employee Employee { get; set; } // Navigation property for Employee
        public virtual Employer Employer { get; set; }
    }
}
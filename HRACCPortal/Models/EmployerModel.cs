using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class EmployerModel
    {
        public int EmployerIdPK { get; set; }
        public string EmployerName { get; set; }
        public string EmployerContactPhone { get; set; }
        public string EmployerContactEmail { get; set; }
        public string EmployerContactAddress1 { get; set; }
        public string EmployerContactAddress2 { get; set; }
        public string EmployerContactCity { get; set; }
        public string EmployerContactState { get; set; }
        public string EmployerContactZip { get; set; }
        public string EmployerFEID { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool isActive { get; set; }
    }
}
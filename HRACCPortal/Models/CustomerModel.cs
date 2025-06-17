using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class CustomerModel
    {
        public int CustomerIdPK { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContactPhone { get; set; }
        public string CustomerContactEmail { get; set; }
        public string CustomerContactAddress1 { get; set; }
        public string CustomerContactAddress2 { get; set; }
        public string CustomerContactCity { get; set; }
        public string CustomerContactState { get; set; }
        public string CustomerContactZip { get; set; }
        public string CustomerTerm { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool isActive { get; set; }
        public string CustomerFEID { get; set; }
    }
}
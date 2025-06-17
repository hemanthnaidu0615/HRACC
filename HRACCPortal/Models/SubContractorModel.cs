using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class SubContractorModel
    {
        public int SubContractorIdPK { get; set; }
        public string SubContractorName { get; set; }
        public string SubContractorContactPhone { get; set; }
        public string SubContractorContactEmail { get; set; }
        public string SubContractorContactAddress1 { get; set; }
        public string SubContractorContactAddress2 { get; set; }
        public string SubContractorContactCity { get; set; }
        public string SubContractorContactState { get; set; }
        public string SubContractorContactZip { get; set; }
        public string SubContractorTerm { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool isActive { get; set; }

        public string SubContractorFEID { get; set; }
    }
}
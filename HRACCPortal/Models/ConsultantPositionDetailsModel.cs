using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class ConsultantPositionDetailsModel
    {         
        public int ConsultantPositionIdPK { get; set; }
        public int ConsultantIdFK { get; set; }
        public int PositionIdFK { get; set; }
        public int PositionRateIdFK { get; set; }
        public string PositionStartDate { get; set; }
        public string PositionEndDate { get; set; }       
        public string PositionActive { get; set; }
        public string DateAdded { get; set; }
        public string DateUpdated { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
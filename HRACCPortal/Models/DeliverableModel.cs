using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class DeliverableModel
    {
        public int DeliverableId { get; set; }

        public int PositionId { get; set; }

        public string DeliverableNumber { get; set; }

        public string DeliverableDescription { get; set; }

        // Optional: Navigation property if needed
        public string PositionName { get; set; }
    }
}
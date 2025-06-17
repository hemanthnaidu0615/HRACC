using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class ChequeDetailViewModel
    {
        public int ChequeId { get; set; }
        public int PaymentId { get; set; }
        public string ChequeNumber { get; set; }
        public decimal ChequeAmount { get; set; }
        public string DateReceived { get; set; }

    }
}
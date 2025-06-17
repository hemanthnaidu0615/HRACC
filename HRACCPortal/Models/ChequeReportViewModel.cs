using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class ChequeReportViewModel
    {
        public int CNumber { get; set; }
        public string ChequeNumber { get; set; }
        public decimal? ChequeAmount { get; set; }
        public DateTime? DateReceived { get; set; }
        public decimal AmountPaidSoFar { get; set; }
        public decimal BalanceRemaining { get; set; }
    }
}
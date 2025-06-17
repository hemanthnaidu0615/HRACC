using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class ChequeHistoryItem
    {
        
            public int CNumber { get; set; }
            public string ChequeNumber { get; set; }
            public decimal Amount { get; set; }
            public DateTime DateReceived { get; set; }
            public decimal BalanceAfter { get; set; }
        
    }
}
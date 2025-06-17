using HRACCPortal.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class PaymentDetailedReportViewModel
    {
        public PaymentsReceived Payment { get; set; }
        public List<ChequeHistoryItem> ChequeHistory { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class CustomerQuarterBalanceViewModel
    {
        public int CustomerId { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }

        public decimal TotalInvoiceAmount { get; set; }
        public decimal TotalAmountPaid { get; set; }

        public decimal BalanceAmount => TotalInvoiceAmount - TotalAmountPaid;
    }
}
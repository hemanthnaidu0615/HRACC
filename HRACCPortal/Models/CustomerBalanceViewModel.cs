using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class CustomerBalanceViewModel
    {

       
            public int CustomerId { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }

            public decimal TotalInvoiceAmount { get; set; }
            public decimal TotalAmountPaid { get; set; }

            public decimal BalanceAmount => TotalInvoiceAmount - TotalAmountPaid;
        


    }

}
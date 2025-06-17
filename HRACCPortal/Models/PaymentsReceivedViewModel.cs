using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class PaymentsReceivedViewModel
    {
        public int PaymentsReceivedId { get; set; }
        public int CustomerIdFK { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceAmount { get; set; }
        public string InvoiceDueDate { get; set; }

        public string ChequeNumber { get; set; }
        public decimal ChequeAmount { get; set; }
        public string DateReceived { get; set; }

        public string Status { get; set; }

    }
}
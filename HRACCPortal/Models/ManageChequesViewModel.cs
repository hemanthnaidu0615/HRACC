using HRACCPortal.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class ManageChequesViewModel
    {
       
            public PaymentsReceived Payment { get; set; }
            public List<ChequeDetail> Cheques { get; set; }

            public ChequeDetailViewModel NewCheque { get; set; }
    }
}
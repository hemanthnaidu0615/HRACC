using HRACCPortal.Edmx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class InvoiceFrom
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Subcontractor is required")]
        [Display(Name = "Subcontractor")]
        public int SubcontractorId { get; set; }

        [Required(ErrorMessage = "Invoice Number is required")]
        [StringLength(50, ErrorMessage = "Invoice Number cannot exceed 50 characters")]
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Required(ErrorMessage = "Invoice Amount is required")]
        [Range(0.01, 10000000, ErrorMessage = "Amount must be greater than zero")]
        [Display(Name = "Invoice Amount")]
        public decimal InvoiceAmount { get; set; }

        [Required(ErrorMessage = "Invoice Date is required")]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        [Required(ErrorMessage = "Payment Status is required")]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation property
        public virtual SubContractor Subcontractor { get; set; }
    }

    public class InvoiceFromViewModel
    {
        public int Id { get; set; }
        public string SubcontractorName { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string FormattedAmount => InvoiceAmount.ToString("C");
        public DateTime InvoiceDate { get; set; }
        public string FormattedDate => InvoiceDate.ToString("d");
        public string PaymentStatus { get; set; }

        public string StatusClass
        {
            get
            {
                if (PaymentStatus == "Paid")
                    return "badge-light-success";
                else if (PaymentStatus == "Partially Paid")
                    return "badge-light-warning";
                else
                    return "badge-light-danger";
            }
        }

        public virtual SubContractor Subcontractor { get; set; }

    }
}
using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class InvoiceFromController : Controller
    {
        private readonly HRACCDBEntities entities = new HRACCDBEntities();

        public ActionResult Index()
        {

            var invoices = entities.InvoicesFrom
               .Include("SubContractor")
               .Select(i => new InvoiceFromViewModel
               {
                   Id = i.Id,
                   SubcontractorName = i.SubContractor.SubContractorName,  //  Access navigation property
                   InvoiceNumber = i.InvoiceNumber,
                   InvoiceAmount = i.InvoiceAmount,
                   InvoiceDate = i.InvoiceDate,
                   PaymentStatus = i.PaymentStatus
               }).ToList();



            ViewBag.Subcontractors = entities.SubContractors
             .ToList() // Materialize to memory first
             .Select(s => new SelectListItem
             {
                 Value = s.SubContractorIdPK.ToString(),
                 Text = s.SubContractorName
             }).ToList();

            return View(invoices);
        }



    }
}
using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class InvoiceToController : Controller
    {
        private readonly HRACCDBEntities entities = new HRACCDBEntities();

        public ActionResult Index()
        {
            var invoices = entities.InvoicesTo
               .Include("SubContractor")
               .Select(i => new InvoiceToViewModel
               {
                   Id = i.Id,
                   SubcontractorName = i.SubContractor.SubContractorName,
                   InvoiceNumber = i.InvoiceNumber,
                   InvoiceAmount = i.InvoiceAmount,
                   InvoiceDate = i.InvoiceDate,
                   PaymentStatus = i.PaymentStatus
               }).ToList();

            ViewBag.Subcontractors = entities.SubContractors
                .ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.SubContractorIdPK.ToString(),
                    Text = s.SubContractorName
                }).ToList();

            return View(invoices);
        }

        [HttpPost]

        public ActionResult Create(HRACCPortal.Models.InvoiceTo model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Map model to EDMX entity
                    var edmxInvoice = new HRACCPortal.Edmx.InvoicesTo
                    {
                        SubcontractorId = model.SubcontractorId,
                        InvoiceNumber = model.InvoiceNumber,
                        InvoiceAmount = model.InvoiceAmount,
                        InvoiceDate = model.InvoiceDate,
                        PaymentStatus = model.PaymentStatus,
                        CreatedDate = DateTime.Now
                    };

                    entities.InvoicesTo.AddObject(edmxInvoice);
                    entities.SaveChanges();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errors = new List<string> { ex.Message } });
                }
            }

            // Collect model validation errors
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, errors });
        }

    }
}
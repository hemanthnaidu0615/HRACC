using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class InvoiceGenerationController : Controller
    {
        // GET: InvoiceGeneration
        public ActionResult Index()
        {
            return View();
        }

        private readonly InvoiceGenerationModel model;
        private readonly clsCrud cls;

        public InvoiceGenerationController()
        {
            model = new InvoiceGenerationModel();
            cls = new clsCrud();
        }
        // GET: Invoice
        public ActionResult ViewInvoiceGeneration()
        {
            model.GetInvoiceGenerationList();
            return View(model);
        }
        public ActionResult GetInvoiceGeneration(int id)
        {
            InvoicePdfModel invoicePdfModel = cls.GenratePdf(id);

            return View(invoicePdfModel);
        }
        //POST: Add/Edit Position Rate
        [HttpPost]
        public ActionResult AddEditInvoiceGeneration(InvoiceGenerationModel model)
        {
            try
            {
                string status = model.AddEditInvoiceGeneration(model);
                return Json(new { message = status, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message, JsonRequestBehavior.AllowGet });
            }
        }

        //POST: Get Position Rate Edit View
        public ActionResult EditInvoiceGeneration(int? id)
        {
            try
            {
                InvoiceGenerationModel invoicegenerationData = model.GetInvoiceGenerationDetailsById(id);

                return Json(new { invoicegenerationData = invoicegenerationData, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return View(e.Message);
            }

        }
    }
}
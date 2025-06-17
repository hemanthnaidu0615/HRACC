using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly InvoiceModel model;
        private readonly clsCrud cls;
        private readonly HRACCDBEntities entities = new HRACCDBEntities();

        public InvoiceController()
        {
            model = new InvoiceModel();
            cls = new clsCrud();
        }
        // GET: Invoice
        public ActionResult ViewInvoice()
        {
            model.GetInvoiceList();
            return View(model);
        }
        public ActionResult GetInvoice(int id)
        {
            InvoicePdfModel invoicePdfModel = cls.GenratePdf(id);

            return View(invoicePdfModel);
        }
        //POST: Add/Edit Position Rate
        [HttpPost]
        public ActionResult AddEditInvoice(InvoiceModel model)
        {
            try
            {

                string status = model.AddEditInvoice(model);
                var savedInvoice = entities.Invoices
                         .FirstOrDefault(x => x.InvoiceNumber == model.InvoiceNumber);

                int invoiceIdPk = savedInvoice != null ? savedInvoice.InvoiceIdPK : 0;
                return Json(new { message = status, invoiceId = invoiceIdPk, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message, JsonRequestBehavior.AllowGet });
            }
        }

        //POST: Get Position Rate Edit View
        public ActionResult EditInvoice(int? id)
        {
            try
            {
                InvoiceModel invoiceData = model.GetInvoiceDetailsById(id);

                return Json(new { invoiceData = invoiceData, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return View(e.Message);
            }

        }


        [HttpPost]
        public JsonResult AcceptInvoice(int id)
        {
            var invoice = entities.Invoices.FirstOrDefault(x => x.InvoiceIdPK == id);
            if (invoice != null)
            {
                invoice.Status = "Accepted";
                entities.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult RejectInvoice(int id)
        {
            var invoice = entities.Invoices.FirstOrDefault(x => x.InvoiceIdPK == id);
            if (invoice != null)
            {
                entities.Invoices.DeleteObject(invoice);
                entities.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


    }
}
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class InvoiceSubmissionController : Controller
    {
        // GET: InvoiceSubmission
        public ActionResult Index()
        {
            return View();
        }

        private readonly InvoiceSubmissionModel ismodel;

        public InvoiceSubmissionController()
        {
            ismodel = new InvoiceSubmissionModel();
        }
        // GET: Position
        [HttpGet]
        public ActionResult ViewInvoiceSubmission()
        {
            ismodel.GetInvoiceSubmissionList();
            return View(ismodel);
        }

        //POST: Add Position
        [HttpPost]
        public ActionResult AddInvoiceSubmission(InvoiceSubmissionModel ismodel)
        {
            try
            {
                string status = ismodel.AddEditInvoiceSubmission(ismodel);
                return Json(new { message = status, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message, JsonRequestBehavior.AllowGet });
            }

        }
        //POST: Get Position for Edit View
        public ActionResult EditInvoiceSubmission(int? id)
        {
            try
            {
                InvoiceSubmissionModel invoicesubmissionData = ismodel.GetInvoiceSubmissionDetailsById(id);

                return Json(new { invoicesubmissionData = invoicesubmissionData, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return View(e.Message);
            }

        }

       
    }
}
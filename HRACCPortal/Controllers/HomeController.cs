using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using System.Globalization;

namespace HRACCPortal.Controllers
{
    public class HomeController : Controller
    {
        public clsCrud cls;
        public LoginModel lmodel;
        public HomeController()
        {
            cls = new clsCrud();
            lmodel = new LoginModel();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ValidateLogin(LoginModel model)
        {
            try
            {
                string validateLogin = lmodel.ValidateLoginDetails(model);
                return Json(new { message = validateLogin, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message, JsonRequestBehavior.AllowGet });
            }
        }
        public ActionResult CreateInvoice(int id)
        {
            InvoicePdfModel invoicePdfModel = cls.GenratePdf(id);
             
            return View(invoicePdfModel);
        }
        public ActionResult UrlAsPDF(int id)
        {

            //this will generate google home page to in a pdf doc
            string url = "/Home/CreateInvoice/"+id;
            InvoicePdfModel invoicePdfModel = cls.GenratePdf(id);

            //string inv = "Invoice to FDOH from_"+ invoicePdfModel.CompanyName + "_"+ invoicePdfModel.invDetails[0].ConsultatntName + "_"+ CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(invoicePdfModel.invDetails[0].Month)) + invoicePdfModel.invDetails[0].Year +"_"+ invoicePdfModel.invDetails[0].PurchaseOrderNo;

            string inv = "Invoice";
            return new Rotativa.UrlAsPdf(url)
            {
               // FileName = "urltest.pdf",
                FileName =inv+".pdf",
            };
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout()
        {
            return RedirectToAction("Index");
        }
        public ActionResult Test()
        {
            return View();
        }
    }
}
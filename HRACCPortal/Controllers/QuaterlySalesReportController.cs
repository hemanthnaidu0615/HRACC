using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class QuaterlySalesReportController : Controller
    {
        private readonly QuaterlySalesReportModel qsrmodel;
        private readonly HRACCDBEntities entities = new HRACCDBEntities();
        public QuaterlySalesReportController()
        {
            qsrmodel = new QuaterlySalesReportModel();
        }
        // GET: Position
        [HttpGet]
        public ActionResult ViewQuaterlySalesReport()
        {
            qsrmodel.GetQuaterlySalesReportList();
            return View(qsrmodel);
        }

        //POST: Add Position
        [HttpPost]
        public ActionResult AddQuaterlySalesReport(QuaterlySalesReportModel qsrmodel)
        {
            try
            {
                string status = qsrmodel.AddEditQuaterlySalesReport(qsrmodel);
                return Json(new { message = status, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message, JsonRequestBehavior.AllowGet });
            }

        }
        //POST: Get Position for Edit View
        public ActionResult EditQuaterlySalesReport(int? id)
        {
            try
            {
                QuaterlySalesReportModel quaterlysalesreportData = qsrmodel.GetQuaterlySalesReportDetailsById(id);

                return Json(new { quaterlysalesreportData = quaterlysalesreportData, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return View(e.Message);
            }

        }

        public ActionResult QuarterlyReport()
        {
            // Step 1: Get all valid invoices
            var validInvoices = entities.Invoices
                .Where(i => i.CustomerId != null)
                .Select(i => new
                {
                    i.CustomerId,
                    i.Year,
                    i.Month,
                    i.InvoiceNumber,
                    i.InvoiceAmount
                })
                .ToList();

            // Step 2: Get all valid payments
            var payments = entities.PaymentsReceiveds
                .Where(p => p.Status != "Cancelled" && p.CustomerIdFK != null)
                .Select(p => new
                {
                    p.CustomerIdFK,
                    p.InvoiceNumber,
                    p.AmountPaid,
                    p.BalanceAmount
                })
                .ToList();

            // Step 3: Group by CustomerId, Year, Quarter
            var result = validInvoices
                .GroupBy(inv => new
                {
                    inv.CustomerId,
                    inv.Year,
                    Quarter = (Convert.ToInt32(inv.Month) - 1) / 3 + 1
                    // Jan-Mar => Q1, Apr-Jun => Q2, etc.
                })
                .Select(g =>
                {
                    var invoiceNumbers = g.Select(x => x.InvoiceNumber).ToList();
                    var totalInvoiceAmount = g.Sum(x => x.InvoiceAmount);

                    var relatedPayments = payments
                        .Where(p => invoiceNumbers.Contains(p.InvoiceNumber))
                        .ToList();

                    var totalAmountPaid = relatedPayments.Sum(p => p.AmountPaid ?? 0);

                    return new CustomerQuarterBalanceViewModel
                    {
                        CustomerId = g.Key.CustomerId ?? 0,
                        Year = int.TryParse(g.Key.Year?.ToString(), out int y) ? y : 0,
                        Quarter = g.Key.Quarter,
                        TotalInvoiceAmount = (decimal)totalInvoiceAmount,
                        TotalAmountPaid = totalAmountPaid
                        // BalanceAmount computed in model
                    };
                })
                .OrderBy(x => x.CustomerId)
                .ThenBy(x => x.Year)
                .ThenBy(x => x.Quarter)
                .ToList();

            return View(result);
        }

    }
}
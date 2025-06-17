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
    public class BalanceSheetController : Controller
    {
        private readonly BalanceSheetModel bsmodel;
        private readonly HRACCDBEntities entities = new HRACCDBEntities();
        public BalanceSheetController()
        {
            bsmodel = new BalanceSheetModel();
        }
        // GET: BalanceSheet
        [HttpGet]
        public ActionResult ViewBalanceSheet()
        {
            bsmodel.GetBalanceSheetList();
            return View(bsmodel);
        }


        //    public ActionResult Index()
        //    {
        //        // Step 1: Get all invoices and group by CustomerId, Year, Month
        //        var invoiceData = entities.Invoices
        //            .Where(i => i.Status != "Cancelled" && i.CustomerId != null)
        //            .GroupBy(i => new { i.CustomerId, i.Year, i.Month })
        //            .Select(g => new
        //            {
        //                g.Key.CustomerId,
        //                g.Key.Year,
        //                g.Key.Month,
        //                TotalInvoiceAmount = g.Sum(x => x.InvoiceAmount)
        //            });

        //        // Step 2: Get payments with proper date handling
        //        var paymentData = entities.PaymentsReceiveds
        //            .Where(p => p.Status != "Cancelled" && p.CustomerIdFK != null)
        //            .Join(entities.Invoices,
        //                p => new { p.InvoiceNumber, CustomerId = (int?)p.CustomerIdFK },
        //                i => new { i.InvoiceNumber, CustomerId = i.CustomerId },
        //                (p, i) => new { p, i.Year, i.Month, i.CustomerId })

        //            .GroupBy(x => new { x.CustomerId, x.Year, x.Month })
        //            .Select(g => new
        //            {
        //                g.Key.CustomerId,
        //                g.Key.Year,
        //                g.Key.Month,
        //                TotalAmountPaid = g.Sum(x => x.p.AmountPaid ?? 0)
        //            });

        //        var invoiceDataList = invoiceData.ToList();     // Materialize from DB
        //        var paymentDataList = paymentData.ToList();
        //        var combined = invoiceDataList
        //.Select(i => new CustomerBalanceViewModel
        //{
        //    CustomerId = i.CustomerId.Value,
        //    Year = int.TryParse(i.Year.ToString(), out int yearVal) ? yearVal : 0,
        //    Month = int.TryParse(i.Month.ToString(), out int monthVal) ? monthVal : 0,
        //    TotalInvoiceAmount = (decimal)i.TotalInvoiceAmount,
        //    TotalAmountPaid = 0
        //})
        //.Concat(paymentDataList.Select(p => new CustomerBalanceViewModel
        //{
        //    CustomerId = p.CustomerId.Value,
        //    Year = int.TryParse(p.Year.ToString(), out int yearVal) ? yearVal : 0,
        //    Month = int.TryParse(p.Month.ToString(), out int monthVal) ? monthVal : 0,
        //    TotalInvoiceAmount = 0,
        //    TotalAmountPaid = p.TotalAmountPaid
        //}))
        //.GroupBy(x => new { x.CustomerId, x.Year, x.Month })
        //.Select(g => new CustomerBalanceViewModel
        //{
        //    CustomerId = g.Key.CustomerId,
        //    Year = g.Key.Year,
        //    Month = g.Key.Month,
        //    TotalInvoiceAmount = g.Sum(x => x.TotalInvoiceAmount),
        //    TotalAmountPaid = g.Sum(x => x.TotalAmountPaid)
        //    // BalanceAmount is a computed property, no need to set
        //})
        //.OrderBy(x => x.CustomerId)
        //.ThenBy(x => x.Year)
        //.ThenBy(x => x.Month);



        //        return View(combined.ToList());
        //    }

        public ActionResult Index()
        {
            // Get all valid invoices
            var validInvoices = entities.Invoices
                .Where(i =>i.CustomerId != null)
                .Select(i => new
                {
                    i.CustomerId,
                    i.Year,
                    i.Month,
                    i.InvoiceNumber,
                    i.InvoiceAmount
                })
                .ToList();

            // Get all valid payments
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

            // Join invoices with their payments based on InvoiceNumber
            var result = validInvoices
                .GroupBy(inv => new { inv.CustomerId, inv.Year, inv.Month })
                .Select(g =>
                {
                    var invoiceNumbers = g.Select(x => x.InvoiceNumber).ToList();
                    var totalInvoiceAmount = g.Sum(x => x.InvoiceAmount);

                    var relatedPayments = payments
                        .Where(p => invoiceNumbers.Contains(p.InvoiceNumber))
                        .ToList();

                    var totalAmountPaid = relatedPayments.Sum(p => p.AmountPaid ?? 0);

                    return new CustomerBalanceViewModel
                    {
                        CustomerId = g.Key.CustomerId ?? 0,
                        Year = int.TryParse(g.Key.Year?.ToString(), out int y) ? y : 0,
                        Month = int.TryParse(g.Key.Month?.ToString(), out int m) ? m : 0,
                        TotalInvoiceAmount = (decimal)totalInvoiceAmount,
                        TotalAmountPaid = totalAmountPaid
                        // BalanceAmount = computed inside ViewModel
                    };
                })
                .OrderBy(x => x.CustomerId)
                .ThenBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return View(result);
        }

        //POST: Add BalanceSheet
        [HttpPost]
        public ActionResult AddBalanceSheet(BalanceSheetModel bsmodel)
        {
            try
            {
                string status = bsmodel.AddEditBalanceSheet(bsmodel);
                return Json(new { message = status, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message, JsonRequestBehavior.AllowGet });
            }

        }
        //POST: Get BalanceSheet for Edit View
        public ActionResult EditBalanceSheet(int? id)
        {
            try
            {
                BalanceSheetModel balancesheetData = bsmodel.GetBalanceSheetDetailsById(id);

                return Json(new { balancesheetData = balancesheetData, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return View(e.Message);
            }

        }
    }
}
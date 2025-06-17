using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class PaymentsReceivedController : Controller
    {
        // GET: BalanceSheet
        public HRACCDBEntities entities;
        public ActionResult Index()
        {
            return View();
        }

        private readonly PaymentsReceivedModel prmodel;

        public PaymentsReceivedController()
        {
            entities = new HRACCDBEntities();
            prmodel = new PaymentsReceivedModel();
        }
        // GET: Position
        [HttpGet]
        public ActionResult ViewPaymentsReceived()
        {
            prmodel.GetPaymentsReceivedList();
            return View(prmodel);
        }

        //POST: Add Position

        [HttpPost]
        public ActionResult AddPaymentsReceived(PaymentsReceivedViewModel prvm)
        {
            try
            {
                PaymentsReceived payment;
                var currentDate = DateTime.Now;
                // 1️⃣ Create or update payment
                if (prvm.PaymentsReceivedId > 0)
                {
                    payment = entities.PaymentsReceiveds
                        .FirstOrDefault(p => p.PaymentsReceivedId == prvm.PaymentsReceivedId);
                    if (payment == null)
                        return Json(new { message = "Payment not found" });

                    payment.CustomerIdFK = prvm.CustomerIdFK;
                    payment.InvoiceNumber = prvm.InvoiceNumber;
                    payment.InvoiceAmount = prvm.InvoiceAmount;
                    payment.InvoiceDueDate = prvm.InvoiceDueDate;
                    payment.DateUpdated = currentDate.ToString("yyyy-MM-dd");
                    payment.UpdatedBy = "ADMIN";
                }
                else
                {
                    payment = new PaymentsReceived
                    {
                        CustomerIdFK = prvm.CustomerIdFK,
                        InvoiceNumber = prvm.InvoiceNumber,
                        InvoiceAmount = prvm.InvoiceAmount,
                        InvoiceDueDate = prvm.InvoiceDueDate,
                        DateAdded = currentDate.ToString("yyyy-MM-dd"),
                        DateUpdated = currentDate.ToString("yyyy-MM-dd"),
                        AddedBy = "ADMIN",
                        UpdatedBy = "ADMIN"
                    };
                    entities.PaymentsReceiveds.AddObject(payment);
                }

                entities.SaveChanges();

                // 2️⃣ Handle first cheque only (IsFirstCheque = true)
                HRACCPortal.Edmx.ChequeDetail firstCheque = entities.ChequeDetails
                             .FirstOrDefault(c => c.PaymentsReceivedId == payment.PaymentsReceivedId && c.CNumber == 1);



                if (firstCheque != null)
                {
                    // Update existing first cheque
                    firstCheque.ChequeNumber = prvm.ChequeNumber;
                    firstCheque.ChequeAmount = prvm.ChequeAmount;
                    firstCheque.DateReceived = Convert.ToDateTime(prvm.DateReceived);
                }
                else
                {
                    // Add new first cheque
                    var cheque = new HRACCPortal.Edmx.ChequeDetail
                    {
                        PaymentsReceivedId = payment.PaymentsReceivedId,
                        ChequeNumber = prvm.ChequeNumber,
                        ChequeAmount = prvm.ChequeAmount,
                        DateReceived = Convert.ToDateTime(prvm.DateReceived),
                        CNumber = 1
                    };
                    entities.ChequeDetails.AddObject(cheque);
                }

                entities.SaveChanges();

                var totalPaid = entities.ChequeDetails
                  .Where(c => c.PaymentsReceivedId == payment.PaymentsReceivedId)
                  .Sum(c => (decimal?)c.ChequeAmount) ?? 0;

                // Safely parse InvoiceAmount string to decimal
                decimal invoiceAmountDecimal = Convert.ToDecimal(prvm.InvoiceAmount);

                var balance = invoiceAmountDecimal - totalPaid;

                payment.AmountPaid = totalPaid;
                payment.BalanceAmount = balance; // if BalanceAmount is string

                if (balance == 0)
                    payment.Status = "Received";
                else if (totalPaid > 0)
                    payment.Status = "Partially Received";
                else
                    payment.Status = "Pending";

                entities.SaveChanges();

                return Json(new { message = "Success" });

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }
        //POST: Get Position for Edit View
        public ActionResult EditPaymentsReceived(int? id)
        {
            try
            {
                PaymentsReceivedModel paymentsreceivedData = prmodel.GetPaymentsReceivedDetailsById(id);

                return Json(new { paymentsreceivedData = paymentsreceivedData, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return View(e.Message);
            }

        }

        public ActionResult AddCheques(int? paymentId, int? chequeId = null)
        {
            if (!paymentId.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "paymentId is required.");
            }

            var cheque = chequeId.HasValue
                ? entities.ChequeDetails.FirstOrDefault(c => c.ChequeId == chequeId.Value)
                : null;

            var payment = entities.PaymentsReceiveds.FirstOrDefault(p => p.PaymentsReceivedId == paymentId.Value);

            if (payment == null)
                return HttpNotFound();

            if (payment.BalanceAmount == 0)
            {
                TempData["Message"] = "Payment is already completed. No more cheques can be added.";
                return RedirectToAction("ManageCheques", new { paymentId });
            }

            var model = new ChequeDetailViewModel
            {
                ChequeId = cheque?.ChequeId ?? 0,
                PaymentId = paymentId.Value,
                ChequeAmount = cheque?.ChequeAmount ?? 0,
                ChequeNumber = cheque?.ChequeNumber,
                DateReceived = cheque?.DateReceived?.ToString("yyyy-MM-dd")
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult AddOrUpdateCheque(ManageChequesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ManageCheques", new { paymentId = model.NewCheque.PaymentId });
            }

            var cheque = entities.ChequeDetails.FirstOrDefault(c => c.ChequeId == model.NewCheque.ChequeId);
            if (cheque != null)
            {
                cheque.ChequeNumber = model.NewCheque.ChequeNumber;
                cheque.ChequeAmount = model.NewCheque.ChequeAmount;
                cheque.DateReceived = Convert.ToDateTime(model.NewCheque.DateReceived);
            }
            else
            {
                // Check if total paid will exceed invoice amount
                var payment = entities.PaymentsReceiveds.FirstOrDefault(p => p.PaymentsReceivedId == model.NewCheque.PaymentId);
                if (payment == null) return HttpNotFound();

                var totalPaidSoFar = entities.ChequeDetails
                    .Where(c => c.PaymentsReceivedId == model.NewCheque.PaymentId)
                    .Sum(c => c.ChequeAmount ?? 0);

                if ((totalPaidSoFar + model.NewCheque.ChequeAmount) > Convert.ToDecimal(payment.InvoiceAmount))
                {
                    TempData["Error"] = "Cheque amount exceeds balance.";
                    return RedirectToAction("ManageCheques", new { paymentId = model.NewCheque.PaymentId });
                }

                var newCheque = new ChequeDetail
                {
                    PaymentsReceivedId = model.NewCheque.PaymentId,
                    ChequeNumber = model.NewCheque.ChequeNumber,
                    ChequeAmount = model.NewCheque.ChequeAmount,
                    DateReceived = Convert.ToDateTime(model.NewCheque.DateReceived),
                };

                // Set proper CNumber
                newCheque.CNumber = entities.ChequeDetails
                    .Count(c => c.PaymentsReceivedId == model.NewCheque.PaymentId) + 1;

                entities.ChequeDetails.AddObject(newCheque);
            }

            entities.SaveChanges();

            // Recalculate payment status
            var updatedPayment = entities.PaymentsReceiveds.FirstOrDefault(p => p.PaymentsReceivedId == model.NewCheque.PaymentId);
            if (updatedPayment != null)
            {
                var totalPaid = entities.ChequeDetails
                    .Where(c => c.PaymentsReceivedId == model.NewCheque.PaymentId)
                    .Sum(c => c.ChequeAmount ?? 0);

                updatedPayment.AmountPaid = totalPaid;
                updatedPayment.BalanceAmount = Convert.ToDecimal(updatedPayment.InvoiceAmount) - totalPaid;

                if (updatedPayment.BalanceAmount == 0)
                    updatedPayment.Status = "Received";
                else if (totalPaid > 0)
                    updatedPayment.Status = "Partially Received";
                else
                    updatedPayment.Status = "Pending";

                entities.SaveChanges();
            }
            TempData["Success"] = "Cheque saved successfully!";
            TempData["SweetAlertMessage"] = "Cheque saved successfully!";

            return RedirectToAction("ManageCheques", new { paymentId = model.NewCheque.PaymentId });
        }

        [HttpGet]
        public ActionResult ManageCheques(int paymentId)
        {
            var payment = entities.PaymentsReceiveds.FirstOrDefault(p => p.PaymentsReceivedId == paymentId);
            if (payment == null) return HttpNotFound();

            var cheques = entities.ChequeDetails
                .Where(c => c.PaymentsReceivedId == paymentId)
                .OrderBy(c => c.CNumber)
                .ToList();

            var viewModel = new ManageChequesViewModel
            {
                Payment = payment,
                Cheques = cheques,
                NewCheque = new ChequeDetailViewModel { PaymentId = paymentId }
            };

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult AddOrUpdateCheques(ChequeDetailViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = string.Join(", ", errors) });
                }

                var payment = entities.PaymentsReceiveds.FirstOrDefault(p => p.PaymentsReceivedId == model.PaymentId);
                if (payment == null)
                    return Json(new { success = false, message = "Payment not found" });

                ChequeDetail cheque;
                if (model.ChequeId > 0)
                {
                    cheque = entities.ChequeDetails.FirstOrDefault(c => c.ChequeId == model.ChequeId);
                    if (cheque == null)
                        return Json(new { success = false, message = "Cheque not found" });
                }
                else
                {
                    var totalPaid = entities.ChequeDetails
                        .Where(c => c.PaymentsReceivedId == model.PaymentId)
                        .Sum(c => c.ChequeAmount ?? 0);



                    cheque = new ChequeDetail
                    {
                        PaymentsReceivedId = model.PaymentId,
                        CNumber = entities.ChequeDetails.Count(c => c.PaymentsReceivedId == model.PaymentId) + 1
                    };
                    entities.ChequeDetails.AddObject(cheque);
                }

                cheque.ChequeNumber = model.ChequeNumber;
                cheque.ChequeAmount = model.ChequeAmount;
                cheque.DateReceived = Convert.ToDateTime(model.DateReceived);
                entities.SaveChanges();

                UpdatePaymentStatus(payment);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void UpdatePaymentStatus(PaymentsReceived payment)
        {
            var totalPaid = entities.ChequeDetails
                .Where(c => c.PaymentsReceivedId == payment.PaymentsReceivedId)
                .Sum(c => c.ChequeAmount ?? 0);

            payment.AmountPaid = totalPaid;
            payment.BalanceAmount = Convert.ToDecimal(payment.InvoiceAmount) - totalPaid;

            payment.Status = payment.BalanceAmount == 0 ? "Received" :
                            totalPaid > 0 ? "Partially Received" : "Pending";

            entities.SaveChanges();
        }

        public ActionResult ChequeReport(int paymentId)
        {
            var payment = entities.PaymentsReceiveds.FirstOrDefault(p => p.PaymentsReceivedId == paymentId);
            if (payment == null)
                return HttpNotFound();

            var cheques = entities.ChequeDetails
                .Where(c => c.PaymentsReceivedId == paymentId)
                .OrderBy(c => c.CNumber) // Assuming chronological order
                .ToList();

            decimal totalPaidSoFar = 0;
            decimal invoiceAmount = Convert.ToDecimal(payment.InvoiceAmount);

            var report = new List<ChequeReportViewModel>();

            foreach (var cheque in cheques)
            {
                totalPaidSoFar += cheque.ChequeAmount ?? 0;
                decimal balance = invoiceAmount - totalPaidSoFar;

                report.Add(new ChequeReportViewModel
                {
                    CNumber = cheque.CNumber ?? 0,
                    ChequeNumber = cheque.ChequeNumber,
                    ChequeAmount = cheque.ChequeAmount,
                    DateReceived = cheque.DateReceived,
                    AmountPaidSoFar = totalPaidSoFar,
                    BalanceRemaining = balance
                });
            }

            ViewBag.InvoiceAmount = invoiceAmount;
            ViewBag.PaymentId = paymentId;

            // IMPORTANT: Passing report (a List), not ManageChequesViewModel
            return View(report);
        }


    }
}
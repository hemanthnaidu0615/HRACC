using HRACCPortal.Edmx;
using HRACCPortal.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Models
{
    public class PaymentsReceivedModel : PaymentsReceivedObjectModel
    {
        private HRACCDBEntities hRACCDBEntities;

        public PaymentsReceivedModel()
        {
            hRACCDBEntities = new HRACCDBEntities();
        }

        public IEnumerable<SelectListItem> ddlCustomerList
        {
            get
            {
                var customer = hRACCDBEntities.Customers.AsEnumerable().ToList();
                IEnumerable<SelectListItem> items = from value in customer
                                                    select new SelectListItem
                                                    {
                                                        Text = value.CustomerName,
                                                        Value = value.CustomerIdPK.ToString(),
                                                    };
                return items;
            }
        }
        public string AddEditPaymentsReceived(PaymentsReceivedModel model)
        {
            string statusDetail = string.Empty;
            if (model.PaymentsReceivedId == 0)
            {
                PaymentsReceived pr = new PaymentsReceived()
                {
                    CustomerIdFK = model.CustomerIdFK,
                    InvoiceNumber = model.InvoiceNumber,
                    InvoiceAmount = model.InvoiceAmount,
                    InvoiceDueDate = model.InvoiceDueDate,
                    DateAdded = IndianTimeNow,
                    DateUpdated = IndianTimeNow,
                    AddedBy = "ADMIN",
                    UpdatedBy = "ADMIN"
                };

                hRACCDBEntities.PaymentsReceiveds.AddObject(pr);
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            else
            {
                var updatePaymentsReceived = hRACCDBEntities.PaymentsReceiveds.Where(x => x.PaymentsReceivedId == model.PaymentsReceivedId).FirstOrDefault();
                updatePaymentsReceived.CustomerIdFK = model.CustomerIdFK;
                updatePaymentsReceived.InvoiceNumber = model.InvoiceNumber;
                updatePaymentsReceived.InvoiceAmount = model.InvoiceAmount;
                updatePaymentsReceived.InvoiceDueDate = model.InvoiceDueDate;
             //   updatePaymentsReceived.Balance = model.Balance;
                updatePaymentsReceived.DateUpdated = IndianTimeNow;
                updatePaymentsReceived.UpdatedBy = "ADMIN";
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            return statusDetail;
        }
        //Edit Position
        public PaymentsReceivedModel GetPaymentsReceivedDetailsById(int? paymentsreceivedId)
        {
            var paymentsreceivedData = hRACCDBEntities.PaymentsReceiveds.Single(x => x.PaymentsReceivedId == PaymentsReceivedId);
            var customerData = hRACCDBEntities.Customers.Where(x => x.CustomerIdPK == paymentsreceivedData.CustomerIdFK).FirstOrDefault();
            PaymentsReceivedModel paymentsreceivedModel = new PaymentsReceivedModel()
            {
                CustomerIdFK = paymentsreceivedData.CustomerIdFK,
                CustomerName = customerData.CustomerName,
                PaymentsReceivedId = paymentsreceivedData.PaymentsReceivedId,
               
                InvoiceNumber = paymentsreceivedData.InvoiceNumber,
                InvoiceAmount = paymentsreceivedData.InvoiceAmount,
                InvoiceDueDate = paymentsreceivedData.InvoiceDueDate,
                
                DateUpdated = paymentsreceivedData.DateUpdated,
                UpdatedBy = "ADMIN"
            };
            return paymentsreceivedModel;
        }
        //view position
        public void GetPaymentsReceivedList()
        {
            PaymentsReceivedList = (from prLts in hRACCDBEntities.PaymentsReceiveds
                                join custLts in hRACCDBEntities.Customers
                                on prLts.CustomerIdFK equals custLts.CustomerIdPK
                                select new
                                {
                                    prLts.PaymentsReceivedId,
                                    prLts.CustomerIdFK,
                                   
                                    prLts.InvoiceNumber,
                                    prLts.InvoiceAmount,
                                    prLts.InvoiceDueDate,
                                   
                                    prLts.DateAdded,
                                    prLts.DateUpdated,
                                    prLts.AddedBy,
                                    prLts.UpdatedBy,
                                    prLts.Status,
                                    custLts.CustomerName

                                }).AsEnumerable().Select(i => new PaymentsReceivedObjectModel
                                {
                                    PaymentsReceivedId = i.PaymentsReceivedId,
                                    CustomerName = i.CustomerName,
                                    
                                    InvoiceNumber = i.InvoiceNumber,
                                    InvoiceAmount = i.InvoiceAmount,
                                    InvoiceDueDate = i.InvoiceDueDate,
                                    
                                    DateAdded = i.DateAdded,
                                    //DateUpdated = Convert.ToDateTime(i.DateUpdated).ToString("MMM,dd, yyyy"),
                                    DateUpdated = !string.IsNullOrWhiteSpace(i.DateUpdated) && DateTime.TryParse(i.DateUpdated, out var dateUpdated)
                                    ? dateUpdated.ToString("MMM, dd, yyyy")
                                    : "N/A",
                                    AddedBy = i.AddedBy,
                                    UpdatedBy = i.UpdatedBy,
                                    Status = i.Status

                                }).ToList();
        }

    }
}
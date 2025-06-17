using HRACCPortal.Edmx;
using HRACCPortal.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Models
{
    public class InvoiceSubmissionModel : InvoiceSubmissionObjectModel
    {
        private HRACCDBEntities hRACCDBEntities;

        public InvoiceSubmissionModel()
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
        public string AddEditInvoiceSubmission(InvoiceSubmissionModel model)
        {
            string statusDetail = string.Empty;
            if (model.InvoiceSubmissionId == 0)
            {
                InvoiceSubmission io = new InvoiceSubmission()
                {
                    CustomerIdFK = model.CustomerIdFK,
                    Year = model.Year,
                    Month = model.Month,
                    DateAdded = IndianTimeNow,
                    DateUpdated = IndianTimeNow,
                    AddedBy = "ADMIN",
                    UpdatedBy = "ADMIN"
                };

                hRACCDBEntities.InvoiceSubmissions.AddObject(io);
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            else
            {
                var updateInvoiceSubmission = hRACCDBEntities.InvoiceSubmissions.Where(x => x.InvoiceSubmissionId == model.InvoiceSubmissionId).FirstOrDefault();
                updateInvoiceSubmission.CustomerIdFK = model.CustomerIdFK;
                updateInvoiceSubmission.Year = model.Year;
                updateInvoiceSubmission.Month = model.Month;
                updateInvoiceSubmission.DateUpdated = IndianTimeNow;
                updateInvoiceSubmission.UpdatedBy = "ADMIN";
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            return statusDetail;
        }
        //Edit Position
        public InvoiceSubmissionModel GetInvoiceSubmissionDetailsById(int? invoicesubmissionId)
        {
            var invoicesubmissionData = hRACCDBEntities.InvoiceSubmissions.Single(x => x.InvoiceSubmissionId == invoicesubmissionId);
            var customerData = hRACCDBEntities.Customers.Where(x => x.CustomerIdPK == invoicesubmissionData.CustomerIdFK).FirstOrDefault();
            InvoiceSubmissionModel invoicesubmissionModel = new InvoiceSubmissionModel()
            {
                CustomerIdFK = invoicesubmissionData.CustomerIdFK,
                CustomerName = customerData.CustomerName,
                InvoiceSubmissionId = invoicesubmissionData.InvoiceSubmissionId,
                Year = invoicesubmissionData.Year,
                Month = invoicesubmissionData.Month,
                DateUpdated = invoicesubmissionData.DateUpdated,
                UpdatedBy = "ADMIN"
            };
            return invoicesubmissionModel;
        }
        //view position
        public void GetInvoiceSubmissionList()
        {
            InvoiceSubmissionList = (from ioLts in hRACCDBEntities.InvoiceSubmissions
                            join custLts in hRACCDBEntities.Customers
                            on ioLts.CustomerIdFK equals custLts.CustomerIdPK
                            select new
                            {
                                ioLts.InvoiceSubmissionId,
                                ioLts.CustomerIdFK,
                                ioLts.Year,
                                ioLts.Month,
                                ioLts.DateAdded,
                                ioLts.DateUpdated,
                                ioLts.AddedBy,
                                ioLts.UpdatedBy,
                                custLts.CustomerName

                            }).AsEnumerable().Select(i => new InvoiceSubmissionObjectModel
                            {
                                InvoiceSubmissionId = i.InvoiceSubmissionId,
                              CustomerName = i.CustomerName,
                                Year = i.Year,
                                Month = i.Month,
                                DateAdded = i.DateAdded,
                                DateUpdated = Convert.ToDateTime(i.DateUpdated).ToString("MMM,dd, yyyy"),
                                AddedBy = i.AddedBy,
                                UpdatedBy = i.UpdatedBy

                            }).ToList();
        }

        
    }
}
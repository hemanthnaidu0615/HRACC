using HRACCPortal.Edmx;
using HRACCPortal.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Models
{
    public class QuaterlySalesReportModel : QuaterlySalesReportObjectModel
    {
        private HRACCDBEntities hRACCDBEntities;

        public QuaterlySalesReportModel()
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
        public string AddEditQuaterlySalesReport(QuaterlySalesReportModel model)
        {
            string statusDetail = string.Empty;
            if (model.QuaterlySalesReportIdPK == 0)
            {
                QuaterlySalesReport qsr = new QuaterlySalesReport()
                {
                    CustomerIdFK = model.CustomerIdFK,
                    Quarter = model.Quarter,
                    Month = model.Month,
                    ConsultantName = model.ConsultantName,
                    InvoiceAmount = model.InvoiceAmount,
                    DateAdded = IndianTimeNow,
                    DateUpdated = IndianTimeNow,
                    AddedBy = "ADMIN",
                    UpdatedBy = "ADMIN"
                };

                hRACCDBEntities.QuaterlySalesReports.AddObject(qsr);
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            else
            {
                var updateQuaterlySalesReport = hRACCDBEntities.QuaterlySalesReports.Where(x => x.QuaterlySalesReportIdPK == model.QuaterlySalesReportIdPK).FirstOrDefault();
                updateQuaterlySalesReport.CustomerIdFK = model.CustomerIdFK;
                updateQuaterlySalesReport.Quarter = model.Quarter;
                updateQuaterlySalesReport.Month = model.Month;
                updateQuaterlySalesReport.ConsultantName = model.ConsultantName;
                updateQuaterlySalesReport.InvoiceAmount = model.InvoiceAmount;
                updateQuaterlySalesReport.DateUpdated = IndianTimeNow;
                updateQuaterlySalesReport.UpdatedBy = "ADMIN";
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            return statusDetail;
        }
        //Edit Position
        public QuaterlySalesReportModel GetQuaterlySalesReportDetailsById(int? quaterlysalesreportPK)
        {
            var quaterlysalesreportData = hRACCDBEntities.QuaterlySalesReports.Single(x => x.QuaterlySalesReportIdPK == quaterlysalesreportPK);
            var customerData = hRACCDBEntities.Customers.Where(x => x.CustomerIdPK == quaterlysalesreportData.CustomerIdFK).FirstOrDefault();
            QuaterlySalesReportModel quaterlysalesreportModel = new QuaterlySalesReportModel()
            {
                CustomerIdFK = quaterlysalesreportData.CustomerIdFK,
                CustomerName = customerData.CustomerName,
                QuaterlySalesReportIdPK = quaterlysalesreportData.QuaterlySalesReportIdPK,
                Quarter = quaterlysalesreportData.Quarter,
                Month = quaterlysalesreportData.Month,
                ConsultantName = quaterlysalesreportData.ConsultantName,
                InvoiceAmount = quaterlysalesreportData.InvoiceAmount,
                DateUpdated = quaterlysalesreportData.DateUpdated,
                UpdatedBy = "ADMIN"
            };
            return quaterlysalesreportModel;
        }
        //view position
        public void GetQuaterlySalesReportList()
        {
            QuaterlySalesReportList = (from qsrLts in hRACCDBEntities.QuaterlySalesReports
                            join custLts in hRACCDBEntities.Customers
                            on qsrLts.CustomerIdFK equals custLts.CustomerIdPK
                            select new
                            {
                                qsrLts.QuaterlySalesReportIdPK,
                                qsrLts.CustomerIdFK,
                                qsrLts.Quarter,
                                qsrLts.Month,
                                qsrLts.ConsultantName,
                                qsrLts.InvoiceAmount,
                                qsrLts.DateAdded,
                                qsrLts.DateUpdated,
                                qsrLts.AddedBy,
                                qsrLts.UpdatedBy,
                                custLts.CustomerName

                            }).AsEnumerable().Select(i => new QuaterlySalesReportObjectModel
                            {
                                QuaterlySalesReportIdPK = i.QuaterlySalesReportIdPK,
                                CustomerName = i.CustomerName,
                                Quarter = i.Quarter,
                                Month = i.Month,
                                ConsultantName = i.ConsultantName,
                                InvoiceAmount = i.InvoiceAmount,
                                DateAdded = i.DateAdded,
                                //DateUpdated = Convert.ToDateTime(i.DateUpdated).ToString("MMM,dd, yyyy"),
                                //UZAIR
                                DateUpdated = !string.IsNullOrWhiteSpace(i.DateUpdated) && DateTime.TryParse(i.DateUpdated, out var dateUpdated)
                                ? dateUpdated.ToString("MMM, dd, yyyy")
                                : "N/A",
                                AddedBy = i.AddedBy,
                                UpdatedBy = i.UpdatedBy

                            }).ToList();
        }
    }
}
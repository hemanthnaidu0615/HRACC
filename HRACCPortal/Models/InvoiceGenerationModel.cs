using HRACCPortal.Edmx;
using HRACCPortal.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace HRACCPortal.Models
{
    public class InvoiceGenerationModel : InvoiceGenerationObjectModel
    {
        private readonly HRACCDBEntities hRACCDBEntities;

        public InvoiceGenerationModel()
        {
            hRACCDBEntities = new HRACCDBEntities();
            //  cls = new clsCrud();
        }

        public IEnumerable<SelectListItem> ddlConsultantList
        {
            get
            {
                var consultantList = hRACCDBEntities.Consultants.AsEnumerable().ToList();
                IEnumerable<SelectListItem> items = from value in consultantList
                                                    select new SelectListItem
                                                    {
                                                        Text = value.ConsultantNameAbbrv,
                                                        Value = value.ConsultantIdPK.ToString(),
                                                    };
                return items;
            }
        }

        public IEnumerable<SelectListItem> ddlConsultantPositionDetailsList
        {
            get
            {
                var consultantPositionDetailsList = hRACCDBEntities.ConsultantPositionDetails.AsEnumerable().ToList();
                IEnumerable<SelectListItem> items = from value in consultantPositionDetailsList
                                                    select new SelectListItem
                                                    {
                                                        Text = value.ConsultantPositionIdPK.ToString(),
                                                        Value = value.ConsultantPositionIdPK.ToString(),
                                                    };
                return items;
            }
        }

        // start invoice
        //view invoice
        public void GetInvoiceGenerationList()
        {
            InvoiceGenerationList = (from invoicegeneration in hRACCDBEntities.InvoiceGenerations
                           select new
                           {
                               invoicegeneration.InvoiceGenerationId,
                               invoicegeneration.InvoiceDate,
                               invoicegeneration.InvoiceMonth,
                               invoicegeneration.InvoiceDueDate,
                               invoicegeneration.DateAdded,
                               invoicegeneration.DateUpdated,
                               invoicegeneration.AddedBy,
                               invoicegeneration.UpdatedBy

                           }).AsEnumerable().Select(i => new InvoiceGenerationObjectModel
                           {
                               InvoiceGenerationId = i.InvoiceGenerationId,
                               InvoiceDate = i.InvoiceDate,
                               InvoiceMonth = i.InvoiceMonth,
                               InvoiceDueDate = i.InvoiceDueDate,
                               DateAdded = i.DateAdded,
                               // DateUpdated = i.DateUpdated,
                               DateUpdated = Convert.ToDateTime(i.DateUpdated).ToString("MMM,dd, yyyy"),
                               AddedBy = i.AddedBy,
                               UpdatedBy = i.UpdatedBy

                           }).ToList();
        }

        //add edit invoice details
        public string AddEditInvoiceGeneration(InvoiceGenerationModel model)
        {
            string statusDetail = string.Empty;
            if (model.InvoiceGenerationId == 0)
            {
                InvoiceGeneration invgdata = new InvoiceGeneration()
                {
                    InvoiceDate = model.InvoiceDate,
                    InvoiceMonth = model.InvoiceMonth,
                    InvoiceDueDate = model.InvoiceDueDate,
                    DateAdded = IndianTimeNow,
                    DateUpdated = IndianTimeNow,
                    AddedBy = "ADMIN",
                    UpdatedBy = "ADMIN"
                };

                hRACCDBEntities.InvoiceGenerations.AddObject(invgdata);
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
                if (statusDetail == "Success")
                {

                    InvoicePdfModel Model = new InvoicePdfModel();
                    clsCrud cls1 = new clsCrud();
                    Model = cls1.GenratePdf(invgdata.InvoiceGenerationId);

                    var sb = new System.Text.StringBuilder();

                    sb.AppendLine(@"<div id=""kt_app_content_container"" class=""app-container container-xxl"">");
                    sb.AppendLine(@"    <!--begin::Card-->");
                    sb.AppendLine(@"");
                    sb.AppendLine(@"    <div class=""card mb-5 mb-xl-10"">");
                    sb.AppendLine(@"        <!--begin::Card header-->");
                    sb.AppendLine(@"        <!--<div class=""card-header border-0 cursor-pointer"" role=""button"" data-bs-toggle=""collapse"" data-bs-target=""#kt_account_profile_details"" aria-expanded=""true"" aria-controls=""kt_account_profile_details"">-->");
                    sb.AppendLine(@"        <!--begin::Card title-->");
                    sb.AppendLine(@"        <!--<div class=""card-title m-0"">");
                    sb.AppendLine(@"            <h3 class=""fw-bold m-0"">Customer Details</h3>");
                    sb.AppendLine(@"        </div>-->");
                    sb.AppendLine(@"        <!--end::Card title-->");
                    sb.AppendLine(@"        <!--</div>-->");
                    sb.AppendLine(@"        <!--begin::Card header-->");
                    sb.AppendLine(@"        <!--begin::Content-->");
                    sb.AppendLine(@"        <div id=""kt_account_settings_profile_details"" class=""collapse show"">");
                    sb.AppendLine(@"            <div class=""p-15"">");
                    sb.AppendLine(@"                <table class=""table"" width=""100%"" style=""padding-left:10px; border:1px dashed #fff !important;"">");
                    sb.AppendLine(@"                    <tr>");
                    sb.AppendLine(@"                        <td>");
                    sb.AppendLine(@"                            <img src=""http://h3usystems.com/Content/Images/HRACC_Logo.png"" />");
                    sb.AppendLine(@"                        </td>");
                    sb.AppendLine(@"                        <td></td>");
                    sb.AppendLine(@"                        <td><h1 style=""text-align: center;margin-top: 50px;font-size: 50px;"">Invoice </h1></td>");
                    sb.AppendLine(@"                    </tr>");
                    sb.AppendLine(@"                    <tr>");
                    sb.AppendLine(@"                        <td>");
                    sb.AppendLine(@"                            <h2>" + Model.CompanyName + " </h2>");
                    sb.AppendLine(@"                            " + Model.CompanyAddress1 + " " + Model.CompanyAddress2 + " < br />");
                    sb.AppendLine(@"                            " + Model.CompanyCity + ", " + Model.CompanyState + ", " + Model.CompanyZip + " ");
                    sb.AppendLine(@"                            " + Model.CompanyEmail + " <br />");
                    sb.AppendLine(@"                            " + Model.CompanyPhone + " <br />");
                    sb.AppendLine(@"                            " + Model.CompanyFax + " ");
                    sb.AppendLine(@"                        </td>");
                    sb.AppendLine(@"                        <td></td>");
                    sb.AppendLine(@"                        <td style=""text-align:right;"">");
                    sb.AppendLine(@"                            <br />");
                    sb.AppendLine(@"                            " + Model.invDetails[0].PurchaseOrderNo + " <br />");
                    sb.AppendLine(@"                            Invoice: <b> #" + Model.invDetails[0].InvoiceNo + " </b> <br />");
                    sb.AppendLine(@"                            Terms: <b> Net " + Model.invDetails[0].Term + " </b> <br />");
                    sb.AppendLine(@"                            Invoice Date:  <b> " + Model.invDetails[0].InvoiceDate + " </b> <br />");
                    sb.AppendLine(@"                            Payment Due:   <b> " + Model.invDetails[0].DueDate + " </b> <br />");
                    sb.AppendLine(@"                            Amount Due (USD) <br />");
                    sb.AppendLine(@"                            <b> $" + Model.invDetails[0].TotalAmount + " </b>");
                    sb.AppendLine(@"                        </td>");
                    sb.AppendLine(@"                    </tr>");
                    sb.AppendLine(@"                    <tr><td colspan=""3""><br /></td></tr>");
                    sb.AppendLine(@"                    <tr>");
                    sb.AppendLine(@"                        <td>");
                    sb.AppendLine(@"                            To: <br />");
                    sb.AppendLine(@"                            " + Model.invDetails[0].CustomerName + "  <br />");
                    sb.AppendLine(@"                            " + Model.invDetails[0].CustomerContactAddress1 + " " + Model.invDetails[0].CustomerContactAddress2 + " <br />");
                    sb.AppendLine(@"                            " + Model.invDetails[0].CustomerContactCity + ",  " + Model.invDetails[0].CustomerContactState + ", " + Model.invDetails[0].CustomerContactZip + " <br />");
                    sb.AppendLine(@"                            " + Model.invDetails[0].CustomerContactEmail + " <br />");
                    sb.AppendLine(@"                            " + Model.invDetails[0].CustomerContactPhone + " <br />");
                    sb.AppendLine(@"");
                    sb.AppendLine(@"                        </td>");
                    sb.AppendLine(@"                        <td colspan=""2""></td>");
                    sb.AppendLine(@"                    </tr>");
                    sb.AppendLine(@"                    <tr>");
                    sb.AppendLine(@"                        <td colspan=""3"">");
                    sb.AppendLine(@"                            <table class=""table"" style=""width: 100% !important;border:1px solid #eee; padding:5px;"" id=""subtable"">");
                    sb.AppendLine(@"                                <tr style=""border:1px solid #eee; padding:5px;"">");
                    sb.AppendLine(@"                                    <th style=""border:1px solid #eee; padding:5px;"">DESCRIPTION</th>");
                    sb.AppendLine(@"                                    <th style=""border:1px solid #eee; padding:5px;"">HOURS</th>");
                    sb.AppendLine(@"                                    <th style=""border:1px solid #eee; padding:5px;"">RATE</th>");
                    sb.AppendLine(@"                                    <th style=""border:1px solid #eee; padding:5px; text-align:right;"">AMOUNT</th>");
                    sb.AppendLine(@"                                </tr>");
                    sb.AppendLine(@"                                <tr style=""border:1px solid #eee; padding:5px;"">");
                    sb.AppendLine(@"                                    <td style=""border:1px solid #eee; padding:5px;"">");
                    sb.AppendLine(@"                                        " + Model.invDetails[0].FirstName + "  " + Model.invDetails[0].MiddleName + " " + Model.invDetails[0].LastName + " <br />");
                    sb.AppendLine(@"                                        Title: " + Model.invDetails[0].PositionTitle + " <br />");
                    sb.AppendLine(@"                                        P.O#: " + Model.invDetails[0].PurchaseOrderNo + " <br />");
                    sb.AppendLine(@"                                        Job No: " + Model.invDetails[0].PositionNumber + " <br />");
                    sb.AppendLine(@"                                        Job Family: " + Model.invDetails[0].PositionFamily + " <br />");
                    sb.AppendLine(@"                                        Scope Variant: " + Model.invDetails[0].PositionScopeVariant + "  <br />");
                    sb.AppendLine(@"                                        Service Date: " + Model.invDetails[0].MonthStartDate + " - " + Model.invDetails[0].MonthEndDate + " ");
                    sb.AppendLine(@"");
                    sb.AppendLine(@"                                    </td>");
                    sb.AppendLine(@"                                    <td style=""border:1px solid #eee; padding:5px;"">" + Model.invDetails[0].RegularHours + " </td>");
                    sb.AppendLine(@"                                    <td style=""border:1px solid #eee; padding:5px;"">$" + Model.invDetails[0].Rate + " </td>");
                    sb.AppendLine(@"                                    <td style=""border:1px solid #eee; padding:5px;text-align:right;"">$" + Model.invDetails[0].TotalAmount + " </td>");
                    sb.AppendLine(@"                                </tr>");
                    sb.AppendLine(@"                                <tr>");
                    sb.AppendLine(@"");
                    sb.AppendLine(@"                                    <td colspan=""3"" style=""border:1px solid #eee; padding:5px;text-align:right;"">Total &nbsp; </td>");
                    sb.AppendLine(@"                                    <td style=""border:1px solid #eee; padding:5px;text-align:right;"">$" + Model.invDetails[0].TotalAmount + " </td>");
                    sb.AppendLine(@"                                </tr>");
                    sb.AppendLine(@"                            </table>");
                    sb.AppendLine(@"                        </td>");
                    sb.AppendLine(@"                    </tr>");
                    sb.AppendLine(@"                    <tr><td colspan=""3""><br /></td></tr>");
                    sb.AppendLine(@"                    <tr>");
                    sb.AppendLine(@"                        <td colspan=""3"" style=""text-align:left"">Make all checks payable to " + Model.CompanyName + " </td>");
                    sb.AppendLine(@"                    </tr>");
                    sb.AppendLine(@"                    <tr><td colspan=""3""><br /></td></tr>");
                    sb.AppendLine(@"                    <tr>");
                    sb.AppendLine(@"                        <td colspan=""3"" style=""text-align:center"">Thank you for your business!</td>");
                    sb.AppendLine(@"                    </tr>");
                    sb.AppendLine(@"                </table>");
                    sb.AppendLine(@"            </div>");
                    sb.AppendLine(@"        </div>");
                    sb.AppendLine(@"    </div>");
                    sb.AppendLine(@"</div>");


                    String fromEmail = "info@h3usystems.com";
                    // String toEmail = "tauseefnabiji@gmail.com";
                    String toEmail = "tauseefnabiji@gmail.com";
                    //String bccEmail = WebConfigurationManager.AppSettings["toBcc"].ToString();
                    // String bccEmail1 = WebConfigurationManager.AppSettings["toBcc1"].ToString();
                    String toCC = WebConfigurationManager.AppSettings["toCC"].ToString();
                    MailMessage message = new MailMessage(new MailAddress(fromEmail, "H3USYSTEMS").ToString(), toEmail, "Invoice", sb.ToString())
                    {
                        IsBodyHtml = true
                    };
                    message.To.Add(Model.invDetails[0].CustomerContactEmail);
                    if (!string.IsNullOrEmpty(toCC))
                    {
                        message.CC.Add(toCC);
                    }
                    //message.CC.Add(toCC);
                    //message.Bcc.Add(bccEmail);
                    //message.Bcc.Add(bccEmail1);
                    ////  message.Bcc.Add("amirza@webtractions.com");

                    SmtpClient smtp = new SmtpClient();


                    smtp.Send(message);
                }
            }
            else
            {
                var updateInvoiceGeneration = hRACCDBEntities.InvoiceGenerations.Where(x => x.InvoiceGenerationId == model.InvoiceGenerationId).FirstOrDefault();
                updateInvoiceGeneration.InvoiceDate = model.InvoiceDate;
                updateInvoiceGeneration.InvoiceMonth = model.InvoiceMonth;
                updateInvoiceGeneration.InvoiceDueDate = model.InvoiceDueDate;
                updateInvoiceGeneration.DateUpdated = IndianTimeNow;
                updateInvoiceGeneration.UpdatedBy = "ADMIN";
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            return statusDetail;
        }

        //Edit Position
        public InvoiceGenerationModel GetInvoiceGenerationDetailsById(int? invoicegenerationId)
        {
            var invoicegenerationData = hRACCDBEntities.InvoiceGenerations.Single(x => x.InvoiceGenerationId == invoicegenerationId);
            InvoiceGenerationModel invoicegenerationModel = new InvoiceGenerationModel()
            {
                InvoiceGenerationId = invoicegenerationData.InvoiceGenerationId,
                InvoiceDate = invoicegenerationData.InvoiceDate,
                InvoiceMonth = invoicegenerationData.InvoiceMonth,
                InvoiceDueDate = invoicegenerationData.InvoiceDueDate,
                DateAdded = invoicegenerationData.DateAdded,
                DateUpdated = invoicegenerationData.DateUpdated,
                AddedBy = invoicegenerationData.AddedBy,
                UpdatedBy = invoicegenerationData.UpdatedBy
            };
            return invoicegenerationModel;
        }
    }
}
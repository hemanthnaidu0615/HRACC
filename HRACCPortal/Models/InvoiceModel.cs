using HRACCPortal.Edmx;
using HRACCPortal.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace HRACCPortal.Models
{
    public class InvoiceModel : InvoiceObjectModel
    {
        private readonly HRACCDBEntities hRACCDBEntities;
        
        public InvoiceModel()
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
        public void GetInvoiceList()
        {
            InvoiceList = (from invoice in hRACCDBEntities.Invoices
                            select new
                            {
                                invoice.InvoiceIdPK,
                                invoice.InvoiceNumber,
                                invoice.Year,
                                invoice.Month,
                                invoice.DueDate,
                                invoice.InvoiceDate,
                                invoice.ConsultantIdFK,
                                invoice.ConsultantPositionIdFK,
                                invoice.RegularHours,
                                invoice.OvertimeHours,
                                invoice.InvoiceAmount,
                                invoice.DateAdded,
                                invoice.DateUpdated,
                                invoice.AddedBy,
                                invoice.UpdatedBy

                            }).AsEnumerable().Select(i => new InvoiceObjectModel
                            {
                                InvoiceIdPK = i.InvoiceIdPK,
                                InvoiceNumber = i.InvoiceNumber,
                                Year = i.Year,
                                Month = i.Month,
                                InvoiceDate=i.InvoiceDate,
                                DueDate = i.DueDate,
                                ConsultantIdFK=i.ConsultantIdFK,
                                ConsultantPositionIdFK=i.ConsultantPositionIdFK,
                                RegularHours=i.RegularHours,
                                OvertimeHours=i.OvertimeHours,
                                InvoiceAmount = i.InvoiceAmount,
                                DateAdded = i.DateAdded,
                                // DateUpdated = i.DateUpdated,
                                //DateUpdated = Convert.ToDateTime(i.DateUpdated).ToString("MMM,dd, yyyy"),

                                //Modified by Uzair
                                DateUpdated = !string.IsNullOrWhiteSpace(i.DateUpdated) && DateTime.TryParse(i.DateUpdated, out var dateUpdated)
                                ? dateUpdated.ToString("MMM, dd, yyyy")
                                : "N/A",
                                AddedBy = i.AddedBy,
                                UpdatedBy = i.UpdatedBy

                            }).ToList();
        }

        //add edit invoice details
        public string AddEditInvoice(InvoiceModel model)
        {
            string statusDetail = string.Empty;
            if (model.InvoiceIdPK == 0)
            {
                Invoice invdata = new Invoice()
                {
                    InvoiceNumber = model.InvoiceNumber,
                    Year = model.Year,
                    Month = model.Month,
                    InvoiceDate = model.InvoiceDate,
                    DueDate = model.DueDate,
                    ConsultantIdFK = model.ConsultantIdFK,
                    ConsultantPositionIdFK = model.ConsultantPositionIdFK,
                    InvoiceAmount = model.InvoiceAmount,
                    RegularHours = model.RegularHours,
                    OvertimeHours = model.OvertimeHours,
                    DateAdded = IndianTimeNow,
                    DateUpdated = IndianTimeNow,
                    AddedBy = "ADMIN",
                    UpdatedBy = "ADMIN"
                };
                if (model.CustomerId > 0)
                {
                    invdata.CustomerId = model.CustomerId;
                }

                hRACCDBEntities.Invoices.AddObject(invdata);
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
                if (statusDetail == "Success")
                {

                    InvoicePdfModel Model = new InvoicePdfModel();
                    clsCrud cls1 = new clsCrud();
                    Model = cls1.GenratePdf(invdata.InvoiceIdPK);

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


                    // String fromEmail = "info@h3usystems.com";
                    //// String toEmail = "tauseefnabiji@gmail.com";
                    // String toEmail = "tauseefnabiji@gmail.com";
                    // //String bccEmail = WebConfigurationManager.AppSettings["toBcc"].ToString();
                    // // String bccEmail1 = WebConfigurationManager.AppSettings["toBcc1"].ToString();
                    // String toCC = WebConfigurationManager.AppSettings["toCC"].ToString();
                    // MailMessage message = new MailMessage(new MailAddress(fromEmail, "H3USYSTEMS").ToString(), toEmail, "Invoice", sb.ToString())
                    // {
                    //     IsBodyHtml = true
                    // };
                    // message.To.Add(Model.invDetails[0].CustomerContactEmail);
                    // if(!string.IsNullOrEmpty(toCC))
                    // {
                    //     message.CC.Add(toCC);
                    // }
                    // //message.CC.Add(toCC);
                    // //message.Bcc.Add(bccEmail);
                    // //message.Bcc.Add(bccEmail1);
                    // ////  message.Bcc.Add("amirza@webtractions.com");

                    // SmtpClient smtp = new SmtpClient();


                    // smtp.Send(message);

                    String fromEmail = "shaikuzairuzair@gmail.com"; // Use your sender's email address
                    String appPassword = "fuuanbubiwfpfavl"; // Use your Gmail app password
                    //String toEmail = Model.invDetails[0].CustomerContactEmail; // Customer's email address
                    String toEmail ="mohammeduzair0311@gmail.com";
                    // Construct the email body
                    MailMessage message = new MailMessage()
                    {
                        From = new MailAddress(fromEmail, "H3USYSTEMS"),
                        Subject = "Invoice",
                        Body = sb.ToString(),
                        IsBodyHtml = true
                    };

                    // Add recipients
                    message.To.Add(toEmail);

                    // Optional CC
                    String toCC = WebConfigurationManager.AppSettings["toCC"].ToString();
                    if (!string.IsNullOrEmpty(toCC))
                    {
                        message.CC.Add(toCC);
                    }

                    // Configure the SMTP client
                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587, // Port for TLS
                        Credentials = new NetworkCredential(fromEmail, appPassword), // Use app password
                        EnableSsl = true, // Enable SSL/TLS
                    };

                    // Send the email
                    try
                    {
                        smtpClient.Send(message);
                        Console.WriteLine("Invoice email sent successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending invoice email: " + ex.Message);
                    }
                }
            }
            else
            {
                var updateInvoice = hRACCDBEntities.Invoices.Where(x => x.InvoiceIdPK == model.InvoiceIdPK).FirstOrDefault();
                updateInvoice.InvoiceNumber = model.InvoiceNumber;
                updateInvoice.Year = model.Year;
                updateInvoice.Month = model.Month;
                updateInvoice.InvoiceDate = model.InvoiceDate;
                updateInvoice.DueDate = model.DueDate;
                updateInvoice.ConsultantIdFK = model.ConsultantIdFK;
                updateInvoice.ConsultantPositionIdFK = model.ConsultantPositionIdFK;
                updateInvoice.InvoiceAmount = model.InvoiceAmount;
                updateInvoice.RegularHours = model.RegularHours;
                updateInvoice.OvertimeHours = model.OvertimeHours;
                updateInvoice.DateUpdated = IndianTimeNow;
                updateInvoice.UpdatedBy = "ADMIN";
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            return statusDetail;
        }

        //Edit Position
        public InvoiceModel GetInvoiceDetailsById(int? invoiceIdPK)
        {
            var invoiceData = hRACCDBEntities.Invoices.Single(x => x.InvoiceIdPK == invoiceIdPK);
            InvoiceModel invoiceModel = new InvoiceModel()
            {
                InvoiceIdPK=invoiceData.InvoiceIdPK,
                InvoiceNumber = invoiceData.InvoiceNumber,
                Year = invoiceData.Year,
                Month = invoiceData.Month,
                InvoiceDate = invoiceData.InvoiceDate,
                DueDate = invoiceData.DueDate,
                ConsultantIdFK = invoiceData.ConsultantIdFK,
                ConsultantPositionIdFK = invoiceData.ConsultantPositionIdFK,
                InvoiceAmount = invoiceData.InvoiceAmount,
                RegularHours = invoiceData.RegularHours,
                OvertimeHours = invoiceData.OvertimeHours,
                DateAdded = invoiceData.DateAdded,
                DateUpdated = invoiceData.DateUpdated,
                AddedBy = invoiceData.AddedBy,
                UpdatedBy = invoiceData.UpdatedBy
            };
            return invoiceModel;
        }
    }
}
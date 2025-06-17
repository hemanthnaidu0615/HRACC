using HRACCPortal.Edmx;
using HRACCPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

using Timesheet = HRACCPortal.Edmx.Timesheet;
using TimesheetDetail = HRACCPortal.Edmx.TimesheetDetail;
using System.Threading.Tasks;
using Rotativa;

namespace HRACCPortal.Controllers
{
    public class TimesheetController : Controller
    {
        // GET: Timesheet
        public HRACCDBEntities entities;

        public TimesheetController()
        {
            entities = new HRACCDBEntities();
        }


        [HttpGet]
        public ActionResult Index()
        {
            string userEmail = Session["UserEmail"] as string;



            if (Session["UserEmail"] == null || Session["UserRole"] == null)
            {
                return RedirectToAction("Login", "Account");
            }


            // Consultant lookup
            Consultant consultant = null;
            if (Session["UserRole"]?.ToString() == "4") // Consultant
            {
                consultant = entities.Consultants.FirstOrDefault(c => c.Email == userEmail);
                if (consultant == null)
                {
                    return HttpNotFound("Consultant not found");
                }
            }

            // Get timesheets based on user role
            List<Timesheet> timesheets;
            if (Session["UserRole"]?.ToString() == "1") // Admin
            {
                timesheets = entities.Timesheets
                    .Where(t => t.Approval == "pending")
                    .ToList();
            }
            else if (Session["UserRole"]?.ToString() == "4") // Consultant
            {
                timesheets = entities.Timesheets
                    .Where(t => t.ConsultantId == consultant.ConsultantIdPK)
                    .ToList();
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Access denied.");
            }

            // Extract unique customer and employer IDs
            var customerIds = timesheets.Where(t => t.CustomerId.HasValue).Select(t => t.CustomerId.Value).Distinct().ToList();
            var employerIds = timesheets.Where(t => t.EmployerId.HasValue).Select(t => t.EmployerId.Value).Distinct().ToList();

            // Fetch related customer and employer data
            var customers = entities.Customers
                 .Where(c => customerIds.Contains(c.CustomerIdPK))
                 .ToDictionary(c => c.CustomerIdPK, c => c.CustomerName);



            var employers = entities.Employers
                .Where(e => employerIds.Contains(e.EmployerIdPK))
                .ToDictionary(e => e.EmployerIdPK, e => e.EmployerName);

            // Pass for dropdowns if needed
            ViewBag.Customers = customers.Select(c => new { CustomerId = c.Key, Name = c.Value }).ToList();

            ViewBag.Employers = employers.Select(e => new { EmployerId = e.Key, Name = e.Value }).ToList();

            // Map timesheets to view model
            var timesheetList = timesheets.Select(t => new TimesheetViewModel
            {
                TimesheetId = t.TimesheetId,
                ConsultantId = (int)t.ConsultantId,
                Month = t.Month ?? 0,
                Year = t.Year ?? 0,
                TotalHours = (int)(t.TotalHours ?? 0),
                Approval = t.Approval,
                CustomerName = t.CustomerId.HasValue && customers.ContainsKey(t.CustomerId.Value)
                ? customers[t.CustomerId.Value]
                : "N/A",
                EmployerName = t.EmployerId.HasValue && employers.ContainsKey(t.EmployerId.Value)
                ? employers[t.EmployerId.Value]
                : "N/A"
            }).ToList();

            return View(timesheetList);
        }



        public ActionResult Create()
        {
            string userEmail = Session["userEmail"] as string;

            var consultant = entities.Consultants.FirstOrDefault(c => c.Email == userEmail);
            if (consultant == null)
            {
                return HttpNotFound("Consultant not found");
            }

            // 🔹 Fetch Customers of Consultant
            var customerIds = entities.ConsultantCustomers
                .Where(cc => cc.ConsultantIdFK == consultant.ConsultantIdPK)
                .Select(cc => cc.CustomerIdFK)
                .ToList();

            var customers = entities.Customers
                .Where(c => customerIds.Contains(c.CustomerIdPK))
                .ToList();

            // 🔹 Fetch Employers of Consultant
            var employerIds = entities.ConsultantEmployers
                .Where(ce => ce.ConsultantIdFK == consultant.ConsultantIdPK)
                .Select(ce => ce.EmployerIdFK)
                .ToList();

            var employers = entities.Employers
                .Where(e => employerIds.Contains(e.EmployerIdPK))
                .ToList();

            ViewBag.Customers = customers;
            ViewBag.Employers = employers;

            return View();
        }


        [HttpPost]
        public ActionResult Create(int month, int year, List<TimesheetDetail> details)
        {
            // Return the number of days in the selected month and year
            //int daysInMonth = DateTime.DaysInMonth(year, month);
            //return Json(new { daysInMonth = daysInMonth });

            string userEmail = Session["userEmail"] as string;
            var consultant = entities.Consultants.FirstOrDefault(c => c.Email == userEmail);

            if (consultant == null)
            {
                return HttpNotFound("Consultant not found");
            }

            var timesheet = new Timesheet
            {
                ConsultantId = consultant.ConsultantIdPK,
                Month = month,
                Year = year,
                TotalHours = details.Sum(d => d.Hours),
                Approval = "pending",
                CreatedAt = DateTime.Now
            };

            entities.Timesheets.AddObject(timesheet);
            entities.SaveChanges(); // Save first to generate TimesheetId

            foreach (var detail in details)
            {
                var timesheetDetail = new TimesheetDetail
                {
                    TimesheetId = timesheet.TimesheetId,
                    WorkDate = detail.WorkDate, // Convert WorkDate to DateTime
                    Description = detail.Description,
                    Hours = detail.Hours
                };

                entities.TimesheetDetails.AddObject(timesheetDetail);
            }

            entities.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ValidateCustomerEmployer(int customerId, int employerId)
        {
            var isValidRelation = entities.CustomerEmployers
                .Any(ce => ce.CustomerIdFK == customerId && ce.EmployerIdFK == employerId);

            if (!isValidRelation)
            {
                return Json(new { success = false, message = "Selected Customer is not related to the selected Employer." });
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult GetDays(int month, int year)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            return Json(new { daysInMonth = daysInMonth });
        }

        //[HttpPost]
        //public ActionResult SaveTimesheet(int month, int year, List<TimesheetDetail> details)
        //{
        //    string userEmail = Session["userEmail"] as string;
        //    var consultant = entities.Consultants.FirstOrDefault(c => c.Email == userEmail);
        //    if (consultant == null)
        //    {
        //        return HttpNotFound("Consultant not found");
        //    }

        //    var timesheet = new Timesheet
        //    {
        //        ConsultantId = consultant.ConsultantIdPK,
        //        Month = month,
        //        Year = year,
        //        TotalHours = details.Sum(d => d.Hours),
        //        Approval = "pending",
        //        CreatedAt = DateTime.Now
        //    };

        //    entities.Timesheets.AddObject(timesheet);


        //    foreach (var detail in details)
        //    {
        //        detail.TimesheetId = timesheet.TimesheetId;
        //        //detail.WorkDate = timesheet.
        //        //entities.TimesheetDetails.AddObject(detail);
        //    }

        //    entities.SaveChanges();

        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public JsonResult SaveTimesheet(int month, int year, int customerId, int employerId, List<TimesheetDetail> details)
        {
            foreach (var detail in details)
            {
                if ((detail.Hours ?? 0) > 12 || (detail.OTHours ?? 0) > 6)
                {
                    return Json(new { success = false, message = "Invalid input: Hours must be ≤ 12 and OT ≤ 6" });
                }
            }

            string userEmail = Session["userEmail"] as string;
            var consultant = entities.Consultants.FirstOrDefault(c => c.Email == userEmail);

            if (consultant == null)
            {
                return Json(new { success = false, message = "Consultant not found" });
            }

            // Calculate total hours (sum of Hours + OTHours)
            decimal totalHours = details.Sum(d => (d.Hours ?? 0) + (d.OTHours ?? 0));

            var timesheet = new Timesheet
            {
                ConsultantId = consultant.ConsultantIdPK,
                Month = month,
                Year = year,
                CustomerId = customerId,
                EmployerId = employerId,
                TotalHours = totalHours,
                Approval = "pending",
                CreatedAt = DateTime.Now
            };

            entities.Timesheets.AddObject(timesheet);
            entities.SaveChanges(); // Save to generate TimesheetId

            foreach (var detail in details)
            {
                var timesheetDetail = new TimesheetDetail
                {
                    TimesheetId = timesheet.TimesheetId,
                    WorkDate = detail.WorkDate,
                    Description = detail.Description,
                    Hours = detail.Hours ?? 0,
                    OTHours = detail.OTHours ?? 0,
                    SOWDeliverableNumber = detail.SOWDeliverableNumber ?? detail.SOWDeliverableNumber // fallback if needed
                };

                entities.TimesheetDetails.AddObject(timesheetDetail);
            }

            entities.SaveChanges();

            return Json(new { success = true });
        }


        public ActionResult ViewTimesheet(int timesheetId)
        {
            ViewBag.TimesheetId = timesheetId;
            return View();
        }

        public ActionResult EditTimesheet(int timesheetId)
        {
            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
            if (timesheet == null) return HttpNotFound();
            if (timesheet.Approval == "approved")
                return new HttpStatusCodeResult(403, "Editing not allowed for approved timesheets.");

            ViewBag.IsReadOnly = (timesheet?.Approval == "approved");

            ViewBag.TimesheetId = timesheetId;
            return View();

        }
        //[HttpGet]
        //public JsonResult GetTimesheetDetails(int timesheetId)
        //{
        //    var timesheet = entities.Timesheets
        //        .FirstOrDefault(t => t.TimesheetId == timesheetId);

        //    if (timesheet == null)
        //    {
        //        return Json(new { success = false, message = "Timesheet not found" }, JsonRequestBehavior.AllowGet);
        //    }

        //    var details = entities.TimesheetDetails
        //        .Where(td => td.TimesheetId == timesheetId)
        //        .Select(td => new
        //        {
        //            WorkDate = td.WorkDate  ,
        //            Description = td.Description,
        //            Hours = td.Hours
        //        }).ToList();

        //    return Json(new
        //    {
        //        success = true,
        //        month = timesheet.Month,
        //        year = timesheet.Year,
        //        details = details
        //    }, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult GetTimesheetDetails(int timesheetId)
        {
            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);

            if (timesheet == null)
            {
                return Json(new { success = false, message = "Timesheet not found" }, JsonRequestBehavior.AllowGet);
            }

            int year = (int)timesheet.Year;
            int month = (int)timesheet.Month;
            int CustomerId = (int)timesheet.CustomerId;
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // Convert month number to month name
            string monthName = new DateTime(year, month, 1).ToString("MMMM");

            var existingDetails = entities.TimesheetDetails
                .Where(td => td.TimesheetId == timesheetId)
                .ToList();

            // Create a full list of days for the month, filling in details where available
            var details = Enumerable.Range(1, daysInMonth)
                .Select(day =>
                {
                    var date = new DateTime(year, month, day);
                    var detail = existingDetails.FirstOrDefault(td => td.WorkDate == date);

                    return new
                    {
                        WorkDate = date.ToString("yyyy-MM-dd"), // Properly formatted date
                        SOWDeliverableNumber = detail?.SOWDeliverableNumber ?? "",
                        Description = detail?.Description ?? "",
                        Hours = detail?.Hours ?? 0,
                        OTHours = detail?.OTHours ?? 0
                    };
                })
                .ToList();
            decimal totalHours = details.Sum(d => d.Hours);
            decimal totalOTHours = details.Sum(d => d.OTHours);
            return Json(new
            {
                success = true,
                month = monthName,
                CustomerId= CustomerId,
                year = year,
                details = details,
                totalHours = totalHours, // Sending total sum to frontend
                totalOTHours = totalOTHours,
                ytdTotal = 0, // Placeholder for future enhancement
                ytdHoursLeft = 2160
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateTimesheet(int timesheetId, List<TimesheetDetail> details)
        {
            foreach (var detail in details)
            {
                if ((detail.Hours ?? 0) > 12 || (detail.OTHours ?? 0) > 6)
                {
                    return Json(new { success = false, message = "Invalid input: Hours must be ≤ 12 and OT ≤ 6" });
                }
            }

            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);

            if (timesheet == null)
            {
                return HttpNotFound("Timesheet not found");
            }

            if (timesheet?.Approval == "approved")
                return new HttpStatusCodeResult(403, "Editing not allowed for approved timesheets.");

            // Fetch existing timesheet details
            var existingDetails = entities.TimesheetDetails.Where(td => td.TimesheetId == timesheetId).ToList();

            // Create a dictionary for quick lookup
            var existingDetailsDict = existingDetails.ToDictionary(td => td.WorkDate);

            foreach (var detail in details)
            {
                if (existingDetailsDict.ContainsKey(detail.WorkDate))
                {
                    // Update existing record
                    var existingDetail = existingDetailsDict[detail.WorkDate];
                    existingDetail.Description = detail.Description;
                    existingDetail.Hours = detail.Hours;
                    existingDetail.SOWDeliverableNumber = detail.SOWDeliverableNumber; // Update SOW Deliverable
                    existingDetail.OTHours = detail.OTHours;
                }
                else
                {
                    // Add new entry
                    entities.TimesheetDetails.AddObject(new TimesheetDetail
                    {
                        TimesheetId = timesheetId,
                        WorkDate = detail.WorkDate,
                        Description = detail.Description,
                        Hours = detail.Hours,
                        SOWDeliverableNumber = detail.SOWDeliverableNumber, // Add SOW Deliverable
                        OTHours = detail.OTHours
                    });
                }
            }

            // Find records to remove (that are no longer in the updated details)
            var updatedDates = details.Select(d => d.WorkDate).ToList();
            var toRemove = existingDetails.Where(d => !updatedDates.Contains(d.WorkDate)).ToList();

            // Remove each entry manually (instead of RemoveRange)
            foreach (var item in toRemove)
            {
                entities.TimesheetDetails.DeleteObject(item);
            }

            var totalHours = details.Sum(d => d.Hours);
            timesheet.TotalHours = totalHours;

            entities.SaveChanges();

            return Json(new { success = true, message = "Timesheet updated successfully" });
        }


        public ActionResult Edit(int id)
        {
            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == id);
            if (timesheet == null)
            {
                return HttpNotFound();
            }

            var daysInMonth = DateTime.DaysInMonth((int)timesheet.Year, (int)timesheet.Month);
            var timesheetDetails = entities.TimesheetDetails.Where(td => td.TimesheetId == id).ToList();

            if (!timesheetDetails.Any())
            {
                for (int i = 1; i <= daysInMonth; i++)
                {
                    timesheetDetails.Add(new TimesheetDetail
                    {
                        WorkDate = new DateTime((int)timesheet.Year, (int)timesheet.Month, i),
                        Description = "",
                        Hours = 0
                    });
                }
            }

            ViewBag.TimesheetDetails = timesheetDetails;
            return View(timesheet);
        }

        [HttpPost]

        public ActionResult Edit(int id, List<TimesheetDetail> details)
        {
            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == id);
            if (timesheet == null)
            {
                return HttpNotFound();
            }

            // Remove old details manually
            var existingDetails = entities.TimesheetDetails.Where(td => td.TimesheetId == id).ToList();
            foreach (var detail in existingDetails)
            {
                entities.TimesheetDetails.DeleteObject(detail); // Correct for ObjectSet<T>
            }

            // Add new details manually
            foreach (var detail in details)
            {
                detail.TimesheetId = id; // Ensure association
                entities.TimesheetDetails.AddObject(detail); // Correct for ObjectSet<T>
            }

            entities.SaveChanges();

            // Update total hours
            timesheet.TotalHours = details.Sum(d => d.Hours);
            entities.SaveChanges();

            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public async Task<ActionResult> SendTimesheet(int timesheetId)
        //{
        //    var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
        //    if (timesheet == null)
        //    {
        //        return HttpNotFound("Timesheet not found");
        //    }

        //    // 🔹 Get Consultant's Customer from ConsultantCustomers table
        //    var consultantCustomer = entities.ConsultantCustomers.FirstOrDefault(e => e.ConsultantIdFK == timesheet.ConsultantId);
        //    if (consultantCustomer == null)
        //    {
        //        return Json(new { success = false, message = "Consultant's customer not found" }, JsonRequestBehavior.AllowGet);
        //    }

        //    // 🔹 Get Customer details from Customers table
        //    var customer = entities.Customers.FirstOrDefault(c => c.CustomerIdPK == consultantCustomer.CustomerIdFK);
        //    if (customer == null || string.IsNullOrEmpty(customer.CustomerContactEmail))
        //    {
        //        return Json(new { success = false, message = "Customer email not found" }, JsonRequestBehavior.AllowGet);
        //    }

        //    string customerEmail = customer.CustomerContactEmail;

        //    // 🔹 Generate PDF file


        //    // 🔹 Send Email
        //    bool emailSent = await SendEmailWithAttachment(customerEmail, pdfBytes, $"Timesheet_{timesheetId}.pdf");


        //    if (emailSent)
        //    {
        //        return Json(new { success = true, message = "Timesheet sent successfully" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "Failed to send timesheet" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //private byte[] GenerateTimesheetPdf(Timesheet timesheet)
        //{
        //    if (timesheet == null)
        //    {
        //        throw new ArgumentNullException(nameof(timesheet), "Timesheet cannot be null.");
        //    }
        //    QuestPDF.Settings.License = LicenseType.Community;

        //    return Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Size(PageSizes.A4);
        //            page.Margin(20);
        //            page.DefaultTextStyle(x => x.FontSize(12));

        //            page.Header()
        //                .AlignCenter()
        //                .Text("Timesheet Report")
        //                .SemiBold()
        //                .FontSize(18)
        //                .FontColor(Colors.Blue.Medium);

        //            page.Content()
        //                .Column(col =>
        //                {
        //                    col.Spacing(10);

        //                    // Consultant Info
        //                    col.Item().Text($"Timesheet ID: {timesheet.TimesheetId}").Bold();
        //                    col.Item().Text($"Consultant Name: {timesheet.ConsultantId}");
        //                    col.Item().Text($"Total Hours: {timesheet.TotalHours}");

        //                    col.Item().Text("Timesheet Details:").Bold().FontSize(14).FontColor(Colors.Blue.Darken2);

        //                    // Create a Table
        //                    col.Item().Table(table =>
        //                    {
        //                        table.ColumnsDefinition(columns =>
        //                        {
        //                            columns.ConstantColumn(100); // Date
        //                            columns.RelativeColumn(2);   // Description
        //                            columns.ConstantColumn(60);  // Hours
        //                        });

        //                        // Header Row
        //                        table.Header(header =>
        //                        {
        //                            header.Cell().Text("Date").Bold();
        //                            header.Cell().Text("Description").Bold();
        //                            header.Cell().Text("Hours").Bold();
        //                        });

        //                        // Timesheet Data
        //                        foreach (var detail in timesheet.TimesheetDetails)
        //                        {
        //                            table.Cell().Text(detail.WorkDate?.ToShortDateString() ?? "N/A");
        //                            table.Cell().Text(detail.Description);
        //                            table.Cell().Text(detail.Hours.ToString());
        //                        }
        //                    });
        //                });

        //            page.Footer()
        //                .AlignCenter()
        //                .Text($"Generated on {DateTime.Now:MMMM dd, yyyy}")
        //                .FontSize(10)
        //                .Italic()
        //                .FontColor(Colors.Grey.Darken2);
        //        });
        //    }).GeneratePdf();
        //}


        // 🔹 Helper Methods for Styling


        public ActionResult DownloadTimesheetPdf(int timesheetId)
        {
            // Fetch Timesheet Record
            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
            string userEmail = Session["userEmail"] as string;
            if (timesheet == null)
            {
                return HttpNotFound("Timesheet not found.");
            }
            int month = (int)timesheet.Month;  // Assuming there is a 'Month' field in your Timesheet table
            int year = DateTime.Now.Year; // Use current year or fetch it from Timesheet if available

            DateTime monthStartDate = new DateTime(year, month, 1);
            DateTime monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

            // Fetch Consultant Details
            var consultant = entities.Consultants.FirstOrDefault(u => u.Email == userEmail);

            var timesheetEntries = entities.TimesheetDetails
               .Where(d => d.TimesheetId == timesheetId)
               .ToList() // Fetch data first
               .Select(d => new TimesheetEntryModel
               {
                   Date = d.WorkDate.HasValue ? d.WorkDate.Value.ToString("yyyy-MM-dd") : "N/A", // Now safe
                   Description = d.Description,
                   Hours = (decimal)d.Hours
               }).ToList();

            // Fetch Company Information
            // Assuming single company record

            // Populate the View Model
            var timesheetPdfModel = new TimesheetPdfModel
            {
                CompanyName = "H3U Systems",
                CompanyPhone = "123-456-7890",
                CompanyEmail = "info@yourcompany.com",
                CompanyAddress1 = "123 Street",
                CompanyAddress2 = "Round 2",
                CompanyCity = "City",
                CompanyState = "State",
                CompanyZip = "000000",
                CompanyFax = "",

                ConsultantName = consultant?.FirstName ?? "Consultant Name",
                PositionTitle = consultant?.UserName ?? "Consultant",

                TimesheetId = timesheet.TimesheetId,
                MonthStartDate = monthStartDate.ToString("yyyy-MM-dd"),
                MonthEndDate = monthEndDate.ToString("yyyy-MM-dd"),
                TotalHours = timesheetEntries.Sum(e => e.Hours),


                TimesheetEntries = timesheetEntries
            };

            // Generate PDF
            return new ViewAsPdf("TimesheetPdf", timesheetPdfModel)
            {
                FileName = $"Timesheet_{timesheetPdfModel.ConsultantName}_{timesheetPdfModel.MonthStartDate}.pdf"
            };
        }




        private async Task<bool> SendEmailWithAttachment(string recipientEmail, byte[] fileBytes, string fileName)
        {
            try
            {
                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("shaikuzairuzair@gmail.com", "prmwmduftlcwhomx");
                    smtpClient.EnableSsl = true;

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress("your-email@gmail.com");
                        mailMessage.To.Add(recipientEmail);
                        mailMessage.Subject = "Timesheet Submission";
                        mailMessage.Body = "<p>Dear Consultant,</p><p>Please find attached the timesheet.</p>";
                        mailMessage.IsBodyHtml = true;

                        // Attach PDF file
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(fileBytes), fileName));

                        Console.WriteLine("Sending email...");
                        await smtpClient.SendMailAsync(mailMessage);
                        Console.WriteLine("Email sent successfully!");

                        return true;
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine("SMTP Exception: " + smtpEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Exception: " + ex.Message);
            }
            return false;
        }

        public ActionResult PendingTimesheets()
        {
            var pendingTimesheets = entities.Timesheets
                .Where(t => t.Approval == "pending")
                .Select(t => new TimesheetViewModel // ✅ Use ViewModel instead of Timesheet entity
                {
                    TimesheetId = t.TimesheetId,
                    Month = (int)t.Month,
                    Year = (int)t.Year,
                    TotalHours = (int)t.TotalHours,
                    CustomerName = t.Customer.CustomerName, // ✅ Fetch customer name
                    EmployerName = t.Employer.EmployerName  // ✅ Fetch employer name
                })
                .ToList();

            return View(pendingTimesheets);
        }

        public ActionResult ApproveTimesheets()
        {
            var pendingTimesheets = entities.Timesheets
                .Where(t => t.Approval == "pending")
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TimesheetViewModel
                {
                    TimesheetId = t.TimesheetId,

                    Month = (int)t.Month,
                    Year = (int)t.Year,
                    TotalHours = (int)t.TotalHours,
                    CustomerName = t.Customer.CustomerName,
                    EmployerName = t.Employer.EmployerName
                }).ToList();

            var approvedTimesheets = entities.Timesheets
                .Where(t => t.Approval == "approved")
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TimesheetViewModel
                {
                    TimesheetId = t.TimesheetId,

                    Month = (int)t.Month,
                    Year = (int)t.Year,
                    TotalHours = (int)t.TotalHours,
                    CustomerName = t.Customer.CustomerName,
                    EmployerName = t.Employer.EmployerName
                }).ToList();

            var rejectedTimesheets = entities.Timesheets
                .Where(t => t.Approval == "rejected")
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TimesheetViewModel
                {
                    TimesheetId = t.TimesheetId,
                    Month = (int)t.Month,
                    Year = (int)t.Year,
                    TotalHours = (int)t.TotalHours,
                    CustomerName = t.Customer.CustomerName,
                    EmployerName = t.Employer.EmployerName
                }).ToList();

            var model = new ApproveTimesheetsViewModel
            {
                PendingTimesheets = pendingTimesheets,
                ApprovedTimesheets = approvedTimesheets,
                RejectedTimesheets = rejectedTimesheets
            };

            return View(model);
        }

        // Approve Timesheet
        [HttpPost]
        public ActionResult Approve(int timesheetId)
        {
            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);

            if (timesheet != null)
            {
                timesheet.Approval = "approved";
                entities.SaveChanges();
            }
            return Json(new { success = true });
        }

        // Reject Timesheet
        [HttpPost]
        public ActionResult Reject(int timesheetId)
        {
            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);

            if (timesheet != null)
            {
                timesheet.Approval = "rejected";
                entities.SaveChanges();
            }
            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult GetTimesheetDetailsForInvoice(int timesheetId)
        {
            var timesheet = entities.Timesheets
                .Where(t => t.TimesheetId == timesheetId)
                .Select(t => new
                {
                    t.TotalHours,
                    //   OvertimeHours = (t.OvertimeHours ?? 0), // Explicit null check
                    t.CustomerId,
                    t.ConsultantId
                })
                .FirstOrDefault();

            return Json(timesheet, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetConsultantPositionDetails(int consultantId)
        {


            var consultantPosition = entities.ConsultantPositionDetails
                .Where(cpd => cpd.ConsultantIdFK == consultantId)
                .Select(cpd => new
                {
                    cpd.PositionRateIdFK,
                    cpd.ConsultantPositionIdPK,
                    cpd.PositionIdFK
                })
                .FirstOrDefault();

            return Json(consultantPosition, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetPositionRate(int positionRateId)
        {
            var positionRate = entities.PositionRates
                .Where(pr => pr.PositionRateIdPK == positionRateId)
                .Select(pr => new
                {
                    pr.Rate,
                    OvertimeRate = (pr.OvertimeRate ?? 0) // Explicitly handling null
                })
                .FirstOrDefault();

            return Json(positionRate, JsonRequestBehavior.AllowGet);
        }

    }
}
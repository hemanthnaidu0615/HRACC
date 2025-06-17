using HRACCPortal.Edmx;
using HRACCPortal.Models;
using HRACCPortal.Services;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class EmployerController : Controller
    {

        // GET: Employer
        public HRACCDBEntities entities;
        clsCrud cls;
        private EmployerService _employerService;

        public EmployerController()
        {
            entities = new HRACCDBEntities();
            cls = new clsCrud();

        }


        private ApplicationUserManager UserManager
        {
            get
            {
                if (HttpContext != null && HttpContext.GetOwinContext() != null)
                {
                    return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                return null;
            }
        }

        private ApplicationSignInManager SignInManager
        {
            get
            {
                if (HttpContext != null && HttpContext.GetOwinContext() != null)
                {
                    return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                }
                return null;
            }
        }

        private EmployerService EmployerService
        {
            get
            {
                if (_employerService == null)
                {
                    var userManager = UserManager;
                    var signInManager = SignInManager;
                    if (userManager != null && signInManager != null)
                    {
                        _employerService = new EmployerService(userManager, signInManager);
                    }
                }
                return _employerService;
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEmployer()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult>  AddEmployer(EmployerModel employer)
        {

            string message = "";
            string mess = "";
            string redirectUrl = "";
            string randomPassword = GenerateRandomPassword();
            if (Session["UserRole"]?.ToString() != "1") // Admin RoleId = 1
            {
                return View("UnauthorizedAccess", "Account");

            }
            try
            {
                message = cls.AddEmployer(employer);

                if (Session["UserRole"]?.ToString() != "1")
                {
                    redirectUrl = "old";
                }
                else
                {
                    redirectUrl = "/Employee/ViewEmployees";

                }
             

                if (message == "success")
                {
                    // Create a RegisterViewModel and pass it to the service for user registration
                    var registerViewModel = new RegisterViewModel
                    {
                        Email = employer.EmployerContactEmail,
                        Password = randomPassword,
                        ConfirmPassword = randomPassword,
                        PhoneNumber = employer.EmployerContactPhone,
                        Active = employer.isActive,
                        UserName = employer.EmployerContactEmail,
                        RoleId = "2",
                    };


                    // Debugging RegisterConsultantAsync call
                    mess = await EmployerService.RegisterEmployerAsync(registerViewModel);

                    if (mess == "success")
                    {
                        await SendWelcomeEmail(employer.EmployerContactEmail, employer.EmployerContactEmail, randomPassword);
                    }
                    // Call the service directly

                }
                else
                {
                    mess = "Error adding consultant to the database.";
                }
                TempData["EmployerAddedMessage"] = "Employer added successfully. Please add at least one employee.";

            }
            catch (Exception e)
            {
                message = e.Message;
            }


            return Json(new { message = message, redirectUrl = redirectUrl, JsonRequestBehavior.AllowGet });
        }


        public ActionResult ViewEmployers()
        {
            var userRole = Session["UserRole"]?.ToString();
            var userEmail = Session["UserEmail"]?.ToString();
            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            if (userRole == "1") // Admin RoleId = 1
            {
                cls.GetEmployers();
                return View(cls);
            }
            else if(userRole == "3")
            {
                cls.GetEmployersForEmployee(userEmail);
                return View(cls);
            }
            else
            {
                // Fetch employer by logged-in user's email
                cls.GetEmployerByEmail(userEmail);
                return View(cls);
             
            }

        }


        public ActionResult EditEmployer(int id)
        {
            EmployerModel cl = cls.GetEmployerById(id);
            return Json(new { cl = cl, JsonRequestBehavior.AllowGet });
        }

        [HttpGet]
        public ActionResult AssignEmployers(int employeeId)
        {
            // Call the existing GetEmployers method
            cls.GetEmployers();

            var employers = cls.EmployerList.Where(employer => employer.isActive).Select(employer => new EmployerModel
            {
                EmployerIdPK = employer.EmployerIdPK,
                EmployerName = employer.EmployerName,
                EmployerContactEmail = employer.EmployerContactEmail
            }).ToList();

            // Get all employers assigned to the employee
            var assignedEmployerIds = entities.EmployeeEmployers
                .Where(ee => ee.EmployeeIdFK == employeeId)
                .Select(ee => ee.EmployerIdFK)
                .ToHashSet();

            ViewBag.AssignedEmployerIds = assignedEmployerIds ?? new HashSet<int>();
            ViewBag.EmployeeId = employeeId;

            return View(employers);
        }



        [HttpPost]
        public ActionResult SaveEmployeeEmployers(int employeeId, List<int> selectedEmployerIds)
        {
            if (selectedEmployerIds == null || !selectedEmployerIds.Any())
            {
                return Json(new { success = false, message = "No employers selected" });
            }

            try
            {
                // Step 1: Remove all existing employers assigned to this employee
                var existingAssignments = entities.EmployeeEmployers
                    .Where(ee => ee.EmployeeIdFK == employeeId)
                    .ToList();

                foreach (var assignment in existingAssignments)
                {
                    entities.EmployeeEmployers.DeleteObject(assignment);
                }

                // Step 2: Add newly selected employers
                foreach (var employerId in selectedEmployerIds)
                {
                    entities.EmployeeEmployers.AddObject(new EmployeeEmployer
                    {
                        EmployeeIdFK = employeeId,
                        EmployerIdFK = employerId,
                        DateAdded = DateTime.Now
                    });
                }

                // Save all changes
                entities.SaveChanges();

                return Json(new { success = true, message = "Employers assigned successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ViewAssignedEmployers(int employeeId)
        {
            // Fetch employee information
            var employee = entities.EmployeeEmployers.FirstOrDefault(e => e.EmployeeIdFK == employeeId);
         

            // Fetch assigned employers' details
            var assignedEmployers = entities.EmployeeEmployers
             .Where(ee => ee.EmployeeIdFK == employeeId)
             .Join(entities.Employers,
                 ee => ee.EmployerIdFK,
                 employer => employer.EmployerIdPK,
                 (ee, employer) => new
                 {
                     employer.EmployerIdPK,
                     employer.EmployerName,
                     employer.EmployerContactEmail,
                     employer.EmployerContactPhone,
                     ee.DateAdded // still DateTime here
                 })
             .ToList() // fetch everything from the database
             .Select(x => new EmployerModel
             {
                 EmployerIdPK = x.EmployerIdPK,
                 EmployerName = x.EmployerName,
                 EmployerContactEmail = x.EmployerContactEmail,
                 EmployerContactPhone = x.EmployerContactPhone,
                 DateAdded = x.DateAdded.ToString("dd-MMM-yyyy") // now safely format in-memory
             })
             .ToList();

            ViewBag.EmployeeId = employeeId;

            return View(assignedEmployers); // Return to View with the data
        }

        private string GenerateRandomPassword()
        {
            const int passwordLength = 12;
            const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_+[]{}|;:,.<>?";
            const string allChars = upperCaseChars + lowerCaseChars + digits + specialChars;

            Random random = new Random();
            StringBuilder password = new StringBuilder();

            // Ensure at least one character from each required set
            password.Append(upperCaseChars[random.Next(upperCaseChars.Length)]);
            password.Append(lowerCaseChars[random.Next(lowerCaseChars.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            // Fill the remaining characters randomly
            for (int i = 4; i < passwordLength; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the characters to ensure randomness
            return new string(password.ToString().OrderBy(_ => random.Next()).ToArray());
        }

        private async Task SendWelcomeEmail(string email, string UserName, string temporaryPassword)
        {
            try
            {
                // Use the correct SMTP host for Gmail
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Port for TLS
                    Credentials = new NetworkCredential("shaikuzairuzair@gmail.com", "fuuanbubiwfpfavl"), // Use your app password
                    EnableSsl = true, // Enable SSL/TLS
                };

                string subject = "Welcome to HRACC Portal";
                string body = $"" +
                             $"<h1>Welcome to HRACC Portal</h1>" +
                             $"<p>Dear Employer,</p>" +
                             $"<p>We are excited to welcome you to the HRACC Portal. Below are your login details:</p>" +
                             $"<p><strong>Login Link:</strong> <a href='https://your-portal-login-url.com'>Login Here</a></p>" +
                             $"<p><strong> User Name : </strong> {UserName} </p>" +
                             $"<p><strong> Email :</strong>{email} </p>" +
                             $"<p><strong>Temporary Password:</strong> {temporaryPassword}</p>" +
                             $"<p>Please use this password to log in and change it immediately for security purposes.</p>" +
                             $"<p>Best Regards,<br>HRACC Portal Team</p>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("shaikuzairuzair@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                // Add recipient
                mailMessage.To.Add(email);

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log or handle the email sending failure
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }
}
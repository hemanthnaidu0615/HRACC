using HRACCPortal.Edmx;
using HRACCPortal.Models;
using HRACCPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRACCPortal.Helpers;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Text;
namespace HRACCPortal.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public HRACCDBEntities entities;
        private EmployeeService _employeeService;
        clsCrud cls;
        public EmployeeController()
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

        private EmployeeService EmployeeService
        {
            get
            {
                if (_employeeService == null)
                {
                    var userManager = UserManager;
                    var signInManager = SignInManager;
                    if (userManager != null && signInManager != null)
                    {
                        _employeeService = new EmployeeService(userManager, signInManager);
                    }
                }
                return _employeeService;
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddEmployee(EmployeeModel employee)
        {


            string message = "";
            string mess = "";
            string randomPassword = GenerateRandomPassword();
            try
            {
                message = cls.AddEmployee(employee);
                if (message == "success")
                {
                    // Create a RegisterViewModel and pass it to the service for user registration
                    var registerViewModel = new RegisterViewModel
                    {
                        Email = employee.EmployeeEmail,
                        Password = randomPassword,
                        ConfirmPassword = randomPassword,
                        PhoneNumber = employee.EmployeePhone,
                        Active = employee.isActive,
                        UserName = employee.EmployeeEmail,
                        RoleId = "3",
                    };


                    // Debugging RegisterConsultantAsync call
                    mess = await EmployeeService.RegisterEmployeeAsync(registerViewModel);

                    if (mess == "success")
                    {
                        await SendWelcomeEmail(employee.EmployeeEmail, employee.EmployeeEmail, randomPassword);
                    }
                    // Call the service directly

                }
                else
                {
                    mess = "Error adding consultant to the database.";
                }
            }
            catch (Exception e)
            {
                message = e.Message;
            }


            return Json(new { message = message, JsonRequestBehavior.AllowGet });
        }


        public ActionResult ViewEmployees()
        {
            var userRole = Session["UserRole"]?.ToString();
            var userEmail = Session["UserEmail"]?.ToString();

            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }
            if (userRole == "1") // Admin RoleId = 1
            {
                cls.GetEmployees();
                return View(cls);
            }
            else if (userRole == "2") // Employer RoleId = 2
            {
                cls.GetEmployeesForEmployer(userEmail); // Employer sees their employees
                return View(cls);
            }
            else  // Employee RoleId = 3
            {
                cls.GetEmployeesForEmployee(userEmail); // Employee sees employees for associated employers
                return View(cls);
            }
            return View(cls);
        }
        public ActionResult EditEmployee(int id)
        {
            EmployeeModel cl = cls.GetEmployeeById(id);
            return Json(new { cl = cl, JsonRequestBehavior.AllowGet });
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
                             $"<p>Dear Employee,</p>" +
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

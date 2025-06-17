using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using HRACCPortal.Models;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Web.Helpers;
using HRACCPortal.Edmx;
using System.Security.Cryptography;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Infrastructure;

namespace HRACCPortal.Controllers
{
    [Authorize]

    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ApplicationUserManager _usersManager;
        public HRACCDBEntities _db;
        // private SignInStatus result;


        public AccountController()
        {
            _db = new HRACCDBEntities();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            _db = new HRACCDBEntities();

            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            else
            {
                ViewBag.ReturnUrl = "/Invoice/ViewInvoice";// "@Url.Action("ViewInvoice","Invoice")@Url.Action("ViewInvoice","Invoice")"
            }
            return View();
        }
        public ActionResult Lockout()
        {
            return View();
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Entered email is incorrect.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            if (user.IsLocked == true)
            {
                return View("ContactAdminstrator");
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                     user = await UserManager.FindByEmailAsync(model.Email);
                    if (user.IsLocked == true)
                    {
                        return View("ContactAdminstrator");
                    }
                    // Check if it's the user's first login
                    if (user != null && user.IsFirstLogin)
                    {
                        // Redirect to security questions page if it's the user's first login
                        return RedirectToAction("ResetPasswordModel", "Account", new { IsFirstLogin = user.IsFirstLogin, email = user.Email });
                    }
                    Session["UserRole"] = user.RoleId;
                    Session["UserEmail"] = user.Email;
                    // Otherwise, redirect to the return URL
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("Password", "Incorrect Password");
                    ViewBag.ReturnUrl = returnUrl;
                
                    return View(model);
            }
        }


        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, IsFirstLogin = true ,RoleId=model.RoleId};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);



                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    //  string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //  var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("ResetPasswordModel", "Account", new { IsFirstLogin = true, email = model.Email });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult UnauthorizedAccess()
        {
            return View();
        }

        //public async Task<ActionResult> AddConsultant(ConsultantModel consultant)
        //{
        //    string message = "";
        //    try
        //    {
        //        // Check if user already exists by email
        //        var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == consultant.Email);

        //        string temporaryPassword = GenerateTemporaryPassword();

        //        if (user != null)
        //        {
        //            // Existing user: update details
        //            user.UserName = consultant.Email;
        //            user.Email = consultant.Email;
        //            user.PhoneNumber = consultant.Phone;
        //            user.IsFirstLogin = true;
        //            user.LockoutEnabled = !consultant.Active;

        //            user.IsLocked = !consultant.Active;

        //            user.PasswordHash = temporaryPassword;

        //            _db.SaveChanges();


        //        }
        //        else
        //        {
        //            // New user: create a new user
        //            var newUser = new AspNetUser
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                UserName = consultant.Email,
        //                Email = consultant.Email,
        //                PhoneNumber = consultant.Phone,
        //                IsFirstLogin = true,
        //                LockoutEnabled = !consultant.Active,
        //                IsLocked = !consultant.Active,
        //                FailedLoginAttempts = 0

        //            };

        //            newUser.PasswordHash =HashPassword(temporaryPassword); ;
        //            _db.AspNetUsers.AddObject(newUser);  // Add the new user to the database
        //            _db.SaveChanges();

        //            var applicationUser = new ApplicationUser
        //            {
        //                Id = newUser.Id,
        //                UserName = newUser.UserName,
        //                Email = newUser.Email,
        //                PhoneNumber = newUser.PhoneNumber,
        //                IsFirstLogin =true,
        //                LockoutEnabled = !consultant.Active,
        //                IsLocked = !consultant.Active,
        //                FailedLoginAttempts = 0
        //            };

        //            // Create a ClaimsIdentity for the new user
        //            var userIdentity = await UserManager.CreateIdentityAsync(applicationUser, DefaultAuthenticationTypes.ApplicationCookie);
        //            await UserManager.UpdateAsync(applicationUser);

        //            message = "User created successfully!";
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        message = e.Message;
        //    }

        //    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        //}




        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }



        [HttpGet]
        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LogOff()
        //{
        //    Session.Clear();
        //    Session.Abandon();
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //    return RedirectToAction("Login", "Account");
        //}

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }



        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Login", "Account");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        private List<SecurityQuestionModel> GetSecurityQuestions()
        {
            return new List<SecurityQuestionModel>
        {
            new SecurityQuestionModel { Id = 1, Question = "What is your mother's maiden name?" },
            new SecurityQuestionModel { Id = 2, Question = "What was your first pet's name?" },
            new SecurityQuestionModel { Id = 3, Question = "What is the name of the city you were born in?" }
        };
        }

        // Example usage in password reset process
        // GET: Reset Password
        public ActionResult ResetPasswordModel(bool? IsFirstLogin, string Email)
        {
            bool isFirstLogin = IsFirstLogin ?? false;

            var model = new ResetPasswordModel
            {
                Email = Email,
                IsFirstLogin = isFirstLogin,
                SecurityQuestions = GetSecurityQuestions()
            };
            return View(model);
        }

        // POST: Reset Password



        // POST: Reset Password or First-Time Setup
        [HttpPost]
        public ActionResult ResetPasswordModel(ResetPasswordModel model)
        {
            var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return HttpNotFound("User not found.");
            }

            if (!ModelState.IsValid)
            {
                model.SecurityQuestions = GetSecurityQuestions();
                return View(model);
            }

           

            if (user.IsFirstLogin == true)
            {
                // Save security questions and answers for first-time login
                SaveSecurityQuestions(user.Id, model);
                user.IsFirstLogin = false; // Mark first login complete
                _db.SaveChanges();
                TempData["Message"] = "Security questions set successfully. You can now log in.";
                return RedirectToAction("NewPassword", new { email = model.Email });
            }
            else
            {
                // Verify security questions during password reset
                if (!VerifyAnswers(user.Id, model))
                {
                    ModelState.AddModelError("", "One or more answers are incorrect.");
                    model.SecurityQuestions = GetSecurityQuestions();
                    return View(model);
                }

                // If answers are correct, redirect to New Password page
                return RedirectToAction("NewPassword", new { email = model.Email });
            }
        }

        private void SaveSecurityQuestions(string userId, ResetPasswordModel model)
        {
            var securityAnswers = new List<SecurityQuestion>
            {
                new SecurityQuestion { UserId = userId, Question = model.SelectedQuestion1, AnswerHash = model.Answer1 },
                new SecurityQuestion { UserId = userId, Question = model.SelectedQuestion2, AnswerHash = model.Answer2 },
                new SecurityQuestion { UserId = userId, Question = model.SelectedQuestion3, AnswerHash = model.Answer3 }
            };

            // Add each security answer one by one
            foreach (var securityAnswer in securityAnswers)
            {
                _db.SecurityQuestions.AddObject(securityAnswer);
            }

            // Save changes to the database
            _db.SaveChanges();
        }

        private bool VerifyAnswers(string userId, ResetPasswordModel model)
        {
            var userAnswers = _db.SecurityQuestions.Where(q => q.UserId == userId).ToList();

            return userAnswers.Any(q => q.Question == model.SelectedQuestion1 && string.Equals(q.AnswerHash, model.Answer1, StringComparison.OrdinalIgnoreCase)) &&
                   userAnswers.Any(q => q.Question == model.SelectedQuestion2 && q.AnswerHash == model.Answer2) &&
                   userAnswers.Any(q => q.Question == model.SelectedQuestion3 && q.AnswerHash == model.Answer3);



        }

        // GET: New Password Page
        [HttpGet]
        public ActionResult NewPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account"); // Redirect if email is missing
            }

            var model = new NewPasswordModel { Email = email };
            return View(model);
        }

        [HttpGet]
        public ActionResult TryAnotherWay()
        {
            
            return View();
        }


        [HttpPost]
        public JsonResult VerifyEmail(string email)
        {
            var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Fetch the user's security question IDs
            var userQuestionKeys = _db.SecurityQuestions
                            .Where(q => q.UserId == user.Id)
                            .Select(q => q.Question) // Fetch 'Question' as a string
                            .ToList();

            // Get the complete list of security questions
            var allQuestions = GetSecurityQuestions();

            var questions = allQuestions
                 .Where(q => userQuestionKeys.Contains(q.Id.ToString())) // Match IDs as strings
                 .Select(q => new { q.Id, q.Question }) // Select ID and Question for the response
                 .ToList();

            // Return the mapped questions as JSON
            return Json(new { success = true, questions });
        }
        // POST: Save New Password
        [HttpPost]
        public ActionResult NewPassword(NewPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return HttpNotFound("User not found.");
            }

            // Update the password in the database
            user.PasswordHash = HashPassword(model.Password); // Replace HashPassword with your hashing function
            _db.SaveChanges();

            TempData["Message"] = "Password has been reset successfully.";
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher();
            return passwordHasher.HashPassword(password);
        }

        [HttpGet]
        public ActionResult ContactAdministrator()
        {
            return View();
        }

        public ActionResult ResetPasswordForUser()
        {
            return View();
        }

        // POST: Fetch security questions based on email
        [HttpPost]
        public ActionResult ResetPasswordForUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View();
            }

            var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found with this email.");
                return View();
            }

            // Fetch security questions for the user
            var securityQuestions = _db.SecurityQuestions
                .Where(q => q.UserId == user.Id)
                .Select(q => new SecurityQuestionModel
                {
                    Question = q.Question,
                    AnswerHash = q.AnswerHash // AnswerHash will be compared later
                }).ToList();

            if (!securityQuestions.Any())
            {
                ModelState.AddModelError("", "No security questions found for this user.");
                return View();
            }

            // Return view with security questions
           
            return View(); // Redirect to a new view for answering questions
        }

        // POST: Verify security question answers
        [HttpPost]
        public ActionResult AnswerSecurityQuestions(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _db.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            // Validate answers
            var userQuestions = _db.SecurityQuestions.Where(q => q.UserId == user.Id).ToList();
            foreach (var question in model.SecurityQuestions)
            {
                var match = userQuestions.FirstOrDefault(q => q.Question == question.Question);
                if (match == null || match.AnswerHash != question.AnswerHash)
                {
                    ModelState.AddModelError("", "One or more answers are incorrect.");
                    return View(model);
                }
            }

            // Redirect to NewPassword page on success
            TempData["Message"] = "Answers verified. Please set a new password.";
            return RedirectToAction("NewPassword", new { email = model.Email });
        }
    

}

    // GET: Set New Password
    //public ActionResult SetNewPassword(string email)
    //{
    //    var model = new SetNewPasswordModel
    //    {
    //        Email = email
    //    };
    //    return View(model);
    //}

    // POST: Set New Password
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult SetNewPassword(SetNewPasswordModel model)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        var user = UserManager.FindByEmail(model.Email);
    //        if (user != null)
    //        {
    //            var resetResult = UserManager.ResetPasswordAsync(user.Id, model.CurrentPassword, model.NewPassword).Result;
    //            if (resetResult.Succeeded)
    //            {
    //                return RedirectToAction("Login", "Account");
    //            }
    //            else
    //            {
    //                ModelState.AddModelError("", "Password reset failed. Please try again.");
    //            }
    //        }
    //        else
    //        {
    //            ModelState.AddModelError("", "Email not found.");
    //        }
    //    }
    //    return View(model);
    //}



}

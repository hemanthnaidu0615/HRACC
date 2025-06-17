using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRACCPortal.Edmx;
using HRACCPortal.Models;

namespace HRACCPortal.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        // GET: Customer
        public HRACCDBEntities entities;
        clsCrud cls;
        public CustomerController()
        {
            entities = new HRACCDBEntities();
            cls = new clsCrud();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCustomer(CustomerModel customer)
        {


            string message = "";
            try
            {
                message = cls.AddCustomer(customer);

              
            }
            catch (Exception e)
            {
                message = e.Message;
            }


            return Json(new { message = message, JsonRequestBehavior.AllowGet });
        }


        //public ActionResult ViewCustomers()
        //{
        //    cls.GetCustomers();
        //    return View(cls);
        //}

        public ActionResult ViewCustomers()
        {
            var userRole = Session["UserRole"]?.ToString();
            var userEmail = Session["UserEmail"]?.ToString();

            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            if (userRole == "1") // Admin RoleId = 1
            {
                cls.GetCustomers(); // Fetch all customers for admin
            }
            else if (userRole == "2") // Employer RoleId = 2
            {
                cls.GetCustomersForEmployer(userEmail); // Fetch customers for the specific employer
            }
            else if (userRole == "3") // Employee RoleId = 3
            {
                cls.GetCustomersForEmployee(userEmail); // Fetch customers for all employers of the employee
            }
            else
            {
                // Handle unknown roles
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(cls); // Return the appropriate customer list
        }
        public ActionResult EditCustomer(int id)
        {
            CustomerModel cl = cls.GetCustomerById(id);
            return Json(new { cl = cl, JsonRequestBehavior.AllowGet });
        }

        [HttpGet]
        public ActionResult AssignCustomerEmployers(int customerId)
        {
            // Populate the Employer list
            cls.GetEmployers();

            var employers = cls.EmployerList.Select(employer => new EmployerModel
            {
                EmployerIdPK = employer.EmployerIdPK,
                EmployerName = employer.EmployerName,
                EmployerContactEmail = employer.EmployerContactEmail
            }).ToList();

            // Get all assigned employer IDs for the customer
            var assignedEmployerIds = entities.CustomerEmployers
                .Where(ce => ce.CustomerIdFK == customerId)
                .Select(ce => ce.EmployerIdFK)
                .ToHashSet(); // HashSet allows for fast lookup

            ViewBag.AssignedEmployerIds = assignedEmployerIds;
            ViewBag.CustomerIdPK = customerId;

            return View(employers);
        }


        [HttpPost]
        public JsonResult SaveCustomerEmployers(int customerId, List<int> selectedEmployerIds)
        {
            try
            {
                // Step 1: Fetch existing CustomerEmployers for the employee
                var existingAssignments = entities.CustomerEmployers.Where(ce => ce.CustomerIdFK == customerId).ToList();

                // Step 2: Remove all existing assignments

                if (existingAssignments.Count > 0)
                {
                    // Remove existing contacts
                    foreach (var contact in existingAssignments)
                    {
                        entities.CustomerEmployers.DeleteObject(contact);
                    }
                }

                // Step 3: Add new assignments if any are selected
                if (selectedEmployerIds != null && selectedEmployerIds.Count > 0)
                {
                    //var newAssignments = selectedEmployerIds.Select(employerId => new CustomerEmployer
                    //{
                    //    CustomerIdFK = employeeId,
                    //    EmployerIdFK = employerId
                    //});

                    foreach (var employerId in selectedEmployerIds)
                    {
                        var newAssignments = new CustomerEmployer
                        {
                            CustomerIdFK = customerId,
                            EmployerIdFK = employerId
                        };
                        entities.CustomerEmployers.AddObject(newAssignments);
                    }

                }

                // Step 4: Save changes to the database
                entities.SaveChanges();

                return Json(new { success = true, message = "Employers successfully assigned to the customer!" });
            }
            catch (Exception ex)
            {
                // Log exception (optional)
                return Json(new { success = false, message = "An error occurred while saving employers." });
            }
        }


        //[HttpGet]
        //public ActionResult ViewCustomerEmployers(int customerId)
        //{
        //    // Fetch assigned employers for the given customer ID
        //    var assignedEmployers = entities.CustomerEmployers
        //                                    .Where(ce => ce.CustomerIdFK == customerId)
        //                                    .Select(ce => new EmployerModel
        //                                    {
        //                                        EmployerIdPK = ce.EmployerIdFK,
        //                                        EmployerName = ce.Employer.EmployerName,
        //                                        EmployerContactEmail = ce.Employer.EmployerContactEmail
        //                                    })
        //                                    .ToList();

        //    // Pass the list to the view
        //    ViewBag.CustomerId = customerId;
        //    return View(assignedEmployers);
        //}

        [HttpGet]
        public ActionResult ViewCustomerEmployers(int customerId)
        {
            // Fetch customer information
            var customer = entities.CustomerEmployers.FirstOrDefault(ce => ce.CustomerIdFK == customerId);
           

            // Fetch assigned employers' details for the customer
            var assignedEmployers = entities.CustomerEmployers
                .Where(ce => ce.CustomerIdFK == customerId)
                .Join(entities.Employers,
                    ce => ce.EmployerIdFK,
                    employer => employer.EmployerIdPK,
                    (ce, employer) => new EmployerModel
                    {
                        EmployerIdPK = employer.EmployerIdPK,
                        EmployerName = employer.EmployerName,
                        EmployerContactEmail = employer.EmployerContactEmail,
                        EmployerContactPhone = employer.EmployerContactPhone
                    })
                .ToList();

            ViewBag.CustomerId = customerId;

            return View(assignedEmployers); // Return to View with the assigned employers
        }

        //[HttpGet]
        //public ActionResult AssignContact(int customerId)
        //{
        //    ViewBag.CustomerId = customerId;

        //    var contacts = entities.Contacts
        //                     .Select(c => new ContactModel
        //                     {
        //                         ContactIdPK = c.ContactIdPK,
        //                         ContactName = c.ContactName,
        //                         ContactEmail = c.ContactEmail,

        //                     })
        //                     .ToList();

        //    return View(contacts);
        //}

        //[HttpPost]
        //public JsonResult SaveCustomerContacts(int customerId, List<int> selectedContactIds)
        //{
        //    // Get the existing contacts for the customer
        //    var existingContacts = entities.CustomerContacts.Where(cc => cc.CustomerId == customerId).ToList();

        //    // Remove existing contacts
        //    if (existingContacts.Count > 0)
        //    {
        //        // Remove existing contacts
        //        foreach (var contact in existingContacts)
        //        {
        //            entities.CustomerContacts.DeleteObject(contact);
        //        }
        //    }
        //    // Add new contacts
        //    if (selectedContactIds != null && selectedContactIds.Count > 0)
        //    {
        //        foreach (var contactId in selectedContactIds)
        //        {
        //            var newContact = new CustomerContact
        //            {
        //                CustomerId = customerId,
        //                ContactId = contactId
        //            };
        //            entities.CustomerContacts.AddObject(newContact);
        //        }
        //    }

        //    // Save changes to the database
        //    entities.SaveChanges();

        //    return Json(new { success = true, message = "Contacts successfully assigned!" });
        //}


        //[HttpGet]
        //public ActionResult ViewAssignedContact(int customerId)
        //{
        //    var assignedContacts = entities.CustomerContacts
        //                             .Where(cc => cc.CustomerId == customerId)
        //                             .Select(cc => new ContactModel
        //                             {
        //                                 ContactIdPK = cc.ContactId,
        //                                 ContactName = cc.Contact.ContactName,
        //                                 ContactEmail = cc.Contact.ContactEmail
        //                             })
        //                             .ToList();

        //    return View(assignedContacts);
        //}

    }
}
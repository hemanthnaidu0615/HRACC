using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class ContactController : Controller
    {

        // GET: Employer
        public HRACCDBEntities entities;
        clsCrud cls;
        public ContactController()
        {
            entities = new HRACCDBEntities();
            cls = new clsCrud();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddContact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddContact(ContactModel contact)
        {

            string message = "";
            try
            {
                message = cls.AddContact(contact);
            }
            catch (Exception e)
            {
                message = e.Message;
            }


            return Json(new { message = message, JsonRequestBehavior.AllowGet });
        }


        public ActionResult ViewContact()
        {
            cls.GetContacts();
            return View(cls);
        }
        public ActionResult EditContact(int id)
        {
            ContactModel cl = cls.GetContactById(id);
            return Json(new { cl = cl, JsonRequestBehavior.AllowGet });
        }

        [HttpGet]
        public ActionResult AssignContact(int customerId)
        {
            var contacts = entities.Contacts
                .Select(c => new ContactModel
                {
                    ContactIdPK = c.ContactIdPK,
                    ContactName = c.ContactName,
                    ContactEmail = c.ContactEmail,
                    ContactPhone = c.ContactPhone
                })
                .ToList();

            var assignedContactIds = entities.CustomerContacts
                .Where(cc => cc.CustomerId == customerId) // <-- updated
                .Select(cc => cc.ContactId)               // <-- updated
                .ToHashSet();

            ViewBag.AssignedContactIds = assignedContactIds ?? new HashSet<int>();
            ViewBag.CustomerId = customerId;

            return View(contacts);
        }



        [HttpPost]
        public JsonResult SaveCustomerContacts(int customerId, List<int> selectedContactIds)
        {
            // Get the existing contacts for the customer
            var existingContacts = entities.CustomerContacts.Where(cc => cc.CustomerId == customerId).ToList();

            // Remove existing contacts
            if (existingContacts.Count > 0)
            {
                // Remove existing contacts
                foreach (var contact in existingContacts)
                {
                    entities.CustomerContacts.DeleteObject(contact);
                }
            }
            // Add new contacts
            if (selectedContactIds != null && selectedContactIds.Count > 0)
            {
                foreach (var contactId in selectedContactIds)
                {
                    var newContact = new CustomerContact
                    {
                        CustomerId = customerId,
                        ContactId = contactId
                    };
                    entities.CustomerContacts.AddObject(newContact);
                }
            }

            // Save changes to the database
            entities.SaveChanges();

            return Json(new { success = true, message = "Contacts successfully assigned!" });
        }


        [HttpGet]
        public ActionResult ViewAssignedContact(int customerId)
        {
            // Step 1: Find all Contact IDs associated with the given Customer ID
            var contactIds = entities.CustomerContacts
                                    .Where(cc => cc.CustomerId == customerId)
                                    .Select(cc => cc.ContactId)
                                    .ToList();

            // Step 2: Query the Contact table to get the details of those Contacts
            var assignedContacts = entities.Contacts
                                            .Where(c => contactIds.Contains(c.ContactIdPK))
                                            .Select(c => new ContactModel
                                            {
                                                ContactIdPK = c.ContactIdPK,
                                                ContactName = c.ContactName,
                                                ContactEmail = c.ContactEmail,
                                                ContactPhone = c.ContactPhone
                                            })
                                            .ToList();

            ViewBag.CustomerId = customerId;
            // Step 3: Return the result to the view
            return View(assignedContacts);
        }

    }
}
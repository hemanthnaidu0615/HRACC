using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class SubContractorController : Controller
    {
        public HRACCDBEntities entities;
        clsCrud cls;
        public SubContractorController()
        {
            entities = new HRACCDBEntities();
            cls = new clsCrud();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddSubContractor()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddSubContractor(SubContractorModel subcontractor)
        {


            string message = "";
            try
            {
                message = cls.AddSubContractor(subcontractor);
            }
            catch (Exception e)
            {
                message = e.Message;
            }


            return Json(new { message = message, JsonRequestBehavior.AllowGet });
        }


        public ActionResult ViewSubContractors()
        {
            cls.GetSubContractors();
            return View(cls);
        }
        public ActionResult EditSubContractor(int id)
        {
            SubContractorModel cl = cls.GetSubContractorById(id);
            return Json(new { cl = cl, JsonRequestBehavior.AllowGet });
        }



        [HttpGet]
        public ActionResult AssignContractorEmployers(int subcontractorId)
        {
            // Call the existing GetCustomers method
            cls.GetEmployers();
            // Get the EmployerList populated by GetEmployers
            var employers = cls.EmployerList.Select(employer => new EmployerModel
            {
                EmployerIdPK = employer.EmployerIdPK,
                EmployerName = employer.EmployerName,
                EmployerContactEmail = employer.EmployerContactEmail
            }).ToList();
           

            // Get the CustomerList populated by GetCustomers

            var assignedEmployerIds = entities.SubcontractorEmployers
                    .Where(cc => cc.SubcontractorId == subcontractorId)
                    .Select(cc => cc.EmployerId)
                    .ToHashSet(); 

            ViewBag.AssignedEmployerIds = assignedEmployerIds ?? new HashSet<int>();
            ViewBag.SubContractorIdPK = subcontractorId;

            return View(employers);
        }

        
        [HttpPost]
        public ActionResult SaveContractorEmployers(int subcontractorId, List<int> selectedEmployerIds)
        {
            if (selectedEmployerIds == null || !selectedEmployerIds.Any())
            {
                return Json(new { success = false, message = "No customers selected!" });
            }

            try
            {
                // Remove existing assignments for the consultant
                var existingAssignments = entities.SubcontractorEmployers
                    .Where(cc => cc.SubcontractorId == subcontractorId)
                    .ToList();

                if (existingAssignments.Count > 0)
                {
                    foreach (var employer in existingAssignments)
                    {
                        entities.SubcontractorEmployers.DeleteObject(employer);
                    }
                }

                // Add new assignments
                foreach (var employerId in selectedEmployerIds)
                {
                    entities.SubcontractorEmployers.AddObject(new SubcontractorEmployer
                    {
                        SubcontractorId = subcontractorId,
                        EmployerId = employerId
                    });
                }

                entities.SaveChanges();

                return Json(new { success = true, message = "Employers assigned successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred: " + ex.Message });
            }
        }


        [HttpGet]
        public ActionResult ViewAssignedContractorsEmployers(int subcontractorId)
        {
            // Fetch consultant information
            var consultant = entities.SubcontractorEmployers.FirstOrDefault(cc => cc.SubcontractorId == subcontractorId);
           

            // Fetch assigned customers' details for the consultant
            var assignedEmployers = entities.SubcontractorEmployers
          .Where(se => se.SubcontractorId == subcontractorId)
          .Join(entities.Employers,
              se => se.EmployerId,
              employer => employer.EmployerIdPK,
              (se, employer) => new EmployerModel
              {
                  EmployerIdPK = employer.EmployerIdPK,
                  EmployerName = employer.EmployerName,
                  EmployerContactEmail = employer.EmployerContactEmail
                  
              })
          .ToList();

            ViewBag.SubContractorIdPK = subcontractorId;
            // Assuming you have a name field

            return View(assignedEmployers); 
        }



    }
}
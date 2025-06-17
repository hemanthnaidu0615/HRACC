/*
using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class WorkerController : Controller
    {
        // GET: Worker
        public HRACCDBEntities entities;
        clsCrud cls;
        public WorkerController()
        {
            entities = new HRACCDBEntities();
            cls = new clsCrud();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddWorker()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddWorker(WorkerModel worker)
        {


            string message = "";
            try
            {
                message = cls.AddWorker(worker);
            }
            catch (Exception e)
            {
                message = e.Message;
            }


            return Json(new { message = message, JsonRequestBehavior.AllowGet });
        }


        public ActionResult ViewWorkers()
        {
            cls.GetWorkers();
            return View(cls);
        }
        public ActionResult EditWorker(int id)
        {
            WorkerModel cl = cls.GetWorkerById(id);
            return Json(new { cl = cl, JsonRequestBehavior.AllowGet });
        }
    }
} */

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
    public class WorkerController : Controller
    {
        public HRACCDBEntities entities;
        clsCrud cls;

        public WorkerController()
        {
            entities = new HRACCDBEntities();
            cls = new clsCrud();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddWorker()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddWorker(WorkerModel worker)
        {
            string message = "";
            try
            {
                // Encrypt sensitive data before storing in the database
                worker.WorkerSalary = EncryptionHelper.Encrypt(worker.WorkerSalary);

                message = cls.AddWorker(worker);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewWorkers()
        {
            // Retrieve employers from the database
            cls.GetWorkers();

            // Decrypt sensitive data after retrieving from the database
            foreach (var worker in cls.WorkerList)
            {
                 worker.WorkerSalary = EncryptionHelper.Decrypt(worker.WorkerSalary);
            } 

            return View(cls);
        }

         public ActionResult EditWorker(int id)
         {
             WorkerModel cl = cls.GetWorkerById(id);

             // Decrypt sensitive data before sending to the view
             cl.WorkerSalary = EncryptionHelper.Decrypt(cl.WorkerSalary);

             return Json(new { cl = cl }, JsonRequestBehavior.AllowGet);
         } 
    }
}
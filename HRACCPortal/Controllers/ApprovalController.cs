using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System.Linq;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly HRACCDBEntities entities = new HRACCDBEntities();

        // List pending timesheets
        public ActionResult Index()
        {
            if (Session["UserRole"]?.ToString() != "1" && Session["UserRole"]?.ToString() != "2")
            {
                return new HttpStatusCodeResult(403, "Access denied: Only Admins and Employers can perform this action.");
            }

            var pendingTimesheets = entities.Timesheets
                .Where(t => t.Approval == "pending")
                .Select(t => new TimesheetViewModel
                {
                    TimesheetId = t.TimesheetId,
                    Month = (int)t.Month,
                    Year = (int)t.Year,
                    TotalHours = (int)t.TotalHours,
                    CustomerName = t.Customer.CustomerName,
                    EmployerName = t.Employer.EmployerName
                })
                .ToList();

            return View(pendingTimesheets);
        }

        [HttpPost]
        public ActionResult Approve(int timesheetId)
        {
            if (Session["UserRole"]?.ToString() != "1" && Session["UserRole"]?.ToString() != "2")
            {
                return new HttpStatusCodeResult(403, "Access denied: Only Admins and Employers can perform this action.");
            }

            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
            if (timesheet != null)
            {
                timesheet.Approval = "approved";
                entities.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult Reject(int timesheetId)
        {
            if (Session["UserRole"]?.ToString() != "1" && Session["UserRole"]?.ToString() != "2")
            {
                return new HttpStatusCodeResult(403, "Access denied: Only Admins and Employers can perform this action.");
            }

            var timesheet = entities.Timesheets.FirstOrDefault(t => t.TimesheetId == timesheetId);
            if (timesheet != null)
            {
                timesheet.Approval = "rejected";
                entities.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}

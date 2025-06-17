using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    public class DeliverableController : Controller
    {
        // GET: Deliverable
        public HRACCDBEntities entities;

        public DeliverableController()
        {
            entities = new HRACCDBEntities();
        }

        // GET: Position/AddDeliverables/{positionId}
        public ActionResult AddDeliverables(int positionId)
        {
            ViewBag.PositionId = positionId;

            var existingDeliverables = entities.Deliverables
                .Where(d => d.PositionId == positionId)
                .Select(d => new DeliverableModel
                {
                    DeliverableId = d.DeliverableId,
                    PositionId = d.PositionId,
                    DeliverableNumber = d.DeliverableNumber,
                    DeliverableDescription = d.DeliverableDescription
                }).ToList();

            return View(existingDeliverables);
        }


        // POST: Position/AddDeliverables
        [HttpPost]
        public ActionResult AddDeliverables(List<DeliverableModel> deliverables)
        {
            if (deliverables != null && deliverables.Any())
            {
                foreach (var item in deliverables)
                {
                    if (!string.IsNullOrWhiteSpace(item.DeliverableNumber) && !string.IsNullOrWhiteSpace(item.DeliverableDescription))
                    {
                        entities.Deliverables.AddObject(new Deliverable
                        {
                            PositionId = item.PositionId,
                            DeliverableNumber = item.DeliverableNumber,
                            DeliverableDescription = item.DeliverableDescription
                        });
                    }
                }
                entities.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "No valid deliverables to add." });
        }

        // GET: Position/ViewDeliverables/{positionId}
        public ActionResult ViewDeliverables(int positionId)
        {
            var deliverables = entities.Deliverables
                                 .Where(d => d.PositionId == positionId)
                                 .Select(d => new DeliverableModel
                                 {
                                     DeliverableId = d.DeliverableId,
                                     PositionId = d.PositionId,
                                     DeliverableNumber = d.DeliverableNumber,
                                     DeliverableDescription = d.DeliverableDescription
                                 })
                                 .ToList();

            ViewBag.PositionId = positionId;
            return View(deliverables);
        }

        [HttpPost]
        public JsonResult DeleteDeliverable(int id)
        {
            var deliverable = entities.Deliverables.FirstOrDefault(d => d.DeliverableId == id);
            if (deliverable != null)
            {
                entities.Deliverables.DeleteObject(deliverable);
                entities.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Deliverable not found." });
        }

    }
}
using HRACCPortal.Edmx;
using HRACCPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRACCPortal.Controllers
{
    [Authorize]
    public class PositionController : Controller
    {
        private readonly PositionModel pmodel;

        public PositionController()
        {
            pmodel = new PositionModel();
        }
        // GET: Position
        [HttpGet]
        public ActionResult ViewPosition()
        {
            pmodel.GetPositionList();
            return View(pmodel);
        }

        //POST: Add Position
        [HttpPost]
        public ActionResult AddPosition(PositionModel pmodel)
        {
            try
            {
               string status = pmodel.AddEditPosition(pmodel);
               return Json(new { message = status, JsonRequestBehavior.AllowGet });
            }
            catch(Exception e){
                return Json(new { message = e.Message, JsonRequestBehavior.AllowGet });
            }
            
        }
        //POST: Get Position for Edit View
        public ActionResult EditPosition(int? id)
        {
            try
            {
                PositionModel positionData = pmodel.GetPositionDetailsById(id);
                
                return Json(new { positionData = positionData, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return View(e.Message);
            }

        }

        //View Position Rate
        public ActionResult ViewPositionRate()
        {
            pmodel.GetPositionRateList();
            return View(pmodel);
        }

        //POST: Get Position Rate Edit View
        public ActionResult EditPositionRate(int? id)
        {
            try
            {
                PositionModel positionRateData = pmodel.GetPositionRateDetailsById(id);

                return Json(new { positionRateData = positionRateData, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return View(e.Message);
            }

        }

        //POST: Add/Edit Position Rate
        [HttpPost]
        public ActionResult AddEditPositionRate(PositionModel pmodel)
        {
            try
            {
                string status = pmodel.AddEditPositionRate(pmodel);
                return Json(new { message = status, JsonRequestBehavior.AllowGet });
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message, JsonRequestBehavior.AllowGet });
            }



        }
    }
}
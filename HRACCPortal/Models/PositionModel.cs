using HRACCPortal.Edmx;
using HRACCPortal.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HRACCPortal.Models
{
    public class PositionModel : PositionObjectModel
    {
        private HRACCDBEntities hRACCDBEntities;

        public PositionModel()
        {
            hRACCDBEntities = new HRACCDBEntities();
        }

        public IEnumerable<SelectListItem> ddlCustomerList
        {
            get
            {
                var customer = hRACCDBEntities.Customers.AsEnumerable().ToList();
                IEnumerable<SelectListItem> items = from value in customer
                                                    select new SelectListItem
                                                    {
                                                        Text = value.CustomerName,
                                                        Value = value.CustomerIdPK.ToString(),
                                                    };
                return items;
            }
        }
        public string AddEditPosition(PositionModel model)
        {
            string statusDetail = string.Empty;
            if (model.PositionIdPK == 0)
            {
                var checkPosition = hRACCDBEntities.Positions.Where(x => x.PositionNumber == model.PositionNumber).ToList();
                if (checkPosition.Count > 0)
                {
                    return "Position number already exist.";
                }
                Position ps = new Position()
                {
                    CustomerIdFK = model.CustomerIdFK,
                    PositionNumber = model.PositionNumber,
                    PositionTitle = model.PositionTitle,
                    PositionFamily = model.PositionFamily,
                    PositionScopeVariant = model.PositionScopeVariant,
                    PurchaseOrderNo = model.PurchaseOrderNo,
                    PurchaseRequisitionNo = model.PurchaseRequisitionNo,
                    Status = model.Status,
                    InactiveDate = model.InactiveDate,
                    InactiveReason = model.InactiveReason,
                    DateAdded = IndianTimeNow,
                    DateUpdated = IndianTimeNow,
                    AddedBy = "ADMIN",
                    UpdatedBy = "ADMIN"
                };

                hRACCDBEntities.Positions.AddObject(ps);
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            else
            {
                var updatePosition = hRACCDBEntities.Positions.Where(x => x.PositionIdPK == model.PositionIdPK).FirstOrDefault();
                updatePosition.CustomerIdFK = model.CustomerIdFK;
                updatePosition.PositionNumber = model.PositionNumber;
                updatePosition.PositionTitle = model.PositionTitle;
                updatePosition.PositionFamily = model.PositionFamily;
                updatePosition.PositionScopeVariant = model.PositionScopeVariant;
                updatePosition.PurchaseOrderNo = model.PurchaseOrderNo;
                updatePosition.PurchaseRequisitionNo = model.PurchaseRequisitionNo;
                updatePosition.Status = model.Status;
                updatePosition.InactiveDate = model.InactiveDate;
                updatePosition.InactiveReason = model.InactiveReason;
                updatePosition.DateUpdated = IndianTimeNow;
                updatePosition.UpdatedBy = "ADMIN";
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            return statusDetail;
        }
        //Edit Position
        public PositionModel GetPositionDetailsById(int? positionPK)
        {
            var positionData = hRACCDBEntities.Positions.Single(x => x.PositionIdPK == positionPK);
            var customerData = hRACCDBEntities.Customers.Where(x => x.CustomerIdPK == positionData.CustomerIdFK).FirstOrDefault();
            PositionModel positionModel = new PositionModel()
            {
                CustomerIdFK = positionData.CustomerIdFK,
                CustomerName = customerData.CustomerName,
                PositionIdPK = positionData.PositionIdPK,
                PositionNumber = positionData.PositionNumber,
                PositionTitle = positionData.PositionTitle,
                PositionFamily = positionData.PositionFamily,
                PositionScopeVariant = positionData.PositionScopeVariant,
                PurchaseOrderNo = positionData.PurchaseOrderNo,
                PurchaseRequisitionNo = positionData.PurchaseRequisitionNo,
                Status = positionData.Status,
                InactiveDate = positionData.InactiveDate,
                InactiveReason = positionData.InactiveReason,
                DateUpdated = positionData.DateUpdated,
                UpdatedBy = "ADMIN"
            };
            return positionModel;
        }
        //view position
        public void GetPositionList()
        {
            PositionList = (from psLts in hRACCDBEntities.Positions
                            join custLts in hRACCDBEntities.Customers
                            on psLts.CustomerIdFK equals custLts.CustomerIdPK
                            select new
                            {
                                psLts.PositionIdPK,
                                psLts.CustomerIdFK,
                                psLts.PositionNumber,
                                psLts.PositionTitle,
                                psLts.PositionFamily,
                                psLts.PositionScopeVariant,
                                psLts.PurchaseOrderNo,
                                psLts.PurchaseRequisitionNo,
                                psLts.Status,
                                psLts.InactiveDate,
                                psLts.InactiveReason,
                                psLts.DateAdded,
                                psLts.DateUpdated,
                                psLts.AddedBy,
                                psLts.UpdatedBy,
                                custLts.CustomerName,
                                

                            }).AsEnumerable().Select(i => new PositionObjectModel
                            {
                                PositionIdPK = i.PositionIdPK,
                                CustomerName = i.CustomerName,
                                PositionNumber = i.PositionNumber,
                                PositionTitle = i.PositionTitle,
                                PositionFamily = i.PositionFamily,
                                PositionScopeVariant = i.PositionScopeVariant,
                                PurchaseOrderNo = i.PurchaseOrderNo,
                                PurchaseRequisitionNo = i.PurchaseRequisitionNo,
                                Status = i.Status,
                                InactiveDate = i.InactiveDate,
                                InactiveReason = i.InactiveReason,
                                DateAdded = i.DateAdded,

                                //DateUpdated = Convert.ToDateTime(i.DateUpdated).ToString("MMM,dd, yyyy"),
                                //Changed by Uzair 
                                DateUpdated = !string.IsNullOrWhiteSpace(i.DateUpdated) && DateTime.TryParse(i.DateUpdated, out var dateUpdated)
                                ? dateUpdated.ToString("MMM, dd, yyyy")
                                : "N/A",
                                AddedBy = i.AddedBy,
                               
                                UpdatedBy = i.UpdatedBy

                            }).ToList();
        }

        //Start -- Position Rate
        //positionList
        public IEnumerable<SelectListItem> ddlPositionList
        {
            get
            {
                var positionList = hRACCDBEntities.Positions.AsEnumerable().ToList();
                IEnumerable<SelectListItem> items = from value in positionList
                                                    select new SelectListItem
                                                    {
                                                        Text = value.PositionNumber.ToString(),
                                                        Value = value.PositionIdPK.ToString(),
                                                    };
                return items;
            }
        }

        //add edit position rate
        public string AddEditPositionRate(PositionModel model)
        {
            string statusDetail = string.Empty;
            if (model.PositionRateIdPK == 0)
            {
                PositionRate pr = new PositionRate()
                {
                    PositionIdFK = model.PositionIdFK,
                    FiscalYearStart = model.FiscalYearStart,
                    FiscalYearEnd = model.FiscalYearEnd,
                    FiscalYearAbbrv = model.FiscalYearAbbrv,
                    Rate = model.Rate,
                    Status = model.Status,
                    InactiveDate = model.InactiveDate,
                    InactiveReason = model.InactiveReason,
                    DateAdded = IndianTimeNow,
                    DateUpdated = IndianTimeNow,
                    AddedBy = "ADMIN",
                    UpdatedBy = "ADMIN"
                };

                hRACCDBEntities.PositionRates.AddObject(pr);
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            else
            {
                var updatePosition = hRACCDBEntities.PositionRates.Where(x => x.PositionRateIdPK == model.PositionRateIdPK).FirstOrDefault();
                updatePosition.PositionIdFK = model.PositionIdFK;
                updatePosition.FiscalYearStart = model.FiscalYearStart;
                updatePosition.FiscalYearEnd = model.FiscalYearEnd;
                updatePosition.FiscalYearAbbrv = model.FiscalYearAbbrv;
                updatePosition.Rate = model.Rate;
                updatePosition.Status = model.Status;
                updatePosition.InactiveDate = model.InactiveDate;
                updatePosition.InactiveReason = model.InactiveReason;
                updatePosition.DateUpdated = IndianTimeNow;
                updatePosition.UpdatedBy = "ADMIN";
                statusDetail = hRACCDBEntities.SaveChanges() == 1 ? "Success" : "Failed";
            }
            return statusDetail;
        }

        //view position rate
        public void GetPositionRateList()
        {
            PositionRateList = (from psrLts in hRACCDBEntities.PositionRates
                                join pstList in hRACCDBEntities.Positions
                                on psrLts.PositionIdFK equals pstList.PositionIdPK
                                select new
                                {
                                    psrLts.PositionRateIdPK,
                                    psrLts.PositionIdFK,
                                    psrLts.FiscalYearStart,
                                    psrLts.FiscalYearEnd,
                                    psrLts.FiscalYearAbbrv,
                                    psrLts.Rate,
                                    psrLts.OvertimeRate,
                                    psrLts.Status,
                                    psrLts.InactiveDate,
                                    psrLts.InactiveReason,
                                    psrLts.DateAdded,
                                    psrLts.DateUpdated,
                                    psrLts.AddedBy,
                                    psrLts.UpdatedBy

                                }).AsEnumerable().Select(i => new PositionObjectModel
                                {
                                    PositionRateIdPK = i.PositionRateIdPK,
                                    PositionIdFK = i.PositionIdFK,
                                    FiscalYearStart = i.FiscalYearStart,
                                    FiscalYearEnd = i.FiscalYearEnd,
                                    FiscalYearAbbrv = i.FiscalYearAbbrv,
                                    Rate = i.Rate,
                                    OvertimeRate=i.OvertimeRate,
                                    Status = i.Status,
                                    InactiveDate = i.InactiveDate,
                                    InactiveReason = i.InactiveReason,
                                    DateAdded = i.DateAdded,
                                   // DateUpdated = i.DateUpdated,
                                    //DateUpdated = Convert.ToDateTime(i.DateUpdated).ToString("MMM,dd, yyyy"),
                                    //Changed by uzair
                                    DateUpdated = !string.IsNullOrWhiteSpace(i.DateUpdated) && DateTime.TryParse(i.DateUpdated, out var dateUpdated)
                                    ? dateUpdated.ToString("MMM, dd, yyyy")
                                    : "N/A",
                                    AddedBy = i.AddedBy,
                                    UpdatedBy = i.UpdatedBy
                                }).ToList();
        }

        //View/Edit Position Rate
        public PositionModel GetPositionRateDetailsById(int? positionRatePK)
        {
            var positionRateData = hRACCDBEntities.PositionRates.Single(x => x.PositionRateIdPK == positionRatePK);
            var positionData = hRACCDBEntities.Positions.Where(x => x.PositionIdPK == positionRateData.PositionIdFK).FirstOrDefault();
            PositionModel positionRateModel = new PositionModel()
            {
                PositionRateIdPK = positionRateData.PositionRateIdPK,
                PositionIdFK = positionRateData.PositionIdFK,
                FiscalYearStart = positionRateData.FiscalYearStart,
                FiscalYearEnd = positionRateData.FiscalYearEnd,
                FiscalYearAbbrv = positionRateData.FiscalYearAbbrv,
                Rate = positionRateData.Rate,
                OvertimeRate = positionRateData.OvertimeRate,
                Status = positionRateData.Status,
                InactiveDate = positionRateData.InactiveDate,
                InactiveReason = positionRateData.InactiveReason,
                DateUpdated = positionRateData.DateUpdated,
                UpdatedBy = "ADMIN"
            };
            return positionRateModel;
        }
    }
}
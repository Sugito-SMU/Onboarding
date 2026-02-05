using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Configuration;
using System.Threading.Tasks;
using Onboarding.Models;
using Onboarding.WebAPIManager;
using Onboarding.Entity;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections;
//using log4net;
using System.Globalization;

namespace Onboarding.Controllers
{
    public partial class RequestorController : Controller
    {
        public ActionResult EmployeeInfo()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }

            EmployeeInfo emplInfo = new EmployeeInfo();
            BindCodeList(emplInfo);
            Request request = (Request)Session["Request"];
            if (request == null)
            {
                return RedirectToAction("Dashboard", "Requestor");
            }
            BindEntityToModel(request, emplInfo);
            ViewBag.ShowSaveSuccessful = false;
            ViewBag.ShowChangeUserType = false;
            return View("EmployeeInfo", emplInfo);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeInfo(EmployeeInfo emplInfo, string FormCommand, string RedirectTo)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }
            ViewBag.ShowSaveSuccessful = false;
            ViewBag.ShowChangeUserType = false;
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            Request request = (Request)Session["Request"];
            HandleStaffTypeChange(request, emplInfo);

            if ((Session["RequestViewMode"] != null) && ((bool)Session["RequestViewMode"] == false))
            {
                BindModelToEntity(emplInfo, request);
                Session["Request"] = request;
            }
            if (FormCommand == "Save")
            {
                BindModelToEntity(emplInfo, request);
                Request rq = api.SaveRequest(request, Helper.Utility.GetCurrentUserId());
                Session["Request"] = request = rq;
                BindEntityToModel(rq, emplInfo);
                ViewBag.ShowSaveSuccessful = true;
                return View("EmployeeInfo", emplInfo);
            }
            else if (FormCommand == "Submit")
            {
                request.Validation = Common.RequestValidation.ValidateRequest(request, false);
                Session["Request"] = request;
                if (request.HasPersonalInfoError())
                {
                    return RedirectToAction("PersonalInfo", "Requestor", new { id = request.ReqId });
                }
                else if (request.HasEmploymentInfoError())
                {
                    return RedirectToAction("EmployeeInfo", "Requestor");
                }
                else if (request.HasOfficeLogisticsError())
                {
                    return RedirectToAction("OfficeLog", "Requestor");
                }
                else if (request.HasSystemAccessError())
                {
                    return RedirectToAction("SystemAccess", "Requestor");
                }
                request.ReqStsCd = Entity.Constant.RequestStatusCode.Submitted;
                Request req = api.SaveRequest(request, Helper.Utility.GetCurrentUserId());
                Session.Clear();
                return RedirectToAction("Dashboard", "Requestor");
            }
            else if (FormCommand == "Amend")
            {
                Session["RequestViewMode"] = false;
                ModelState.Clear();
                BindEntityToModel(request, emplInfo);
                return View("EmployeeInfo", emplInfo);
            }
            else if (FormCommand == "Cancel Request")
            {
                request.ReqStsCd = Entity.Constant.RequestStatusCode.Cancelled;
                Request req = api.SaveRequest(request, Helper.Utility.GetCurrentUserId());
                Session.Clear();
                return RedirectToAction("Dashboard", "Requestor");
            }
            else if (FormCommand == "Delete Request")
            {
                bool res = api.DeleteRequest(request.ReqId, Helper.Utility.GetCurrentUserId());
                Session.Clear();
                return RedirectToAction("Dashboard", "Requestor");
            }
            else if (FormCommand == "Next")
            {
                return RedirectToAction("OfficeLog", "Requestor");
            }
            else if (FormCommand == "Previous")
            {
                return RedirectToAction("PersonalInfo", "Requestor", new { id = request.ReqId });
            }

            if (!string.IsNullOrEmpty(RedirectTo))
            {
                switch (RedirectTo)
                {
                    case "PersonalInfo":
                        return RedirectToAction("PersonalInfo", "Requestor", new { id = request.ReqId });
                    case "EmployeeInfo":
                        return RedirectToAction("EmployeeInfo", "Requestor");
                    case "OfficeLog":
                        return RedirectToAction("OfficeLog", "Requestor");
                    case "SystemAccess":
                        return RedirectToAction("SystemAccess", "Requestor");
                    default:
                        break;
                }
            }

            EmployeeInfo emplInfo2 = new EmployeeInfo();
            Request request2 = (Request)Session["Request"];
            BindEntityToModel(request2, emplInfo2);
            BindCodeList(emplInfo2);
            return View("EmployeeInfo", emplInfo2);
        }

        private void BindCodeList(EmployeeInfo emplInfo)
        {
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            emplInfo.UserTypeList = api.GetUserTypeList();
            emplInfo.UserSubTypeList = api.GetUserSubTypeList().Where(x => x.UserTypeCd == emplInfo.UserTypeCd).OrderBy(x => x.Sequence).ToList();
            ViewBag.CostCentreList = api.GetCostCentreList();
        }

        private void HandleStaffTypeChange(Request req, EmployeeInfo emplInfo)
        {
            if (req != null)
            {
                string prevUserType = (req.EmploymentInfo != null) && (!string.IsNullOrEmpty(req.EmploymentInfo.UserTypeCd)) ? req.EmploymentInfo.UserTypeCd : null;
                string newUserType = ((emplInfo != null) && (!string.IsNullOrEmpty(emplInfo.UserTypeCd))) ? emplInfo.UserTypeCd : null;
                string prevUserSubType = (req.EmploymentInfo != null) && (!string.IsNullOrEmpty(req.EmploymentInfo.UserSubTypeCd)) ? req.EmploymentInfo.UserSubTypeCd : null;
                string newUserSubType = ((emplInfo != null) && (!string.IsNullOrEmpty(emplInfo.UserSubTypeCd))) ? emplInfo.UserSubTypeCd : null;
                if ((prevUserSubType != newUserSubType) || (prevUserType != newUserType))
                {
                    ViewBag.ShowChangeUserType = true;
                    Helper.Utility.SetMandatoryValues(req, true);
                }
                else
                {
                    ViewBag.ShowChangeUserType = false;
                    Helper.Utility.SetMandatoryValues(req, false);
                }
            }
        }

        private void BindEntityToModel(Request request, EmployeeInfo emplInfo)
        {
            ModelState.Clear();
            emplInfo.Request = request;
            Helper.Utility.SetMandatoryValues(request, false);
            if (request.EmploymentInfo != null)
            {
                emplInfo.CostCtrCd = request.EmploymentInfo.CostCtrCd;
                emplInfo.CostCtrDesc = request.EmploymentInfo.CostCtrDesc;
                emplInfo.EmplStartDt = request.EmploymentInfo.EmplStartDt;
                emplInfo.StrEmplStartDt = request.EmploymentInfo.EmplStartDt.HasValue ? request.EmploymentInfo.EmplStartDt.Value.ToString("dd MMM yyyy") : String.Empty;
                if (Common.Validation.IsPermStaff(request.EmploymentInfo.UserSubTypeCd))
                {
                    emplInfo.EmplEndDt = new DateTime(9999, 12, 31);
                    emplInfo.StrEmplEndDt = emplInfo.EmplEndDt.HasValue ? emplInfo.EmplEndDt.Value.ToString("dd MMM yyyy") : String.Empty;
                }
                else
                {
                    emplInfo.EmplEndDt = request.EmploymentInfo.EmplEndDt;
                    emplInfo.StrEmplEndDt = request.EmploymentInfo.EmplEndDt.HasValue ? request.EmploymentInfo.EmplEndDt.Value.ToString("dd MMM yyyy") : String.Empty;
                }
                emplInfo.JobTitle = request.EmploymentInfo.JobTitle;
                emplInfo.ReqEmplId = request.EmploymentInfo.ReqEmplId;
                emplInfo.UserSubTypeCd = request.EmploymentInfo.UserSubTypeCd;
                emplInfo.UserSubTypeDesc = request.EmploymentInfo.UserSubTypeDesc;
                emplInfo.UserTypeCd = request.EmploymentInfo.UserTypeCd;
                emplInfo.UserTypeDesc = request.EmploymentInfo.UserTypeDesc;
            }

            emplInfo.Remarks = request.Remark;


            emplInfo.IsAmended = request.IsAmended.HasValue ? request.IsAmended.Value : false;
            ViewBag.IsAmended = request.IsAmended;
            ViewBag.IsCancelled = request.IsCancelled;
            ViewBag.RequestStatus = request.ReqStsDesc;

            if (request.ReqId > 0)
            {
                ViewBag.Title = string.Format("Request #{0} ({1})", request.ReqId, request.ReqTypeDesc);
            }
            else
            {
                ViewBag.Title = string.Format("New Request ({0})", Session["RequestTypeDesc"]);
            }

            BindCodeList(emplInfo);
            Helper.Utility.SetActionButtons(request, ViewBag, true, true);

            if (Session["RequestViewMode"] != null)
            {
                emplInfo.IsViewMode = (bool)Session["RequestViewMode"];
            }
            else
            {
                if (request.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    Session["RequestViewMode"] = emplInfo.IsViewMode = true;
                }
            }
        }

        private void BindModelToEntity(EmployeeInfo emplInfo, Entity.Request request)
        {
            Entity.RequestEmployment requestEmpl = request.EmploymentInfo;
            if (requestEmpl == null)
            {
                requestEmpl = new RequestEmployment();
                request.EmploymentInfo = requestEmpl;
            }

            if (emplInfo.StrEmplStartDt != null && emplInfo.StrEmplStartDt != "")
            {
                requestEmpl.EmplStartDt = DateTime.ParseExact(emplInfo.StrEmplStartDt, "dd MMM yyyy", CultureInfo.InvariantCulture);
            }
            if (Common.Validation.IsPermStaff(emplInfo.UserSubTypeCd))
            {
                request.EmploymentInfo.EmplEndDt = new DateTime(9999, 12, 31);
            }
            else
            {
                if (emplInfo.StrEmplEndDt != null && emplInfo.StrEmplEndDt != "")
                {
                    requestEmpl.EmplEndDt = DateTime.ParseExact(emplInfo.StrEmplEndDt, "dd MMM yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    requestEmpl.EmplEndDt = null;
                }
            }

            CostCentre cc = Helper.Utility.GetCostCentreFromTextInput(emplInfo.CostCtrDesc);
            if (cc != null)
            {
                requestEmpl.CostCtrCd = cc.CostCtrCd;
                requestEmpl.CostCtrDesc = cc.CostCtrDesc;
            }
            else
            {
                requestEmpl.CostCtrCd = null;
                requestEmpl.CostCtrDesc = null;
            }

            requestEmpl.JobTitle = Common.Security.Sanitize(emplInfo.JobTitle);
            requestEmpl.UserSubTypeCd = Common.Security.Sanitize(emplInfo.UserSubTypeCd);
            requestEmpl.UserTypeCd = Common.Security.Sanitize(emplInfo.UserTypeCd);
            request.Remark = Helper.Utility.DecodeNewLine(Common.Security.Sanitize(Helper.Utility.EncodeNewLine(emplInfo.Remarks)));

            //Disabled check-boxes will assign false value to the model, 
            // hence the mandatory default value that is previously set is reset to false.
            // Need to revert the mandatory default value
            Helper.Utility.SetMandatoryValues(request, false);
        }
    }
}
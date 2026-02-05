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
using System.IO;

namespace Onboarding.Controllers
{
    public partial class RequestorController : Controller
    {
        [HttpGet]
        public ActionResult OfficeLog()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }

            OfficeLog offLog = new OfficeLog();
            Request request = (Request)Session["Request"];
            if (request == null)
            {
                return RedirectToAction("Dashboard", "Requestor");
            }
            BindEntityToModel(request, offLog);
            ViewBag.ShowSaveSuccessful = false;
            return View("OfficeLog", offLog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OfficeLog(OfficeLog offLog, string FormCommand, string RedirectTo)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }
            ViewBag.ShowSaveSuccessful = false;
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            Request request = (Request)Session["Request"];
            if ((Session["RequestViewMode"] != null) && ((bool)Session["RequestViewMode"] == false))
            {
                BindModelToEntity(offLog, request);
                Session["Request"] = request;
            }
            if (FormCommand == "Save")
            {
                Request rq = api.SaveRequest(request, Helper.Utility.GetCurrentUserId());
                Session["Request"] = request = rq;
                BindEntityToModel(rq, offLog);
                ViewBag.ShowSaveSuccessful = true;
                return View("OfficeLog", offLog);
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
                BindEntityToModel(request, offLog);
                return View("OfficeLog", offLog);
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
                return RedirectToAction("SystemAccess", "Requestor");
            }
            else if (FormCommand == "Previous")
            {
                return RedirectToAction("EmployeeInfo", "Requestor");
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
            return RedirectToAction("OfficeLog", "Requestor");
        }

        private void BindEntityToModel(Request request, OfficeLog offLog)
        {
            ModelState.Clear();
            offLog.Request = request;
            Helper.Utility.SetMandatoryValues(request, false);
            if (request.Resources != null)
            {
                if (request.Resources.IsPCSelected != null)
                    offLog.IsPCSelected = request.Resources.IsPCSelected.Value;
                offLog.CorpTitleLevelCd = request.Resources.CorpTitleLevelCd;
                offLog.CorpTitleLevelDesc = request.Resources.CorpTitleLevelDesc;
                if (request.Resources.IsIPPhoneSelected != null)
                    offLog.IsIPPhoneSelected = request.Resources.IsIPPhoneSelected.Value;                
                offLog.Building = request.Resources.Building;
                offLog.Floor = request.Resources.Floor;
                offLog.Room = request.Resources.Room;
            }
            offLog.IsAmended = request.IsAmended.HasValue ? request.IsAmended.Value : false;
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

            Helper.Utility.SetActionButtons(request, ViewBag, true, true);

            if (Session["RequestViewMode"] != null)
            {
                offLog.IsViewMode = (bool)Session["RequestViewMode"];
            }
            else
            {
                if (request.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    Session["RequestViewMode"] = offLog.IsViewMode = true;
                }
            }
        }

        private void BindModelToEntity(OfficeLog offLog, Request request)
        {
            if (request.Resources == null)
            {
                request.Resources = new RequestResources();
            }
            request.Resources.IsPCSelected = offLog.IsPCSelected;
            if (offLog.IsPCSelected == true)
            {
                request.Resources.CorpTitleLevelCd = offLog.CorpTitleLevelCd;
            }
            else
            {
                request.Resources.CorpTitleLevelCd = null;
            }
            request.Resources.IsIPPhoneSelected = offLog.IsIPPhoneSelected;            
            request.Resources.Building = Common.Security.Sanitize(offLog.Building);
            request.Resources.Floor = Common.Security.Sanitize(offLog.Floor);
            request.Resources.Room = Common.Security.Sanitize(offLog.Room);

            //Disabled check-boxes will assign false value to the model, 
            // hence the mandatory default value that is previously set is reset to false.
            // Need to revert the mandatory default value
            Helper.Utility.SetMandatoryValues(request, false);
        }
    }
}
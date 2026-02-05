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
using Newtonsoft.Json.Converters;

namespace Onboarding.Controllers
{
    public partial class RequestorController : Controller
    {
        [HttpGet]
        public ActionResult PersonalInfo(int id)
        {
            if (Helper.Utility.IsRedirectToAgentForm())
            {
                return RedirectToAction("AgentRequest", "Agent", new { id = id});
            }

            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }

            Onboarding.WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            PersonalInfo pInfo = new PersonalInfo();
            Request request = (Request)Session["Request"];
            if (id > 0)
            {
                if ((request == null) || (request.ReqId != id))
                {
                    Session.Clear();
                    Session["Request"] = request = api.GetRequest(id, Helper.Utility.GetCurrentUserId());
                }
            }
            else
            {
                if ((request == null) && (Session["RequestTypeCd"] != null) && (!string.IsNullOrEmpty((string)Session["RequestTypeCd"])))
                {
                    request = new Request();
                    request.ReqTypeCd = (string)Session["RequestTypeCd"];
                    request.ReqStsCd = Entity.Constant.RequestStatusCode.Draft;
                }
            }
            BindEntityToModel(request, pInfo);
            ViewBag.ShowSaveSuccessful = false;
            return View("PersonalInfo", pInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonalInfo(PersonalInfo pInfo, string FormCommand, string RedirectTo)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }

            ViewBag.ShowSaveSuccessful = false;

            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            Request request = null;
            if (Session["Request"] != null)
            {
                request = (Request)Session["Request"];
            }
            else
            {
                request = new Entity.Request();
                request.ReqStsCd = Entity.Constant.RequestStatusCode.Draft;
                request.ReqTypeCd = (string)Session["RequestTypeCd"];
                request.ReqTypeDesc = (string)Session["RequestTypeDesc"];
            }

            if ((Session["RequestViewMode"] != null) && ((bool)Session["RequestViewMode"] == false))
            {
                BindModelToEntity(pInfo, request);
                Session["Request"] = request;
            }

            if (FormCommand == "Save")
            {
                Entity.Request request2 = api.SaveRequest(request, Helper.Utility.GetCurrentUserId());
                Session["Request"] = request2;
                BindEntityToModel(request2, pInfo);
                ViewBag.ShowSaveSuccessful = true;
                return View("PersonalInfo", pInfo);
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
                BindEntityToModel(request, pInfo);
                return View("PersonalInfo", pInfo);
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

            return RedirectToAction("PersonalInfo", "Requestor", new { id = request.ReqId });
        }

        private void BindCodeList(PersonalInfo pInfo)
        {
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            pInfo.GenderList = api.GetGenderList();
            pInfo.SalutationList = api.GetSalutationList();
            ViewBag.RaceList = api.GetRaceList();
        }

        private void BindEntityToModel(Request request, PersonalInfo pInfo)
        {
            Helper.Utility.SetMandatoryValues(request, false);

            ModelState.Clear();
            pInfo.Request = request;
            pInfo.ReqId = request.ReqId;
            pInfo.ReqStsCd = request.ReqStsCd;
            pInfo.ReqStsDesc = request.ReqStsDesc;
            pInfo.ReqTypeCd = request.ReqTypeCd;
            pInfo.ReqTypeDesc = request.ReqTypeDesc;
            pInfo.FirstName = request.FirstName;
            pInfo.LastName = request.LastName;
            pInfo.PrefNm = request.PrefNm;
            pInfo.SalutCd = request.SalutCd;
            pInfo.SalutDesc = request.SalutDesc;
            pInfo.PrsnEmail = request.PrsnEmail;
            pInfo.MobileNo = request.MobileNo;
            pInfo.RaceCd = request.RaceCd;
            pInfo.RaceDesc = request.RaceDesc;
            pInfo.GenderCd = request.GenderCd;
            pInfo.GenderDesc = request.GenderDesc;
            pInfo.IsExistingStaff = request.IsExistingStaff;
            pInfo.ExSmuStd = request.IsExSmuStd;
            pInfo.Remark = request.Remark;
            pInfo.IsActive = request.IsActive;
            pInfo.IsAmended = request.IsAmended.HasValue ? request.IsAmended.Value : false;
            ViewBag.RequestStatus = true;
            ViewBag.IsAmended = request.IsAmended;
            ViewBag.IsCancelled = request.IsCancelled;
            ViewBag.RequestStatus = request.ReqStsDesc;

            if (request != null && request.ReqId > 0)
            {
                ViewBag.Title = string.Format("Request #{0} ({1})", request.ReqId, request.ReqTypeDesc);
            }
            else
            {
                ViewBag.Title = string.Format("New Request ({0})", Session["RequestTypeDesc"]);
            }

            BindCodeList(pInfo);
            Helper.Utility.SetActionButtons(request, ViewBag, false, true);

            if (Session["RequestViewMode"] != null)
            {
                pInfo.IsViewMode = (bool)Session["RequestViewMode"];
            }
            else
            {
                if (request.ReqStsCd != Entity.Constant.RequestStatusCode.Draft)
                {
                    Session["RequestViewMode"] = pInfo.IsViewMode = true;
                }
            }
        }

        private void BindModelToEntity(PersonalInfo pInfo, Request request)
        {
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();

            request.FirstName = Common.Security.Sanitize(pInfo.FirstName);
            request.LastName = Common.Security.Sanitize(pInfo.LastName);
            request.PrefNm = Common.Security.Sanitize(pInfo.PrefNm);
            request.GenderCd = Common.Security.Sanitize(pInfo.GenderCd);
            if ((!string.IsNullOrWhiteSpace(pInfo.PrsnEmail)) &&
                (Common.Security.IsValidEmail(pInfo.PrsnEmail)))
            {
                request.PrsnEmail = pInfo.PrsnEmail;
            }
            else
            {
                request.PrsnEmail = Common.Security.Sanitize(pInfo.PrsnEmail);
            }

            request.MobileNo = pInfo.MobileNo;
            if (!string.IsNullOrWhiteSpace(pInfo.RaceDesc))
            {
                List<Race> raceList = api.GetRaceList();
                Race r = raceList.Where(x => x.RaceDesc.Trim().ToLower() == pInfo.RaceDesc.Trim().ToLower()).FirstOrDefault();
                if (r != null)
                {
                    request.RaceCd = r.RaceCd;
                    request.RaceDesc = r.RaceDesc;
                }
                else
                {
                    request.RaceCd = null;
                    request.RaceDesc = null;
                }
            }

            request.SalutCd = Common.Security.Sanitize(pInfo.SalutCd);
            request.IsExSmuStd = pInfo.ExSmuStd;
            request.IsExistingStaff = pInfo.IsExistingStaff;

            //Disabled check-boxes will assign false value to the model, 
            // hence the mandatory default value that is previously set is reset to false.
            // Need to revert the mandatory default value
            Helper.Utility.SetMandatoryValues(request, false);
        }

        public ActionResult Taber()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestForm))
            {
                return View("Unauthorized");
            }

            Request request = new Request();
            if (Session["Request"] != null)
            {
                request = (Request)Session["Request"];
            }
            return PartialView("_taber", request);
        }
    }
}

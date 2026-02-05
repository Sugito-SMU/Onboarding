using System.Web.Mvc;
using Onboarding.Models;
using Onboarding.WebAPIManager;
using System.Collections.Generic;

namespace Onboarding.Controllers
{
    public partial class RequestorController : Controller
    {
        [HttpGet]
        [Authorize]
        public ActionResult NewRequest()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppCreateRequest))
            {
                return View("Unauthorized");
            }

            Session["hideSaveButton"] = null;
            NewRequest newRequest = new NewRequest();
            if (Session["NewRequest"] == null)
            {
                Onboarding.WebAPIManager.WebAPIManager api = new Onboarding.WebAPIManager.WebAPIManager();
                newRequest.RequestTypes = api.GetRequestTypeList();
                Session["NewRequest"] = newRequest;

                List<Entity.CostCentre> CostCentreList = api.GetCostCentreList();
                List<Entity.UserType> UserTypeList = api.GetUserTypeList();
                List<Entity.UserSubType> UserSubTypeList = api.GetUserSubTypeList();
                List<Entity.Salutation> SalutationList = api.GetSalutationList();
                List<Entity.Gender> GenderList = api.GetGenderList();
                List<Entity.Race> RaceList = api.GetRaceList();
            }
            else
            {
                newRequest = (NewRequest)Session["NewRequest"];
            }
            return View("NewRequest", newRequest);
        }

        [HttpPost]
        public ActionResult NewRequest(NewRequest newRequest)
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppCreateRequest))
            {
                return View("Unauthorized");
            }

            if (ModelState.IsValid)
            {
                string requestTypeCd = Common.Security.Sanitize(newRequest.SelectedReqTypeCd);
                Onboarding.WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
                Session["RequestTypeCd"] = requestTypeCd;
                Session["RequestTypeDesc"] = api.GetRequestTypeDesc(requestTypeCd);
                Session["Request"] = null;
            }
            return RedirectToAction("PersonalInfo", "Requestor", new { id = 0 });
        }
    }
}
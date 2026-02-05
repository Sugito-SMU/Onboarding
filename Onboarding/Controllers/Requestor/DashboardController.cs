using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Onboarding.Models;

namespace Onboarding.Controllers
{
    public partial class RequestorController : Controller
    {
        [HttpGet]     
        [Authorize]
        public ActionResult Dashboard()
        {
            if (!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestorDashboard))
            {
                return View("Unauthorized");
            }

            Session.Clear();
            Models.RequestCount reqCount = new Models.RequestCount();
            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            string userid = Helper.Utility.GetCurrentUserId();
            if (string.IsNullOrEmpty(userid))
            {
                return View("Unauthorized");
            }
            List<Entity.RequestCountForRequestor> listReqCount = api.GetRequestCount(userid);
            Models.RequestCount mReqCount = new Models.RequestCount();
            mReqCount.RequestCounts = listReqCount;
            if (listReqCount != null)
            {
                if (listReqCount.Where(m => m.ReqStsCd == Entity.Constant.RequestStatusCode.Draft).FirstOrDefault() != null)
                    mReqCount.ReqDraftCount = listReqCount.Where(m => m.ReqStsCd == Entity.Constant.RequestStatusCode.Draft).FirstOrDefault().ReqCount;

                if (listReqCount.Where(m => m.ReqStsCd == Entity.Constant.RequestStatusCode.Submitted).FirstOrDefault() != null)
                    mReqCount.ReqSubACount = listReqCount.Where(m => m.ReqStsCd == Entity.Constant.RequestStatusCode.Submitted).FirstOrDefault().ReqCount;

                if (listReqCount.Where(m => m.ReqStsCd == Entity.Constant.RequestStatusCode.Closed).FirstOrDefault() != null)
                    mReqCount.ReqClsCount = listReqCount.Where(m => m.ReqStsCd == Entity.Constant.RequestStatusCode.Closed).FirstOrDefault().ReqCount;

                if (listReqCount.Where(m => m.ReqStsCd == Entity.Constant.RequestStatusCode.Cancelled).FirstOrDefault() != null)
                    mReqCount.ReqCanCount = listReqCount.Where(m => m.ReqStsCd == Entity.Constant.RequestStatusCode.Cancelled).FirstOrDefault().ReqCount;

            }

            return View("Dashboard", mReqCount);
        }

        public ActionResult MyRequestList(string id)
        {
            if ((!Helper.Utility.HasPermission(Entity.Constant.AppResource.AppRequestListForRequestor)) ||
                (!Common.Security.IsValidAlphaNum(id)))
            {
                return View("Unauthorized");
            }

            WebAPIManager.WebAPIManager api = new WebAPIManager.WebAPIManager();
            List<Entity.RequestStatus> reqstatusList = api.GetRequestStatusList();
            Entity.RequestStatus reqStatus = reqstatusList.Where(x => x.ReqStsCd.ToLower().Trim() == id.ToLower().Trim()).FirstOrDefault();
            if (reqStatus != null)
            {
                ViewBag.StatusName = reqStatus.ReqStsDesc;
            }
            ViewBag.StatusID = id;
            Session["StatusCode"] = id;
            Models.RequestCount mReqCount = new Models.RequestCount();
            mReqCount.ReqStsCd = id;
            return View("MyRequestList", mReqCount);
        }
    }
}
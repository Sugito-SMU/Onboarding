using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Onboarding.ServiceManager;
using Onboarding.Entity;
using Onboarding.Common;


namespace Onboarding.WebAPI.Controllers
{
    public class RequestListForRequestorController : ApiController
    {
        [HttpGet]
        [Route("api/RequestListForRequestor/{requestStatusCd}/{userId}")]
        [ResponseType(typeof(RequestListForRequestor))]
        public IHttpActionResult Get(string requestStatusCd, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequestListForRequestor))
            {
                RequestListForRequestorManager reqListMgr = new RequestListForRequestorManager();
                RequestListForRequestor reqList = reqListMgr.GetRequestListForRequestor(requestStatusCd, userId);
                return Ok(reqList);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}

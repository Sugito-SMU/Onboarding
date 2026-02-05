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
    public class RequestCountForRequestorController : ApiController
    {

        [HttpGet]
        [Route("api/RequestCountForRequestor/{userId}")]
        [ResponseType(typeof(Entity.RequestCountForRequestor))]
        public IHttpActionResult Get(string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequestListForRequestor))
            {
                RequestCountForRequestorManager reqCountMgr = new RequestCountForRequestorManager();
                List<RequestCountForRequestor> reqCountList = reqCountMgr.GetRequestCountForRequestor(userId);
                return Ok(reqCountList);
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}

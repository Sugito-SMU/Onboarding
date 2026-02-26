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
    public class RequestCountForHRAdminController : ApiController
    {
        
        [HttpGet]
        [Route("api/RequestCountForHRAdmin/{userId}")]
        [ResponseType(typeof(Entity.RequestCountForHRAdmin))]
        public IHttpActionResult Get(string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequestListForHRAdmin))
            {
                RequestCountForAgentManager reqCountMgr = new RequestCountForAgentManager();
                List<RequestCountForHRAdmin> reqCountList = reqCountMgr.GetRequestCountForHRAdmin(userId);
                return Ok(reqCountList);
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}

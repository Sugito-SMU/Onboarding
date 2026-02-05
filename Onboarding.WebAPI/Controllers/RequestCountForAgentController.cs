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
    public class RequestCountForAgentController : ApiController
    {
        
        [HttpGet]
        [Route("api/RequestCountForAgent/{userId}")]
        [ResponseType(typeof(Entity.RequestCountForAgent))]
        public IHttpActionResult Get(string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequestListForAgent))
            {
                RequestCountForAgentManager reqCountMgr = new RequestCountForAgentManager();
                List<RequestCountForAgent> reqCountList = reqCountMgr.GetRequestCountForAgent(userId);
                return Ok(reqCountList);
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}

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
    public class RequestListForAgentController : ApiController
    {

        [HttpGet]
        [Route("api/RequestListForAgent/{taskStatusCd}/{userId}")]
        [ResponseType(typeof(RequestListForAgent))]
        public IHttpActionResult Get(string taskStatusCd, string userId)
        {
            if (Utility.HasPermission(userId, Entity.Constant.AppResource.ApiRequestListForAgent))
            {
                RequestListForAgentManager reqListMgr = new RequestListForAgentManager();
                RequestListForAgent reqList = reqListMgr.GetRequestListForAgent(taskStatusCd, userId);
                return Ok(reqList);
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
